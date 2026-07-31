import os
import json
import requests
from openai import OpenAI
from azure.cosmos import CosmosClient
from dotenv import load_dotenv

load_dotenv()

# OpenAI client (same as main.py)
openai_client = OpenAI(
    base_url=os.getenv("AZURE_OPENAI_ENDPOINT"),
    api_key=os.getenv("AZURE_OPENAI_KEY"),
)

# Cosmos DB client
cosmos_client = CosmosClient(
    url=os.getenv("COSMOS_ENDPOINT"),
    credential=os.getenv("COSMOS_KEY")
)
database = cosmos_client.get_database_client(os.getenv("COSMOS_DATABASE"))
container = database.get_container_client(os.getenv("COSMOS_CONTAINER"))

def get_embedding(text):
    response = openai_client.embeddings.create(
        model="text-embedding-3-small",  # cheap embedding model
        input=text
    )
    return response.data[0].embedding

def main():
    # Fetch products from your .NET API
    print("Fetching products from API...")
    api_url = os.getenv("DOTNET_API_URL", "https://localhost:7153")
    verify_ssl = os.getenv("DOTNET_API_VERIFY_SSL", "false").lower() == "true"
    response = requests.get(f"{api_url}/api/products", verify=verify_ssl)
    products = response.json()
    print(f"Found {len(products)} products")
    # Add this before the product loop in generate_embeddings.py
    print("Clearing existing embeddings...")
    existing = list(container.query_items(
    query="SELECT c.id FROM c",
    enable_cross_partition_query=True
    ))
    for item in existing:
        container.delete_item(item["id"], partition_key=item["id"])
        print(f"Deleted {len(existing)} existing items")
    for product in products:
        # Combine name + description for richer embedding
        text_to_embed = f"{product['name']} {product.get('description', '')}"
        print(f"Generating embedding for: {product['name']}")

        embedding = get_embedding(text_to_embed)

        # Store in Cosmos DB
        doc = {
            "id": str(product["id"]),
            "name": product["name"],
            "description": product.get("description", ""),
            "price": product["price"],
            "categoryId": product.get("categoryId"),
            "embedding": embedding
        }

        container.upsert_item(doc)
        print(f"Stored: {product['name']}")

    print("Done! All products embedded and stored.")

if __name__ == "__main__":
    main()