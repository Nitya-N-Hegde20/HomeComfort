# clear_cosmos.py
import os
from azure.cosmos import CosmosClient
from dotenv import load_dotenv

load_dotenv()

client = CosmosClient(
    url=os.getenv("COSMOS_ENDPOINT"),
    credential=os.getenv("COSMOS_KEY")
)
container = client \
    .get_database_client(os.getenv("COSMOS_DATABASE")) \
    .get_container_client(os.getenv("COSMOS_CONTAINER"))

items = list(container.query_items(
    query="SELECT c.id FROM c",
    enable_cross_partition_query=True
))

print(f"Deleting {len(items)} items...")
for item in items:
    container.delete_item(item["id"], partition_key=item["id"])
    print(f"Deleted: {item['id']}")

print("Cosmos DB cleared!")