# Solution Design

## Objective

The objective of this solution is to implement a simple CI/CD process for a Windows-hosted ASP.NET Core application. The process automates build, test, publish, and deployment preparation while supporting separate Test and Production environments.

## Application

The selected application is an ASP.NET Core Razor Pages portfolio web application. It includes server-side rendering, authentication configuration, project/profile management, and environment-specific configuration files.

## Branching Model

The repository uses a simple GitFlow-inspired branching model:

- `feature/*`: used for individual changes
- `develop`: integration branch and Test environment source
- `main`: production-ready branch

Developers create feature branches from `develop`. Changes are merged back into `develop` through pull requests after GitHub Actions validation. When a release is ready, `develop` is promoted to `main` using another pull request.

## CI/CD Flow

The GitHub Actions workflow runs on pushes and pull requests to `main` and `develop`.

The workflow performs:

1. Checkout repository
2. Setup .NET 8
3. Restore dependencies
4. Build in Release mode
5. Run automated tests
6. Publish the application
7. Upload the published output as an artifact

## Environment Strategy

The application supports separate configuration files:

- `appsettings.Development.json`
- `appsettings.Test.json`
- `appsettings.Production.json`

Test and Production use separate database names and deployment folders. Secrets such as SMTP passwords and Google OAuth credentials are not stored in source control. They are supplied using User Secrets locally and would be supplied through GitHub Secrets or environment variables in a real deployment.

## Deployment Strategy

The deployment process is simulated on a local Windows machine using separate folders:

- `C:\Deployments\MyPortfolioWebsite-Test`
- `C:\Deployments\MyPortfolioWebsite-Production`

The script `scripts/deploy-local.ps1` publishes the application and copies the output into the correct deployment folder based on the selected environment.

The intended mapping is:

- `develop` -> Test
- `main` -> Production

## Flowchart

```mermaid
flowchart TD
    A["Developer creates feature branch"] --> B["Code changes committed"]
    B --> C["Pull request into develop"]
    C --> D["GitHub Actions CI runs"]
    D --> E{"Build and tests pass?"}
    E -- "No" --> F["Fix issues and push updates"]
    F --> D
    E -- "Yes" --> G["Merge into develop"]
    G --> H["Deploy to Test environment"]
    H --> I["Release pull request from develop to main"]
    I --> J["GitHub Actions CI runs again"]
    J --> K{"Release validation passes?"}
    K -- "No" --> L["Fix release issues"]
    L --> J
    K -- "Yes" --> M["Merge into main"]
    M --> N["Deploy to Production environment"]
