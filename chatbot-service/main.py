from fastapi import FastAPI
from pydantic import BaseModel
from openai import OpenAI
from azure.cosmos import CosmosClient
from azure.servicebus import ServiceBusClient
from dotenv import load_dotenv
import os
import json
import requests
import numpy as np
import threading
from fastapi.middleware.cors import CORSMiddleware

load_dotenv()

app = FastAPI()
app.add_middleware(
    CORSMiddleware,
    allow_origins=[
        "http://localhost:4200",
        "https://nice-pond-080d28600.7.azurestaticapps.net"
    ],
    allow_methods=["*"],
    allow_headers=["*"],
)
# OpenAI client
openai_client = OpenAI(
    base_url=os.getenv("AZURE_OPENAI_ENDPOINT"),
    api_key=os.getenv("AZURE_OPENAI_KEY"),
)

# Cosmos DB client
cosmos_client = CosmosClient(
    url=os.getenv("COSMOS_ENDPOINT"),
    credential=os.getenv("COSMOS_KEY")
)
container = cosmos_client \
    .get_database_client(os.getenv("COSMOS_DATABASE")) \
    .get_container_client(os.getenv("COSMOS_CONTAINER"))

DOTNET_API_URL = os.getenv("DOTNET_API_URL", "https://localhost:7153")
VERIFY_SSL = os.getenv("DOTNET_API_VERIFY_SSL", "false").lower() == "true"

class ChatRequest(BaseModel):
    message: str

def get_embedding(text: str):
    response = openai_client.embeddings.create(
        model="text-embedding-3-small",
        input=text
    )
    return response.data[0].embedding

def extract_intent(message: str):
    response = openai_client.responses.create(
        model=os.getenv("AZURE_OPENAI_DEPLOYMENT"),
        input=[
            {
                "role": "system",
                "content": """You are a product search assistant for a home comfort products store.
Extract the user's search intent and return ONLY a JSON object with these fields:
- category: the type of product they want (string, or null if unclear)
- max_price: maximum price in INR (number, or null if not mentioned)
- keywords: key search terms (list of strings)

Example output:
{"category": "clock", "max_price": null, "keywords": ["wall clock", "clock"]}

Return ONLY the JSON. No explanation, no markdown, no extra text."""
            },
            {
                "role": "user",
                "content": message
            }
        ]
    )
    raw = response.output_text
    return json.loads(raw)

def vector_search(query_text: str, top_k: int = 3):
    query_embedding = np.array(get_embedding(query_text))

    items = list(container.query_items(
        query="SELECT c.id, c.name, c.description, c.price, c.embedding FROM c",
        enable_cross_partition_query=True
    ))

    print(f"DEBUG: Found {len(items)} items in Cosmos DB")

    results = []
    for item in items:
        product_embedding = np.array(item["embedding"])
        distance = 1 - np.dot(query_embedding, product_embedding) / (
            np.linalg.norm(query_embedding) * np.linalg.norm(product_embedding)
        )
        results.append({
            "id": item["id"],
            "name": item["name"],
            "description": item.get("description", ""),
            "price": item.get("price", 0),
            "similarityScore": round(distance, 4)
        })

    results.sort(key=lambda x: x["similarityScore"])
    relevant = [r for r in results if r["similarityScore"] < 0.6]
    return relevant[:top_k]

def get_full_product(product_id: str):
    try:
        response = requests.get(
            f"{DOTNET_API_URL}/api/products/{product_id}",
            verify=VERIFY_SSL
        )
        if response.status_code == 200:
            return response.json()
        return None
    except Exception:
        return None

def embed_and_store_product(product_id: int):
    """Fetch product from .NET API, generate embedding, store in Cosmos DB."""
    try:
        print(f"Embedding product {product_id}...")
        product = get_full_product(str(product_id))

        if not product:
            print(f"Product {product_id} not found in .NET API")
            return

        text_to_embed = f"{product['name']} {product.get('description', '')}"
        embedding = get_embedding(text_to_embed)

        doc = {
            "id": str(product["id"]),
            "name": product["name"],
            "description": product.get("description", ""),
            "price": product["price"],
            "categoryId": product.get("categoryId"),
            "embedding": embedding
        }

        container.upsert_item(doc)
        print(f"Stored embedding for: {product['name']}")

    except Exception as e:
        print(f"Error embedding product {product_id}: {e}")

def start_service_bus_listener():
    """Background thread that listens to Service Bus queue."""
    connection_string = os.getenv("SERVICE_BUS_CONNECTION_STRING")
    queue_name = os.getenv("SERVICE_BUS_QUEUE", "product-created")

    if not connection_string:
        print("SERVICE_BUS_CONNECTION_STRING not set — listener not started")
        return

    print(f"Starting Service Bus listener on queue: {queue_name}")

    with ServiceBusClient.from_connection_string(connection_string) as client:
        with client.get_queue_receiver(queue_name) as receiver:
            for message in receiver:
                try:
                    body = json.loads(str(message))
                    print(f"Received message: {body}")

                    if body.get("eventType") == "product-created":
                        product_id = body.get("productId")
                        embed_and_store_product(product_id)

                    # Mark message as complete (remove from queue)
                    receiver.complete_message(message)

                except Exception as e:
                    print(f"Error processing message: {e}")
                    # Abandon message (returns to queue for retry)
                    receiver.abandon_message(message)

@app.on_event("startup")
def startup_event():
    """Start Service Bus listener in background thread when app starts."""
    thread = threading.Thread(target=start_service_bus_listener, daemon=True)
    thread.start()
    print("Service Bus listener thread started")
def notify_missing_product(search_term: str):
    try:
        response = requests.post(
            f"{DOTNET_API_URL}/api/products/notify-missing",
            json=search_term,
            verify=VERIFY_SSL
        )
        if response.status_code == 200:
            print(f"Notified admin about missing product: {search_term}")
        else:
            print(f"Notification failed: {response.status_code}")
    except Exception as e:
        print(f"Error sending notification: {e}")

@app.post("/chat")
def chat(request: ChatRequest):
    intent = extract_intent(request.message)
    search_text = " ".join(intent.get("keywords", [request.message]))
    matches = vector_search(search_text)

    max_price = intent.get("max_price")
    if max_price:
        matches = [m for m in matches if m.get("price", 0) <= max_price]

    enriched = []
    for match in matches:
        full_product = get_full_product(match["id"])
        if full_product:
            full_product["similarityScore"] = match["similarityScore"]
            enriched.append(full_product)
        else:
            enriched.append(match)

    if not enriched:
        # No matches found — notify admin via .NET API
        notify_missing_product(search_text)

    return {
        "intent": intent,
        "matches": enriched,
        "message": request.message
    }

