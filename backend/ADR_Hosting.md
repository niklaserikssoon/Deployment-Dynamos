# ADR 1: Hosting Platform and Infrastructure Strategy for Azure Deployment

## Status
Approved

## Context
Our team needs to deploy a full-stack containerized application consisting of a .NET backend web API, a frontend client web app, and a relational database. The solution must be production-ready, meaning it requires continuous deployment via CI/CD, secure secret management, production monitoring, and scalable hosting with the lowest possible administrative overhead. 

We evaluated three potential architectural approaches in Azure:
1. **Azure Virtual Machines (IaaS):** High control, but high maintenance overhead (OS patching, manual scaling, firewall configurations).
2. **Azure Kubernetes Service (AKS - Coordinated PaaS):** Extremely powerful, but introduces high complexity in cluster management, network policies, and a steep learning curve for a small team project.
3. **Azure App Services / Container Apps (PaaS):** Fully managed, native support for Docker containers, automated scaling, seamless integration with GitHub Actions, and minimal operational maintenance.

## Decision
We decided to use **Azure App Service (Web App for Containers)** for hosting both the frontend and backend services, paired with **Azure SQL Database** as a fully managed relational database.

## Consequences & Motivations
Our choice is driven by the following critical architectural and educational factors:

### 1. Minimizing Operational Overhead (PaaS vs. IaaS)
By opting for Azure App Service (a Platform-as-a-Service model), Azure manages the underlying operating system, runtime patching, hardware provision, and base infrastructure security. This allows our team to focus entirely on application logic and automated delivery pipelines instead of infrastructure maintenance, complying with core **DevOps principles**.

### 2. Multi-Stage Docker & Non-Root Containers Optimization
To ensure cloud efficiency and security, our `.NET` application utilizes an optimized **Multi-Stage Dockerfile**. 
* **The Build Stage** utilizes the full .NET SDK to compile the code and run tests.
* **The Final Production Stage** copies *only* the compiled binaries into a minimal ASP.NET Core Runtime base image, radically reducing image size, storage costs in **Azure Container Registry (ACR)**, and deployment times.
* **Non-Root Execution:** The container is explicitly configured to run under a non-root user account (`USER app`). In the event of an application-layer vulnerability, this heavily mitigates the risk, preventing an attacker from gaining root-level control over the underlying host file system.

### 3. Integrated Security with Managed Identities (Least Privilege)
Azure App Service provides native support for **System-Assigned Managed Identities** (Microsoft Entra ID). This eliminates the need to distribute or store high-privilege credentials/passwords for communication between Azure components. The App Service identity is granted exact, read-only permissions (**Key Vault Secrets User** role) using Azure Role-Based Access Control (**RBAC**), entirely adhering to the **Principle of Least Privilege**.

### 4. Seamless CI/CD & Production Monitoring Integration
Azure App Services integrates natively with GitHub Actions via deployment slots or publishing profiles. Furthermore, it allows for trivial, zero-code integration with **Azure Application Insights**, automatically providing us with comprehensive observability (latencies, HTTP error rates, and cross-component dependency tracking) straight out of the box.