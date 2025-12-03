# 💰 PayrollPulse: A Full-Stack AI-Enhanced Payroll System Demo

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4.svg?style=for-the-badge)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
[![Nuxt](https://img.shields.io/badge/Nuxt-4.x-00DC82.svg?style=for-the-badge)](https://nuxt.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-4169E1?style=for-the-badge&logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)](https://www.docker.com/)
[![Semantic Kernel](https://img.shields.io/badge/Semantic_Kernel-blue?style=for-the-badge&logo=microsoft&logoColor=white)](https://learn.microsoft.com/en-us/semantic-kernel/overview/)

## 🌟 Project Overview

**PayrollPulse** is a modern, full-stack demonstration application built with **.NET 8** and **Nuxt 4**. It showcases a modern, scalable, and AI-enhanced approach to a business-critical payroll system. This project emphasizes clean architecture, type safety, and the integration of local Generative AI (via Ollama and Semantic Kernel) for advanced, internal business logic.

> **Note:** This repository is a **work in progress** with continuous refactorings and new features being added to demonstrate various engineering patterns.

---

## 🛠️ Technology Stack

### ⚙️ Backend: .NET 8 Minimal API

The server-side is built for performance and maintainability, adhering to robust architectural principles.

| Component            | Technology                               | Key Features                                                    |
| :------------------- | :--------------------------------------- | :-------------------------------------------------------------- |
| **Runtime**          | .NET 8 Minimal API                       | High-performance, lightweight API endpoints.                    |
| **AI Integration**   | Ollama with `phi3:mini`                  | Self-hosted LLM runtime for internal AI processing.             |
| **AI Orchestration** | Semantic Kernel                          | SDK for AI function creation and prompt engineering.            |
| **Data Access**      | EF Core 8 / Postgres                     | Robust ORM and reliable relational database.                    |
| **Architecture**     | Feature Slice & Pragmatic DDD            | Focus on feature isolation and domain clarity.                  |
| **Ecosystem**        | Docker, Authentication, FluentValidation | Containerization, security, and elegant server-side validation. |

### 🖥️ Frontend: Nuxt 4

A modern, reactive, and type-safe client built on Vue.js.

| Component     | Technology       | Key Features                                               |
| :------------ | :--------------- | :--------------------------------------------------------- |
| **Framework** | Nuxt 4           | Intuitive full-stack Vue framework (SSR capabilities).     |
| **UI/UX**     | NuxtUI 4         | Beautiful, accessible, and customizable component library. |
| **Language**  | Typescript / Zod | Ensures type-safe code and robust client-side validation.  |
| **Features**  | Charts, Auth     | Data visualization and secure session management.          |

---

## 🚀 Getting Started

To run this project, you must have **Docker** and **Docker Compose** installed on your machine.

### Complete Setup Instructions

The application requires the `phi3:mini` model to be pulled into the Ollama container _before_ the main services attempt to connect to it.

1.  **Clone the Repository:**

    ```bash
    git clone https://github.com/enriquemartinez-emc/PayrollPulse.git
    cd PayrollPulse
    ```

2.  **Pull the AI Model (`phi3:mini`):**
    First, enter the `ollama` container defined in `docker-compose.yml`:

    ```bash
    docker compose run payroll_ollama bash
    ```

    Next, execute the command inside the container to download the model:

    ```bash
    ollama pull phi3:mini
    ```

    Once the pull is complete, exit the container:

    ```bash
    exit
    ```

3.  **Run the Full Project Stack:**
    This command will build and start all required services: Frontend, Backend API, Postgres Database, and the Ollama runtime.
    ```bash
    docker compose up --build
    ```
    - **Frontend UI:** `http://localhost:3000`
    - **Backend API (Swagger):** `http://localhost:8080`

---

## 🔒 Default Credentials

The database is pre-seeded with initial users for immediate testing and exploration.

| Role                 | User Email                 | Password     |
| :------------------- | :------------------------- | :----------- |
| **Administrator**    | `admin@company.com`        | `Admin1234!` |
| **Employee Example** | `bob.williams@company.com` | `Bob1234!`   |
