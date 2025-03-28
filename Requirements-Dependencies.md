# Dependencies and Requirements for Temperature Monitoring System

Here's a comprehensive list of everything you need to run the application:

## Development Environment
- **IDE**: JetBrains Rider 2024
- **.NET Version**: .NET 9.0
- **Project Type**: Console Application

## Required NuGet Packages

### Core Functionality
- **Akka.NET** (v1.5.0 or later)
  - For Actor Model pattern implementation
  - Install command: `dotnet add package Akka`

- **System.Threading.Tasks.Dataflow** (v7.0.0 or later)
  - For Producer-Consumer pattern
  - Install command: `dotnet add package System.Threading.Tasks.Dataflow`

- **System.Text.Json** (v7.0.0 or later)
  - For JSON serialization in Bridge pattern
  - Install command: `dotnet add package System.Text.Json`

### Testing
- **MSTest.TestFramework** (v3.0.0 or later)
  - For unit testing
  - Install command: `dotnet add package MSTest.TestFramework`

- **MSTest.TestAdapter** (v3.0.0 or later)
  - For running tests in IDE
  - Install command: `dotnet add package MSTest.TestAdapter`

- **Microsoft.NET.Test.Sdk** (latest version)
  - For running tests
  - Install command: `dotnet add package Microsoft.NET.Test.Sdk`

## File Requirements
- **sensor_data.txt** - Must be located in the output directory
  - Path: `bin/Debug/net9.0/sensor_data.txt`
  - Contains the sample sensor data to be processed

## System Requirements
- Any operating system that supports .NET 9.0:
  - **Windows**: Windows 10 or later
  - **macOS**: macOS 11 (Big Sur) or later
  - **Linux**: Various distributions with .NET runtime installed

## Configuration
- No database required (data is stored in memory)
- No external services required
- Configured to run as a console application

## Installation Steps

1. **Install .NET 9.0 SDK**
   - Download from [Microsoft's .NET download page](https://dotnet.microsoft.com/download)

2. **Clone/Create the project in Rider**
   - Ensure target framework is set to .NET 9.0

3. **Install required NuGet packages**
   - In Rider: Right-click project > Manage NuGet Packages
   - Add each package listed above

4. **Copy the sensor_data.txt file**
   - Place in project root
   - Set "Copy to Output Directory" property to "Copy always"

5. **Build the solution**
   - Ensure there are no compilation errors

## Running the Application

1. In Rider, press F5 or click the Run button
2. The application will:
   - Read sensor data from the text file
   - Process it through various design patterns
   - Display the results in the console

## Troubleshooting

- If you encounter "File not found" errors, check that sensor_data.txt is being copied to the output directory
- For Akka.NET related errors, ensure the correct version is installed and compatible with .NET 9.0
- If unit tests fail, ensure they're being run with the correct MSTest adapter