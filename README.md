# GameLauncher (WPF App)

My classroom has an arcade machine that allows students to play games on using old style arcade sticks. I am building an app to collect games on the pc, display and launch them from a nice interface.

## Problem
Students work hard for a month to make a game but it is cumbersome to move games in order for students to play other students games. It is also hard for outside stakeholders to play students games. The best way currently is screen video recorders to showcase work but viewers need to be able to play games to understand the work. 

Furthermore, there is no "best" situation for hosting games online without paying for a hosting service and then there is the issue of student privacy related to the games that they make.

## Solution
I was fortune to be able to collab with a current Engineering teacher to build a arcade machine in my classroom. This arcade machine would be the perfect host for students to showcase their work. No privacy issues as it will all be local.

## My goals for this project:
* This app needs to be useable by other teachers who teach Game Design with Unity and C#.
* This app must be able to search for games in assigned folders and launch them without issues.
* Once a user is done playing a game, the launcher will return to itself should the user want to play another game.
* The UI must be usable using controllers or the custom joysticks on the machine.
* The UI should have thumbnails for the games in the menu.
* The app should be able to filter games by the school year they were created.
* The first version of this app will be installed and ran locally with opportunity to make it cloud-based in the future.
* The app must have a designated "kill switch" for games that have bugs so the program does not freeze and ruin user experience.

## PD for me for this project
* Use my knowledge of C# and .NET to build a solution for my classroom
* Practice my app development
* Learn the basics of WPF
* Practice project management principles on myself to execute a solution
* Learn System.IO and other file management librarie
* Create a basic way to store data (JSON or SQL) so data is persistent

## Tech Stack
* C# 
* WPF
* .NET 10
* Unity (for games)
* Github for repo

## Architecture & Project Management

### Architectural Design
To ensure the app is maintainable and scalable for other teachers, I am following the **MVVM (Model-View-ViewModel)** pattern. This separates the game-searching and process-launching logic from the WPF user interface, allowing for easier testing and future updates (such as moving to a cloud-based database).

### Project Management Approach
I am acting as the sole Project Manager and Developer for this solution, utilizing the following principles:
* **Requirements Gathering:** Identified specific classroom pain points (privacy, friction, ease of use) to define the MVP (Minimum Viable Product).
* **Milestone Planning:** 
    * Phase 1: Core Logic 
    * Phase 2: UI Development 
    * Phase 3: Deployment & User Testing 
* **Risk Management:** Addressed the "frozen game" risk by planning a global hotkey kill-switch to ensure the arcade machine never requires a keyboard/mouse intervention.

### Core Components
* **Game Discovery Engine:** A service responsible for recursively scanning directories for executable files.
* **Metadata Handler:** Logic to parse file paths and folder names to categorize games by school year.
* **Process Monitor:** A wrapper for the Windows Process class to handle the lifecycle of the launched game and the return-to-menu state.