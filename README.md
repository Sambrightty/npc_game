🧠 Adaptive Enemy NPC in Unity
This project demonstrates an adaptive Enemy NPC using Finite State Machine (FSM), Genetic Algorithm (GA), Perception (Vision & Hearing), and a Behavior Tree design, built in Unity (v3.12.1.0). It features a 1v1 combat system with realistic combat behaviors, health systems, contextual voice lines, and a responsive UI.

🎮 Game Overview
The game is a stylized 1v1 arena where the Player and an Enemy NPC engage in melee combat. The enemy observes the player's tactics and adapts over time, making each session progressively more challenging.

🔥 Core Features
✅ Finite State Machine-based Enemy AI

✅ Adaptive Learning with Genetic Algorithm

✅ Enemy Perception (Line of Sight + Hearing System)

✅ Health System with Punch/Block Mechanics

✅ Red-Damage Overlay & Low-Health Effects

✅ UI Panels: Start, Pause, Instruction, Quit, Health Bars

✅ Voice Cues for Hurt, Victory, Taunt, etc.

✅ Player Behavior Tracking

✅ Integrated Behavior Tree (Design Reference)

🛠️ Technologies Used
Unity 3.12.1.0

C#

Mixamo animations (Enemy)

Custom FSM & GA implementation

draw.io for behavior tree design

🗂️ Folder Structure
arduino
CopyEdit
📦Assets
┣ 📂Animations
┣ 📂Audio
┣ 📂Materials
┣ 📂Models
┣ 📂Prefabs
┣ 📂Scenes
┣ 📂Scripts
┃ ┣ 📜EnemyFSM.cs
┃ ┣ 📜EnemyState.cs
┃ ┣ 📜EnemyGrudgeMemory.cs
┃ ┣ 📜PlayerController.cs
┃ ┣ 📜PlayerBehaviorTracker.cs
┃ ┣ 📜HealthSystem.cs
┃ ┣ 📜VoiceManager.cs
┃ ┣ 📜UIManager.cs
┃ ┣ 📜FieldOfView.cs
┃ ┣ 📜EnemyHearing.cs
┃ ┣ 📜GeneticAlgorithm.cs
┃ ┗ 📜Utils.cs
┗ 📂UI

🧠 Behavior Trees
📌 Enemy Behavior Tree includes:
Patrol

Search

Chase

Attack

Retreat

Adapt using GA based on player style

📌 Player Behavior Tree includes:
Punch

Block

Move

Retreat

Heal

👉 Diagrams are available under /Docs or as SVGs in the repo.

🚀 How to Run
Clone the repo:

bash
CopyEdit
git clone https://github.com/your-username/game_name.git

Open in Unity (v3.12.1.0 or later)

Load the MainScene from Assets/Scenes

Hit ▶️ Play!

🧪 Controls
Action
Key
Move
Arrow Keys
Punch
Space (Player)
Block
B
Heal Player
H

📈 Future Improvements
✅ Refactor FSM into modular states

🔲 Add combo system for player

🔲 Visualize behavior tree at runtime

🔲 Implement sound-based distractions

🔲 Multiplayer 1v1 over LAN

🔲 Skill-based upgrades and unlocks

🙌 Credits
Mixamo for 3D enemy model and animations

Unity Asset Store (Free UI Pack)

Audio sources: freesound.org

Developed by [Your Name]

📄 License
MIT License. Feel free to fork, improve, or build on it!
