
---

```markdown
# Kumadio

**Kumadio** is an interactive audio books platform built with **.NET** (Clean Architecture) and **Angular** (standalone components) plus **Ionic** for mobile integration. The platform enables branching audio storytelling, advanced creator tools, and a smooth user experience across web and mobile devices.

<div align="center">
  <img src="https://media.licdn.com/dms/image/v2/D4E0BAQHrhPezbdG85A/company-logo_200_200/company-logo_200_200/0/1730828228363/kumadio_logo?e=1749081600&v=beta&t=c4dRSnD6-tORESnJgNuRdFZUEVE1j7dhc3k9kVD2JA0" alt="Kumadio Logo" width="200">
</div>

## Table of Contents

1. [Overview](#overview)  
2. [Features](#features)  
3. [Tech Stack](#tech-stack)  
4. [Architecture](#architecture)  
   - [Backend: Clean Architecture](#backend-clean-architecture)  
   - [Frontend: Standalone Components](#frontend-standalone-components)  
5. [Project Structure](#project-structure)  
6. [Installation](#installation)  
7. [Usage](#usage)  
8. [Demo](#demo)  
9. [Onboarding Flow](#onboarding-flow)  
10. [Contributing](#contributing)  
11. [License](#license)

---

## Overview

**Kumadio** bridges the gap between **interactive audio books** and modern day cross-platform usage.  
It allows:

- **Listeners** to browse and play branching audio stories.  
- **Creators** to upload and build multi-path audio trees.  
- A frictionless login, signup, and **onboarding** flow that personalizes content for children based on age, interests, and learning preferences.

---

## Features

- **Creator Tools**:  
  - Upload MP3 or other audio files.  
  - Build a “choose your own adventure” style branching tree.  
  - Real-time preview of the audio flow.  
- **Search & Discovery**:  
  - Full-text search over titles, categories, or authors.  
  - “Discover” view for new audio books.  
- **User Personalization**:  
  - Onboarding form to capture child’s interests, age, gender.  
  - Tailored recommendations.  
- **Cross-Platform**:  
  - Web usage via Angular.  
  - Mobile builds via Ionic Capacitor for iOS/Android.  

---

## Tech Stack

**Frontend**  
- **Angular** 17+ (Standalone components, TypeScript)  
- **Ionic** (for mobile deployment)  
- **RxJS** for reactive flows  
- **SCSS** for styling  

**Backend**  
- **.NET** 8 with **Clean Architecture**  
- **Entity Framework Core** for data access  
- **JWT** + refresh token authentication  
- **SQL Server**

**Infrastructure**  
- **Docker** (optional for containerizing)  
- **Git** for version control  

---

## Architecture

### Backend: Clean Architecture

The **.NET backend** follows a **Clean Architecture** approach, ensuring:

- **Separation of Concerns**: 
  - **Core** (domain models, logic)  
  - **Infrastructure** (EFCore, data persistence, external services)  
  - **Web** (controllers, endpoints, authentication logic)  
- **Dependency Inversion**: The domain layer does not depend on external layers. Instead, interfaces in the core are implemented in infrastructure.  
- **Testability**: Each layer can be tested in isolation, making maintenance easier.

A typical structure:

```
/Kumadio.Core
    - Entities
    - Interfaces
    - Domain Services
/Kumadio.Infrastructure
    - EF DbContext
    - Repository Implementations
    - External Integrations
/Kumadio.Domain
    - Entities
    - Enums
/Kumadio.Web
    - Controllers
    - Auth / Startup / Middlewares
    - DTOs & Mappers
```

### Frontend: Standalone Components

The **Angular** front-end is built entirely with **standalone components**—a feature introduced in Angular 14—meaning:

- **No traditional `NgModule`** files.  
- Each component imports its own dependencies.  
- This leads to a more modular, tree-shakable architecture.  
- The **Ionic** integration is done with references to needed Ionic modules at the component or route level.

Such an approach improves clarity, reduces overhead, and is future-friendly in Angular’s ecosystem.

---

## Project Structure

```
kumadio/
│
├── .NET (Backend)
│   ├── Kumadio.Core         # Domain services
│   ├── Kumadio.Domain         # Domain entities
│   ├── Kumadio.Infrastructure
│   │   └── Data, EF repos    # Repositories and db communication
│   ├── Kumadio.Web          # Controllers, Auth
│   └── ...
│
├── AngularIonic (Frontend)
│   ├── src/
│   │   ├── app/
│   │   │   ├── home/
│   │   │   │   ├── components/
│   │   │   │   └── services/
│   │   │   ├── onboarding/
│   │   │   └── ...
│   ├── package.json
│   ├── ionic.config.json
│   └── ...
│
├── README.md
└── ...
```

---

## Installation

**Prerequisites**:
- .NET 8 SDK  
- Node.js (v16+) & npm  
- Angular 17
- Ionic CLI (if building mobile)  
- SQL Server or a supported DB for EF

### Steps

1. **Clone** the repository:
   ```bash
   git clone https://github.com/marceta99/AIAudioTales.git
   ```
2. **Backend**:
   - `cd` into `.NET` solution folder (e.g. `Kumadio.Web.sln`).
   - `dotnet restore` & `dotnet build`.
   - Configure `appsettings.json` (DB & JWT secrets).
   - (Optional) `dotnet ef database update` for migrations.
3. **Frontend**:
   - `cd AngularIonic`
   - `npm install`
   - `ionic serve` or `npm run start` for local dev.

---

## Usage

1. **Run the .NET API**:
   ```bash
   dotnet run --project Kumadio.Web
   ```
   The API typically runs at `http://localhost:5000`.

2. **Run Angular**:
   ```bash
   cd AngularIonic
   npm run start
   ```
   or
   ```bash
   ionic serve
   ```
   Access it at `http://localhost:8100`.

3. **Mobile**:
   ```bash
   ionic capacitor run android
   ```
   or
   ```bash
   ionic capacitor run ios
   ```
   This will launch the mobile build in an emulator/real device.

---

## Demo

You can view a **live slideshow** or short **video** demonstrating Kumadio:

1. [Video Demo on YouTube](https://www.youtube.com/channel/UC_cu6RoNHrYh7tCGiblSvfA)  

<div align="center">
  <img src="https://example.com/video-thumb.jpg" width="200" alt="Video thumbnail">
</div>

---

## Onboarding Flow

**Kumadio** includes a **multi-slide** onboarding flow using custom pointer events or Swiper to:

- Capture **child’s age** (4–10).  
- **Gender** (Muški, Ženski, Ne želim da navedem).  
- **Interests** (adventure, animals, science, etc.).  
- **Learning & Development** areas.  

These answers tailor the recommended audio books to best suit each child. The onboarding is built with:

- Angular **reactive forms** for typed inputs/checkboxes.  
- A custom “horizontal slider” logic for a slick user experience.  
- A final submission to `.NET` saving user preferences.

---

## Contributing

We appreciate all contributions. If you’d like to help:

1. **Fork** this repository.  
2. Create a **branch** for your feature or fix.  
3. Make your changes with clear commit messages.  
4. **Push** and open a **Pull Request**.  

Please open an **issue** first to discuss major changes.

---

**Enjoy building & exploring Kumadio!** 
A new horizon for interactive storytelling. 
