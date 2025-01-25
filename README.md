# Personal Task Manager (WPF Application)

A modern desktop task management application built with WPF (.NET) following MVVM architecture pattern, designed to help users organize tasks with advanced filtering, prioritization, and reminders.

## Key Features
- 🗂️ **Task Management**: Create/Edit/Delete tasks with titles, descriptions, due dates
- 🏷️ **Categorization**: Organize tasks using custom categories and tags
- 🚨 **Reminders**: Get notified about upcoming deadlines
- 🔍 **Smart Filtering**: Search by text, filter by category/tag/priority
- 📊 **Statistics**: View completion metrics and productivity insights
- 🎨 **Custom UI**: Clean and intuitive WPF interface with validation
- 💾 **Data Persistence**: JSON-based local storage

## Tech Stack
- **Frontend**: WPF (XAML) with MVVM pattern
- **Core**: .NET 7/8, C#
- **Validation**: DataAnnotations + FluentValidation
- **Dependencies**: 
  - MVVM Toolkit
  - Newtonsoft.Json
  - MaterialDesignThemes (UI)

## Screenshot
![Application Screenshot](/screenshots/main-ui.png) <!-- Add actual path -->

## Getting Started
1. **Prerequisites**: .NET 7/8 SDK
2. **Installation**:
   ```bash
   git clone https://github.com/kilany99/task-manager.git
   cd task-manager
   dotnet restore
   dotnet build
Run: Open in Visual Studio and run/debug

Configuration
Edit appsettings.json to modify storage paths

Default data files created in %AppData%/TaskManager/

Contributing
Contributions welcome! Please follow:

Fork repository

Create feature branch

Submit PR with detailed description

License
MIT © 2024 Kilany99
