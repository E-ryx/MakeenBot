# MakeenBot (ASP.NET Core Web API)
#### Developed a Bale bot to streamline employees report submissions and **automate** HR report retrieval.

- The bot accepts structured messages from employees via webhook,
validates input using **regex-based message validators**, and **stores**
valid reports in a **SQL Server database.** HR personnel can request
and receive **Excel exports** of stored reports directly through the bot.
- Implemented a **clean, layered architecture** with a dedicated update
controller (for handling webhook payloads), a message handler (to
route actions like adding/editing/viewing reports), and use-casespecific services. Followed **SOLID principles** by separating concerns
into **validation, processing,** and **data access layers,** with **repository
classes** managing database operations. Focused on **maintainability**
and **extensibility**, enabling **easy adaptation** for future workflows or
additional roles.
