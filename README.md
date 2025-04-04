# Kumadio

**Kumadio** is an interactive audio books platform built with **.NET** (Clean Architecture), **Angular** (standalone components), and **Ionic** for mobile integration. The platform enables branching audio storytelling, advanced creator tools, and a smooth user experience across web and mobile devices.

<div align="center">
  <img src="https://media.licdn.com/dms/image/v2/D4E0BAQHrhPezbdG85A/company-logo_200_200/company-logo_200_200/0/1730828228363/kumadio_logo?e=1749081600&v=beta&t=c4dRSnD6-tORESnJgNuRdFZUEVE1j7dhc3k9kVD2JA0" alt="Kumadio Logo" width="200">
</div>

---

## Table of Contents

1. [Overview](#overview)
2. [Features](#features)
3. [Tech Stack](#tech-stack)
4. [Architecture](#architecture)
5. [Project Structure](#project-structure)
6. [Installation](#installation)
7. [Usage](#usage)
8. [Demo](#demo)
9. [Onboarding Flow](#onboarding-flow)
10. [Contributing](#contributing)
11. [License](#license)

---

## Overview

**Kumadio** bridges the gap between **interactive audio books** and modern cross-platform usage. It offers:

- **Listeners** a branching, choice-based audio experience.
- **Creators** tools to build multi-path audio stories.
- A frictionless login, signup, and **onboarding** flow that personalizes content for children based on age, interests, and learning goals.

---

## Features

- **Creator Tools**
  - Upload and manage branching audio stories.
  - Real-time previews to test story paths.

- **Search & Discovery**
  - Full-text search on titles, authors, and categories.
  - "Discover" view showcasing new or featured content.

- **User Personalization**
  - Collect child's age, gender, and interests during onboarding.
  - Tailored recommendations based on preferences.

- **Cross-Platform**
  - Web application via Angular.
  - Mobile apps for iOS and Android using Ionic Capacitor.

---

## Tech Stack

**Frontend**
- Angular 17+ (Standalone components, TypeScript)
- Ionic (Mobile deployment)
- RxJS (Reactive flows)
- SCSS (Styling)

**Backend**
- .NET 8 (Clean Architecture)
- Entity Framework Core (Data access)
- JWT Authentication
- SQL Server

**Infrastructure**
- Docker
- Git

---

## Architecture

### Backend: Clean Architecture

- **Core**: Domain logic, models, interfaces.
- **Infrastructure**: Database context, repositories, external services.
- **Web**: Controllers, authentication, middleware.

Structure:
```
Kumadio.Core          # Domain logic, interfaces
Kumadio.Domain        # Entities, enums
Kumadio.Infrastructure
  └── DbContext, repositories, services
Kumadio.Web           # API endpoints, controllers, auth
```

### Frontend: Standalone Components

- Modular standalone components, no NgModule.
- Each component imports own dependencies.
- Ionic integrated at component level for mobile.

---

## Project Structure

```
kumadio/
├── Backend (.NET)
│   ├── Kumadio.Core
│   ├── Kumadio.Domain
│   ├── Kumadio.Infrastructure
│   └── Kumadio.Web
└── Frontend (Angular/Ionic)
    ├── src/
    │   ├── app/
    │   │   ├── home/
    │   │   ├── onboarding/
    │   │   └── ...
    ├── package.json
    └── ionic.config.json
```

---

## Installation

**Prerequisites**
- .NET 8 SDK
- Node.js (v16+) & npm
- Angular 17
- Ionic CLI
- SQL Server

**Steps**

1. **Clone repository**
   ```bash
   git clone https://github.com/marceta99/AIAudioTales.git
   ```

2. **Backend**
   ```bash
   dotnet restore
   dotnet build
   dotnet ef database update
   ```

3. **Frontend**
   ```bash
   cd AngularIonic
   npm install
   ionic serve
   ```

Access at `http://localhost:8100`

---

## Usage

1. **Run Backend**
   ```bash
   dotnet run --project Kumadio.Web
   ```
   Available at `http://localhost:5000`

2. **Run Frontend**
   ```bash
   cd AngularIonic
   ionic serve
   ```
   Available at `http://localhost:8100`

3. **Mobile Apps**
   ```bash
   ionic capacitor run android
   ```
   or
   ```bash
   ionic capacitor run ios
   ```

---

## Demo

Check out our demo video on YouTube:

[![Kumadio Demo](https://img.youtube.com/vi/VIDEO_ID/0.jpg)](https://www.youtube.com/watch?v=VIDEO_ID)

*Click thumbnail to watch!*

*(Replace `VIDEO_ID` with your actual YouTube video ID.)*

---

## Onboarding Flow

**Kumadio** uses a multi-step onboarding process:

- Child’s **age** (4–10)
- **Gender** selection (Muški, Ženski, Ne želim da navedem)
- **Interests** (Adventure, Animals, etc.)
- Learning & development preferences

Implemented using Angular Reactive Forms and a custom slider interface.

---

## Contributing

We welcome contributions!

1. Fork repository
2. Create feature branch
3. Commit clearly
4. Submit Pull Request

For major changes, [open an issue](../../issues) first.

---

## License

Licensed under the MIT License. See [LICENSE](LICENSE).

**Enjoy Kumadio** – A new horizon for interactive storytelling!

