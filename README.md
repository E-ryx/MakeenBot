# 🤖 MakeenBot (ASP.NET Core Web API)

A smart Bale bot built to **streamline employee report submissions** and **automate HR report retrieval**.  

---

### 🛠️ Key Features

- 📩 Accepts structured report messages from employees via **webhooks**
- 🧹 Validates input using **regex-based validators**
- 💾 Saves valid reports in a **SQL Server** database
- 📊 Allows HR personnel to **request and receive Excel exports** directly through the bot

---

### 🧱 Architecture Highlights

- ✅ Built using a **clean, layered architecture** for maintainability and scalability
- 🔄 A dedicated **Update Controller** handles incoming webhook payloads  
- 🧠 A **Message Handler** routes actions like adding, editing, and viewing reports  
- 🧰 Task-specific services encapsulate logic for core functionalities  
- 🧪 Follows **SOLID principles** with clear separation of:
  - **Validation**
  - **Processing**
  - **Data Access**

- 🗄️ Uses **repository classes** to manage all database interactions  
- 🔧 Designed for **extensibility**—easily adaptable to new workflows or additional roles

---
