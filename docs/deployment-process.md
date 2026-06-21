# Deployment Process

## Overview

This project uses GitHub Actions to automate validation and publishing for an ASP.NET Core portfolio web application. The deployment approach is designed to mimic a typical organizational release process with separate Test and Production environments.

The application is built and tested automatically before it is considered ready for deployment.

## Branching Strategy

The repository uses three branch types:

- `main`: production-ready code
- `develop`: integration and test-ready code
- `feature/*`: individual changes or tasks

The normal flow is:

1. Create a feature branch from `develop`.
2. Make changes on the feature branch.
3. Open a pull request into `develop`.
4. GitHub Actions runs CI checks.
5. If checks pass, merge into `develop`.
6. When ready for release, open a pull request from `develop` into `main`.
7. After CI passes again, merge into `main` to represent a production release.

## CI Pipeline

The CI workflow runs using GitHub Actions on a Windows runner.

The pipeline performs these steps:

1. Checkout repository
2. Setup .NET 8
3. Restore dependencies
4. Build the application in Release mode
5. Run automated tests
6. Publish the application
7. Upload the published output as an artifact

This ensures that every change is validated before it is merged or deployed.

## Environment Configuration

The application uses separate configuration files for different environments:

- `appsettings.Development.json`
- `appsettings.Test.json`
- `appsettings.Production.json`

The Test and Production configurations use separate database names so each environment can be isolated.

Sensitive values such as SMTP passwords and Google OAuth secrets are not stored in source control. For local development, these values are stored using .NET User Secrets. In a real deployment, they would be supplied through GitHub Secrets, environment variables, or server-level configuration.

## Local Deployment Simulation

The deployment process is simulated locally using separate Windows folders:

- `C:\Deployments\MyPortfolioWebsite-Test`
- `
