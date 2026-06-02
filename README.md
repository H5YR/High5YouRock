# High 5, You Rock! 🙌

This README is a work in progress, if anything is unclear or you have suggestions for improvement, please open an issue or submit a pull request!

A community recognition platform built with **Umbraco 17** on **.NET 10**, featuring Mastodon social integration, a widget system, and a Vite-powered frontend.

LIVE site: https://h5yr.com
---

## Prerequisites

Before getting started, make sure you have the following installed:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js](https://nodejs.org/) (LTS recommended) and npm
- [Visual Studio 2026](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- A supported database (SQL Server LocalDB is configured by default)
- Git

---

## Getting Started

### 1. Fork & Clone the Repository

1. Navigate to [https://github.com/H5YR/High5YouRock](https://github.com/H5YR/High5YouRock) on GitHub.
2. Click the **Fork** button (top-right) to create your own copy of the repository.
3. Clone your fork locally:

```bash
git clone https://github.com/<your-username>/High5YouRock.git
cd High5YouRock
```

4. Add the upstream remote so you can pull in future changes:

```bash
git remote add upstream https://github.com/H5YR/High5YouRock.git
```

---

### 2. Install Frontend Dependencies

Navigate to the frontend directory and install npm packages:

```bash
cd H5YR/frontend
npm install
```

---

### 3. Configure the Application

Copy or review `H5YR/appsettings.json` and ensure your connection string and any required API settings (Mastodon, widget API keys etc.) are configured. For local overrides, use `appsettings.Development.json` which is excluded from source control.

Key settings to review:

| Section | Description |
|---|---|
| `ConnectionStrings` | Database connection string |
| `Umbraco:CMS` | Umbraco core configuration |
| `uSync` | uSync import/export settings |
| `APISettings` | Widget API configuration |
| `TwitterSettings` | Twitter/X integration settings |

---

### 4. Build & Run

#### Option A — Visual Studio

1. Open `High5YouRock.sln` in Visual Studio 2026.
2. Set **H5YR** as the startup project.
3. Press **F5** to build and run.

Visual Studio will launch the Vite dev server automatically via `Vite.AspNetCore`.

#### Option B — CLI

In one terminal, start the Vite dev server:

```bash
cd H5YR/frontend
npm run dev
```

In a second terminal, run the .NET application:

```bash
dotnet run --project H5YR/H5YR.csproj
```

---

### 5. First-Time Umbraco Setup

On first run, Umbraco will present the **installer wizard** at `https://localhost:<port>/umbraco/install`. Complete the installer to create your admin account and initialise the database.

Once the installer is complete, proceed to the uSync import below.

---

### 6. Import Content with uSync

This project uses [uSync](https://jumoo.co.uk/usync/) to keep Document Types, Data Types, Templates, and Content in sync via source control. All uSync configuration files are stored in `H5YR/uSync/v17/`.

To import everything into your local Umbraco instance:

1. Log in to the **Umbraco backoffice** at `/umbraco`.
2. Navigate to **Settings → uSync**.
3. Click **Import All** to import all Document Types, Data Types, Templates, Members, Media Types, and Content from the `uSync/v17` folder.
4. Review the import report and confirm there are no errors.

> **Tip:** If you add or modify Document Types, Data Types, or Content locally and want to share those changes, use **Export All** in uSync and commit the updated `.config` files in `uSync/v17/` to your branch.

---

## Contributing

We welcome contributions! Please follow the steps below to keep things consistent.

### Branching Strategy

All active development targets the `u17/develop` branch. **Do not open pull requests directly against `main`.**

### Contribution Workflow

1. Make sure your fork is up to date with upstream:

```bash
git fetch upstream
git checkout u17/develop
git merge upstream/u17/develop
```

2. Create a new feature or fix branch from `u17/develop`:

```bash
git checkout -b feature/your-feature-name
```

3. Make your changes, following the existing code style and conventions.

4. Build the project and verify there are no errors:

```bash
dotnet build
```

5. Commit your changes with a clear, descriptive message:

```bash
git add .
git commit -m "feat: describe your change here"
```

6. Push your branch to your fork:

```bash
git push origin feature/your-feature-name
```

7. Open a **Pull Request** on GitHub from your fork's branch targeting **`H5YR/High5YouRock:u17/develop`**.

### Pull Request Guidelines

- Keep PRs focused — one feature or fix per PR.
- Include a clear description of what changed and why.
- If your change affects Umbraco structure (Document Types, Data Types, etc.), export uSync config and include the updated files in your PR.
- If you add new npm packages, commit the updated `package.json` and `package-lock.json`.

---

## Project Structure

```
High5YouRock/
├── H5YR/                        # Umbraco web project
│   ├── frontend/                # Vite frontend (SCSS, JS)
│   │   ├── package.json
│   │   └── vite.config.js
│   ├── uSync/v17/               # uSync configuration files
│   ├── wwwroot/                 # Static assets (compiled frontend output)
│   ├── appsettings.json
│   └── Program.cs
└── H5YR.Core/                   # Class library (services, models, controllers)
    ├── Controllers/             # Surface & API controllers
    ├── Models/                  # Generated & view models
    ├── Services/                # Business logic services
    ├── Data/                    # Database entities, stores & migrations
    ├── Composers/               # Umbraco composers for DI registration
    └── ViewComponents/          # Razor view components
```

---

## Useful Links

- [Umbraco Documentation](https://docs.umbraco.com/)
- [uSync Documentation](https://jumoo.co.uk/usync/docs/)
- [Vite Documentation](https://vitejs.dev/)
- [Skybrud Mastodon](https://github.com/abjerner/Skybrud.Social.Mastodon)
