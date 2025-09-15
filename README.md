# FuelTrack-Malaysia API

## Project Overview

This repository contains the source code for the **FuelTrack-Malaysia** Web API, developed as part of the SWC3633/SWC4443 course.

## Pair Assignment

- **MUHAMMAD ZULHUSNI BIN ROSDAN** | ID: AM2405015965
- **YASIERUL FIRDAUS BIN AZIZAN** | ID: AM2405015974

---

## Local Setup and Execution Guide

### Prerequisites

To successfully build and run this project, ensure you have the following dependencies installed on your system:

- **_.NET SDK 9.0_** or a later version. The SDK is required to build, run, and publish .NET applications.
- **_Git_**, for cloning the project repository.
- A compatible development environment, such as **_Visual Studio_** or **_Visual Studio Code_** with the C# extension.

### 1. Repository Cloning

Begin by cloning the project from the GitHub repository to your local machine using the following command in your terminal:

```bash
git clone https://github.com/MuhammadZulhusni/FuelTrack-Malaysia.git
```

### 2. Navigate to the Project Directory

Change your working directory to the newly cloned project folder:

```bash
cd FuelTrack-Malaysia
```

### 3. Restore NuGet Packages

Before building, it is essential to restore the project's dependencies. This command will download all required NuGet packages specified in the project file:

```bash
dotnet restore
```

### 4. Build the Project

Build the solution to compile the source code and check for any syntax errors. A successful build is required before running the application.

```bash
dotnet build
```

### 5. Run the Application

You can execute the application directly from the command line or via an IDE.

```bash
dotnet run
```

This command will launch the Kestrel web server, and the API will be accessible at the URL displayed in the console (e.g., https://localhost:7001).
