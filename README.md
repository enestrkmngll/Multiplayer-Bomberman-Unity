\# Multiplayer Bomberman - Unity



\[cite\_start]A multiplayer Bomberman game developed by effectively applying Object-Oriented Programming (OOP) principles and software Design Patterns. \[cite\_start]Built using the Unity game engine and C#, the project reimagines classic arcade mechanics through a modern, scalable, and maintainable software architecture\[cite: 19].



\## 🚀 Key Features

\* \[cite\_start]\*\*Multiplayer Infrastructure:\*\* Built upon Unity Netcode for GameObjects, utilizing a Client-Server architecture to ensure synchronized gameplay\[cite: 25].

\* \[cite\_start]\*\*Dynamic AI:\*\* AI-controlled bots managed by distinct algorithms (Wander \& Chase)\[cite: 27, 246].

\* \[cite\_start]\*\*Data Persistence:\*\* Integrated local SQLite database for secure user registration, authentication, and a competitive Leaderboard\[cite: 26].

\* \[cite\_start]\*\*Dynamic Environments:\*\* Three unique themes (Desert, Forest, City) dynamically generated\[cite: 24].

\* \[cite\_start]\*\*Power-up System:\*\* Various power-ups including speed boosts, explosion range extensions, and extra bomb capacity\[cite: 23].



\## 🏗️ Software Architecture

\[cite\_start]To ensure code maintainability and avoid the "God Class" problem, the project is structured strictly on the \*\*MVP (Model-View-Presenter)\*\* architectural pattern\[cite: 31, 32]. 

\* \[cite\_start]\*\*Model:\*\* Holds the state (POCO classes) without MonoBehaviour overhead\[cite: 45, 46].

\* \[cite\_start]\*\*View:\*\* Passive interface responsible only for scene visualization and capturing input\[cite: 49, 50].

\* \[cite\_start]\*\*Presenter:\*\* The orchestrator that processes logic and handles Network Synchronization\[cite: 54, 59].



\## 🧩 Applied Design Patterns

\[cite\_start]The following design patterns were rigorously implemented to ensure high code quality and modularity\[cite: 29]:



\### Creational Patterns

\* \[cite\_start]\*\*Abstract Factory:\*\* Used for dynamic theme generation (`ThemeFactory`, `DesertThemeFactory`, etc.)\[cite: 97, 100].

\* \[cite\_start]\*\*Singleton:\*\* Centralized state, network, and data management (`GameManager`)\[cite: 160].



\### Structural Patterns

\* \[cite\_start]\*\*Facade:\*\* Orchestrates complex startup sequences like network init, theme sync, and delayed spawning (`GameStarterFacade`)\[cite: 205, 208].



\### Behavioral Patterns

\* \[cite\_start]\*\*State Pattern:\*\* Manages Enemy AI behaviors (`WanderState`, `ChaseState`) dynamically\[cite: 251, 254].

\* \[cite\_start]\*\*Observer Pattern:\*\* Decouples the explosion system, notifying players, enemies, and walls via the `IExplosionObserver` interface\[cite: 303, 307].

\* \[cite\_start]\*\*Strategy Pattern:\*\* Encapsulates different power-up effects (`SpeedBoostStrategy`, `RangeBoostStrategy`, `BombCountStrategy`)\[cite: 344, 347].



\### Data Management

\* \[cite\_start]\*\*Repository Pattern:\*\* Isolates SQLite data access logic from business logic (`UserRepository`)\[cite: 385, 399].

