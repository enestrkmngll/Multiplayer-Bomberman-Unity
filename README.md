# Multiplayer Bomberman - Unity

A multiplayer Bomberman game developed by effectively applying Object-Oriented Programming (OOP) principles and software Design Patterns. Built using the Unity game engine and C#, the project reimagines classic arcade mechanics through a modern, scalable, and maintainable software architecture.

## 🚀 Key Features
* **Multiplayer Infrastructure:** Built upon Unity Netcode for GameObjects, utilizing a Client-Server architecture to ensure synchronized gameplay.
* **Dynamic AI:** AI-controlled bots managed by distinct algorithms (Wander & Chase).
* **Data Persistence:** Integrated local SQLite database for secure user registration, authentication, and a competitive Leaderboard.
* **Dynamic Environments:** Three unique themes (Desert, Forest, City) dynamically generated.
* **Power-up System:** Various power-ups including speed boosts, explosion range extensions, and extra bomb capacity.

## 🏗️ Software Architecture
To ensure code maintainability and avoid the "God Class" problem, the project is structured strictly on the **MVP (Model-View-Presenter)** architectural pattern. 

* **Model:** Holds the state (POCO classes) without MonoBehaviour overhead.
* **View:** Passive interface responsible only for scene visualization and capturing input.
* **Presenter:** The orchestrator that processes logic and handles Network Synchronization.

## 🧩 Applied Design Patterns
The following design patterns were rigorously implemented to ensure high code quality and modularity:

### Creational Patterns
* **Abstract Factory:** Used for dynamic theme generation (ThemeFactory, DesertThemeFactory, etc.).
* **Singleton:** Centralized state, network, and data management (GameManager).

### Structural Patterns
* **Facade:** Orchestrates complex startup sequences like network init, theme sync, and delayed spawning (GameStarterFacade).

### Behavioral Patterns
* **State Pattern:** Manages Enemy AI behaviors (WanderState, ChaseState) dynamically.
* **Observer Pattern:** Decouples the explosion system, notifying players, enemies, and walls via the IExplosionObserver interface.
* **Strategy Pattern:** Encapsulates different power-up effects (SpeedBoostStrategy, RangeBoostStrategy, BombCountStrategy).

### Data Management
* **Repository Pattern:** Isolates SQLite data access logic from business logic (UserRepository).
