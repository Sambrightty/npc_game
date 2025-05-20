# 🎮 Adaptive Enemy NPC System (Unity - Final Year Project)

This Unity project showcases an intelligent, adaptive Enemy NPC system capable of perceiving, reacting to, and learning from player behavior in a 3D arena. Built for academic research, it integrates perception mechanics, a finite state machine (FSM), and a foundation for adaptive AI and memory systems.

---

## ✅ Completed Features

### 🔧 Phase 1: Core Setup

- Organized Unity project structure: Scripts, Prefabs, UI, Audio, etc.
- Built enclosed 3D test arena with obstacles and lighting.
- Integrated Unity NavMesh:
  - Baked navigation mesh.
  - Attached `NavMeshAgent` to enemy.

### 👁️ Phase 2: Enemy Perception

- Field of View (FoV) detection using raycasting and angle logic.
- Line-of-sight blocking via `Physics.Raycast`.
- Awareness states: **Unaware**, **Suspicious**, **Alerted**, **Engaged**.
- Auditory detection using trigger collider (e.g., for sprint sounds).
- Debug color/log feedback based on awareness level.

### 🤖 Phase 3: Finite State Machine (FSM)

- Enemy FSM with states: **Patrol**, **Chase**, **Attack**, **Search**, **Retreat**.
- Waypoint-based patrol.
- Navigation and chasing via NavMeshAgent.
- Basic melee attack with damage debug logging.
- Retreat when health is low.

### ❤️ Health System

- `HealthSystem.cs`: modular, reusable for player and enemy.
- Tracks health, applies damage, handles death logic.

### 🧍 Player Combat & Controls

- Third-person movement system.
- Attack input with animations.
- Combat interaction integrated with enemy hitbox and health.

### 🖥️ UI & Game Flow

- Health bars for both player and enemy.
- UI feedback on damage/death.
- Pause menu, instructions overlay, restart and quit options.
- End screen based on win/loss condition.

---

## ⚠️ Remaining Task

| Component                   | Status         | Details                                                                    |
| --------------------------- | -------------- | -------------------------------------------------------------------------- |
| 🧍 Player & Enemy 3D Models | ⚙️ In Progress | Import distinctive animated models for player and enemy.                   |
| 🌆 Arena Visuals            | ⚙️ In Progress | Replace placeholders with textured props, environment packs, and lighting. |
| 🕺 Animation Assignment     | ⚙️ In Progress | Attach walk, idle, attack, hit, and die animations using Animator.         |

> 💡 _All gameplay logic and systems are complete. Only visual polish (3D/Animation) remains._

---

## 🔧 Tech Stack

- Unity 2021+
- C# Scripts
- NavMesh System
- FSM Architecture
- Raycasting & Trigger Colliders
- Unity UI Toolkit

---

## 🎯 Final Project Goals

- ✅ Core gameplay & combat loop
- ✅ AI behavior switching via FSM
- ✅ Player combat, movement, and animations
- ✅ UI with real-time health & state feedback
- ✅ End-game logic with restart and quit flow
- 🔜 Replace placeholders with finalized 3D models and animation assets

---

📌 **Project Status:**  
🟢 _95% complete — core mechanics and logic are done. Awaiting final 3D model & animation integration._
