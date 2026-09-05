## 📁 Project Structure Overview

This Angular application follows a clean and scalable folder structure, designed around best practices with **feature-based organization**, **standalone components**, and **clear separation of concerns**.

---

### 🧠 `core/` – Application-wide logic

Contains singleton services and global concerns that are not tied to any single feature.

| Folder          | Description                                                                                    |
| --------------- | ---------------------------------------------------------------------------------------------- |
| `guards/`       | Route guards (e.g. `AuthGuard`, `RoleGuard`) for navigation control.                           |
| `interceptors/` | Global HTTP interceptors (e.g. auth tokens, error handling).                                   |
| `models/`       | Global interfaces and types shared across the app.                                             |
| `services/`     | Singleton services used throughout the app, such as authentication, configuration, or logging. |

> Core logic should be globally available and never depend on feature-specific code.

---

### 🚀 `features/` – Business functionality

Each feature lives in its own folder with everything it needs: pages, components, services, models.

Example: `features/someFeature/`

| Folder        | Description                                                      |
| ------------- | ---------------------------------------------------------------- |
| `pages/`      | Routable components used in the router (e.g. `SomeFeaturePage`). |
| `components/` | UI components scoped to this feature, not reused globally.       |
| `services/`   | Feature-specific services (API calls, logic, state management).  |
| `models/`     | Interfaces and types related only to this feature.               |

> Feature folders are **lazy-loadable** via `loadChildren` in routing.

---

### 🧱 `layout/` – Application shell

Handles visual structure and shared layout between pages (e.g. header, footer, sidebar).

| Folder        | Description                                                                                  |
| ------------- | -------------------------------------------------------------------------------------------- |
| `components/` | Layout-specific elements like `NavbarComponent`, `FooterComponent`.                          |
| `shell/`      | Layout wrappers such as `MainLayout`, which include `<router-outlet>` and structure the app. |

> Layout components are used as route containers in `app.routes.ts`.

---

### 🧩 `shared/` – Reusable components and utilities

Generic, reusable building blocks used across multiple features.

| Folder        | Description                                                    |
| ------------- | -------------------------------------------------------------- |
| `components/` | Reusable UI (e.g. buttons, inputs, cards).                     |
| `directives/` | Attribute or structural directives used across templates.      |
| `pipes/`      | Custom pipes for formatting or transforming data in templates. |

> Shared code is feature-agnostic and must not depend on feature-specific logic.

---

### 📌 Root Files

| File            | Purpose                                                                  |
| --------------- | ------------------------------------------------------------------------ |
| `app.routes.ts` | Main routing configuration with layout routing and lazy-loaded features. |
| `app.config.ts` | Runtime configuration or global tokens.                                  |
| `main.ts`       | App bootstrap file.                                                      |
| `styles.css`    | Global styles including Tailwind setup and design tokens.                |

---

## ✅ Summary

This structure provides:

- Modular, testable and maintainable code.
- Feature isolation and clear responsibility boundaries.
- Built-in support for lazy loading and standalone components.
- Clean separation between global logic (`core`), reusable pieces (`shared`), and business domains (`features`).

### 🔍 Code Quality

This project uses **ESLint** with **ESLintPrettier** integration for linting and formatting.  
To ensure a consistent developer experience:

- Install the ESLint and Prettier ESLint extensions in your VS Code.
- Default Formatter should be Prettier ESLint
- Check Format on save.

---
