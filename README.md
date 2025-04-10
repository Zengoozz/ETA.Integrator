# ETA.Integrator

A modern integration platform for authentication and integration between ETA and ERPs .

## Table of Contents
- [Tech Stack](#tech-stack-overview)
- [Server-Side Documentation](#server-side-documentation)
- [Client-Side Documentation](#client-side-documentation)
- [Installation](#installation)
- [Project Structure](#project-structure)

---

## Tech Stack Overview

### 🌐 **Client Side**

<table>
  <tr>
    <td align="center"><img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/react/react-original.svg" width="40" alt="V18.2.0" /><br/>React</td>
    <td align="center"><img src="https://vitejs.dev/logo.svg" width="40" alt="V4.4.5" /><br/>Vite</td>
    <td align="center"><img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/javascript/javascript-original.svg" width="40" alt="JavaScript" /><br/>JavaScript</td>
    <td align="center"><img src="https://avatars.githubusercontent.com/u/12101536?s=200&v=4" width="40" alt="V5.8.1" /><br/>Ant Design</td>
    <td align="center"><img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/vscode/vscode-original.svg" width="40" alt="VSCode" /><br/>VS Code</td>
  </tr>
  <tr>
    <td align="center"><img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/dot-net/dot-net-original.svg" width="40" alt="V8.0" /><br/>ASP.NET Core</td>
    <td align="center"><img src="https://restsharp.dev/img/restsharp.png" width="40" alt="V112.1.0" /><br/>RestSharp</td>
    <td align="center"><img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/git/git-original.svg" width="40" alt="Git" /><br/>Git</td>
    <td align="center"><img src="https://seeklogo.com/images/P/postman-logo-0087CA0D15-seeklogo.com.png" width="40" alt="Postman" /><br/>Postman</td>
    <td align="center"><img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/visualstudio/visualstudio-plain.svg" width="40" alt="Visual Studio" /><br/>Visual Studio</td>
  </tr>
</table>

---

## Server-Side Documentation

### API Endpoints

#### Authentication

| Endpoint                  | Method | Description              | Requires Auth |
|---------------------------|--------|--------------------------|---------------|
| `/HMS/Login`              | POST   | User login               | No            |
| `/HMS/Login/Settings`     | GET    | User Settings            | No            |
| `/HMS/Login/SaveSettings` | POST   | User Settings Save       | No            |
| `/HMS/Token/Connect`      | GET    | Connect to ETA           | Yes           |

#### Invoices

| Endpoint                  | Method | Description             | Requires Auth |
|---------------------------|--------|-------------------------|---------------|
| `/HMS/Invoices`           | GET    | List all invoices       | Yes           |
| `/HMS/Invoices/Submit`    | POST    | Submit selected invoices| Yes           |

> **Note:** All POST/PUT endpoints expect JSON payloads.

---


## Client-Side Documentation

The React client provides an interactive user interface for managing and displaying invoices.

### Key Features

- **Login Form:** Handles user authentication and redirects based on the user's session state.
- **Invoices Table:** Displays a paginated list of invoices with filtering options.
- **Settings:** Allows users to modify configuration settings, such as client credentials.
- **Dark/Light Mode:** Theme switcher based on the user's system preferences or manual selection.
- **Responsive UI:** Uses Ant Design components for a modern and responsive layout.

---

## Installation
```bash
# Clone repository
git clone https://github.com/Zengoozz/ETA.Integrator.git
```
### **Client Application**
```bash
cd ETA.Integrator/client

# Install dependencies
npm install

# Run development server
npm run dev
```
### **Server Application**
```bash
cd ETA.Integrator/server

# Restore packages
dotnet restore

# Run the application
dotnet run
```

---

## Project Structure
```bash
ETA.Integrator/
- ClientApp/           # React frontend
    - src/
        - Assets/      # Static files (styles and images)
        - Components/  # Reusable UI components
        - Constants/   # Static values and configuration constants
        - Hooks/       # Custom React hooks 
        - Pages/       # Main page components (Routes)
        - Routes/      # Navigator between pages
        - Services/    # Functions for API and data

- Server/              # ASP.NET Core backend
    - Controllers/     # API controller classes
    - Interface/       # Interface definitions
    - Models/          # Date transfer objects
    - Services/        # Business logic services
```

---

