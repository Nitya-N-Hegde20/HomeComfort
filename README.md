# 🏠 HomeComfortHub

**HomeComfortHub** is a full-stack, AI-powered product discovery application built using **Angular, ASP.NET Core, Python, Azure OpenAI, Azure SQL, Cosmos DB, and Azure Service Bus**.

The application combines traditional keyword search with **AI-powered semantic search**, allowing users to discover relevant products even when their search terms don't exactly match the product names stored in the database.

It also includes an AI chatbot for natural-language product discovery and an event-driven architecture that automatically generates embeddings whenever new products are added.

---

# ✨ Key Features

* 🛍️ Product and category browsing
* 🔎 SQL-based keyword search
* 🧠 AI-powered semantic product search
* 💬 Natural-language shopping chatbot
* 🔢 Vector embeddings for product similarity
* ⚡ Event-driven product embedding generation
* 📨 Azure Service Bus background processing
* 📧 Automatic email notification for unsuccessful searches
* ⏱️ 24-hour duplicate-notification prevention
* ☁️ Cloud deployment using Microsoft Azure
* 🔗 Amazon and Flipkart affiliate links
* 🗄️ Azure SQL as the primary product database
* 🌌 Cosmos DB for product vector representations

---

# 🏗️ Architecture

HomeComfortHub follows a distributed architecture consisting of three major application components:

1. **Angular Frontend**
2. **ASP.NET Core Web API**
3. **Python AI Service**

```text
                        ┌──────────────────────────┐
                        │         Angular          │
                        │    Azure Static Web Apps │
                        └────────────┬─────────────┘
                                     │
                                     │ REST
                                     ▼
                        ┌──────────────────────────┐
                        │      ASP.NET Core API    │
                        │      Business Logic      │
                        └───────┬─────────┬────────┘
                                │         │
                                │         │ Product Created Event
                                ▼         ▼
                        ┌───────────┐  ┌─────────────────┐
                        │ Azure SQL │  │ Azure Service   │
                        │ Database  │  │ Bus Queue       │
                        └───────────┘  └────────┬────────┘
                                               │
                                               ▼
                                     ┌─────────────────────┐
                                     │   Python AI Service │
                                     │                     │
                                     │ • Intent Extraction │
                                     │ • Semantic Search   │
                                     │ • Embeddings        │
                                     └───────┬───────┬─────┘
                                             │       │
                           ┌─────────────────┘       └──────────────┐
                           ▼                                      ▼
                  ┌─────────────────┐                    ┌─────────────────┐
                  │  Azure OpenAI   │                    │    Cosmos DB    │
                  │                 │                    │                 │
                  │ GPT-5-mini      │                    │ Product Vector  │
                  │ Embedding Model │                    │ Embeddings      │
                  └─────────────────┘                    └─────────────────┘
```

---

# 🔄 Application Flow

## 1. Product Browsing

When a user opens HomeComfortHub:

```text
User
 ↓
Angular
 ↓
.NET API
 ↓
Azure SQL
 ↓
Products returned to Angular
```

Angular is hosted using **Azure Static Web Apps**.

The ASP.NET Core API retrieves product and category information from **Azure SQL Database** and returns it to the frontend.

---

## 2. Keyword Search

When a user searches for a product:

```http
GET /api/products/search?term=clock
```

The request follows:

```text
Angular
   ↓
ASP.NET Core API
   ↓
SQL LIKE Search
   ↓
Azure SQL
```

If matching products exist, they are immediately returned to the user.

---

# 🧠 Semantic Search

Traditional keyword searches cannot always understand what a user actually means.

For example:

```text
User searches:
"wall clock"

Database contains:
"Alarm Clock"
```

A normal SQL keyword search may return no result.

HomeComfortHub therefore automatically falls back to **AI-powered semantic search**.

```text
Keyword Search
      ↓
0 Results
      ↓
Python AI Service
      ↓
Intent Extraction
      ↓
Embedding Generation
      ↓
Vector Similarity Search
      ↓
Closest Products
```

The Python service uses **Azure OpenAI GPT-5-mini** to extract structured search intent.

Example:

```json
{
  "category": "clock",
  "keywords": [
    "wall clock",
    "alarm clock"
  ]
}
```

The extracted keywords are converted into an embedding using:

```text
text-embedding-3-small
```

Product embeddings are retrieved from **Cosmos DB**, and the Python service calculates **cosine similarity** between the query embedding and stored product embeddings.

The closest product IDs are selected.

The Python service then calls the ASP.NET Core API to retrieve complete product information such as:

* Product name
* Description
* Price
* Image
* Amazon affiliate link
* Flipkart affiliate link

The final products are returned to Angular.

---

# 💬 AI Shopping Assistant

HomeComfortHub also provides an AI-powered chat interface.

Instead of searching using individual keywords, users can describe what they need naturally.

Example:

```text
"I need something for dry lips under ₹200."
```

The request follows:

```text
User
 ↓
Angular Chat Widget
 ↓
Python AI Service
 ↓
Azure OpenAI
 ↓
Intent Extraction
 ↓
Embedding Generation
 ↓
Cosmos DB Vector Search
 ↓
Product IDs
 ↓
.NET API
 ↓
Full Product Details
 ↓
Chat Product Cards
```

The chatbot displays relevant products directly in the conversation, including price and affiliate links.

---

# ⚡ Event-Driven Product Embedding

One of the key architectural features of HomeComfortHub is its asynchronous product synchronization.

When an administrator adds a product:

```text
Admin
 ↓
Angular
 ↓
POST /api/products
 ↓
.NET API
 ↓
Azure SQL
```

After saving the product, the API publishes an event to **Azure Service Bus**.

Example event:

```json
{
  "event": "product-created",
  "productId": 123
}
```

The API can immediately return success to the frontend without waiting for AI processing.

Meanwhile:

```text
Azure Service Bus
        ↓
Python Background Consumer
        ↓
Fetch Product from .NET API
        ↓
Generate Product Embedding
        ↓
Store Vector in Cosmos DB
```

The Python service continuously listens to the Service Bus queue.

When a `product-created` event arrives, it:

1. Retrieves the complete product from the .NET API.
2. Generates an embedding from the product name and description.
3. Stores the product's vector representation in Cosmos DB.

The new product then automatically becomes available for semantic search.

This keeps the .NET API and AI service **loosely coupled**.

---

# 📧 Search Monitoring & Notifications

HomeComfortHub also tracks unsuccessful product searches.

If traditional keyword search returns no products, the backend can send an email notification indicating what users are searching for.

Example:

```text
A user searched for "Air Purifier",
but no matching product was found.
```

Email delivery is implemented using **MailKit with Gmail SMTP**.

A **24-hour deduplication mechanism** prevents repeated notifications for the same search term.

If semantic search also fails, the Python service calls the .NET notification endpoint so unsuccessful AI searches can also be tracked.

This can help identify products users want but that are currently missing from the platform.

---

# 🛠️ Technology Stack

| Layer            | Technology              |
| ---------------- | ----------------------- |
| Frontend         | Angular                 |
| Backend          | ASP.NET Core Web API    |
| AI Service       | Python                  |
| Primary Database | Azure SQL Database      |
| AI Model         | Azure OpenAI GPT-5-mini |
| Embeddings       | text-embedding-3-small  |
| Vector Storage   | Azure Cosmos DB         |
| Messaging        | Azure Service Bus       |
| Email            | MailKit + Gmail SMTP    |
| Frontend Hosting | Azure Static Web Apps   |
| Backend Hosting  | Azure App Service       |
| Cloud Platform   | Microsoft Azure         |

---

# 📂 Project Structure

```text
HomeComfortHub/
│
├── frontend/
│   └── Angular application
│
├── backend/
│   └── ASP.NET Core Web API
│
├── ai-service/
│   └── Python AI service
│
│
├── .gitignore
└── README.md
```

---

# 🔌 Main API Endpoints

### Products

```http
GET /api/products
GET /api/products/{id}
GET /api/products/search?term={searchTerm}
POST /api/products
```

### AI Chat / Semantic Search

```http
POST /chat
```

### Search Notification

```http
POST /api/products/notify
```

---

# 🔐 Configuration & Security

Sensitive configuration is **not committed to source control**.

The application requires configuration for services such as:

```text
Azure SQL Connection String
Azure OpenAI Endpoint
Azure OpenAI API Key
Cosmos DB Connection
Azure Service Bus Connection
Gmail SMTP Credentials
```

These values should be provided through environment variables, Azure application settings, or local development configuration that is excluded using `.gitignore`.

> ⚠️ Never commit API keys, passwords, connection strings, or other secrets to the repository.

---

# 🎯 Engineering Concepts Demonstrated

HomeComfortHub was built to explore several real-world software engineering concepts beyond basic CRUD operations:

* Full-stack application development
* REST API design
* Cloud application deployment
* Semantic search
* Large Language Model integration
* Vector embeddings
* Cosine similarity
* Event-driven architecture
* Asynchronous background processing
* Message queues
* Service decoupling
* Caching
* Search analytics
* External email integration
* Distributed application architecture

---


# 📌 Summary

HomeComfortHub demonstrates how a traditional **Angular + ASP.NET Core** application can be extended with modern AI and cloud architecture.

Rather than relying only on exact keyword matching, the application combines:

**SQL Search + LLM Intent Understanding + Vector Embeddings + Semantic Similarity + Event-Driven Processing**

to create a more intelligent product discovery experience.

The project also demonstrates how **Azure Service Bus** can decouple the core application from AI processing, allowing product creation and embedding generation to happen independently and asynchronously.
