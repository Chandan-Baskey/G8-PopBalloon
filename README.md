# 🎈 G8-PopBalloon

<div align="center">

![Game View](https://github.com/Chandan-Baskey/G8-PopBalloon/blob/63952c6f38b0162ce3646962f7d201da63e52000/Game-View.jpg)

<br/>

![Unity](https://img.shields.io/badge/Unity-2022.3+-000000?style=for-the-badge&logo=unity&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![Platform](https://img.shields.io/badge/Platform-PC%20%7C%20Mobile-blue?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)

**A fast-paced, click-to-pop balloon arcade game built in Unity. Tap balloons before they escape — lose hearts when they slip past. How long can you hold the sky?**

</div>

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Gameplay](#-gameplay)
- [Features](#-features)
- [Architecture & Scripts](#-architecture--scripts)
- [Game Systems Deep Dive](#-game-systems-deep-dive)
- [Installation](#-installation)
- [How to Play](#-how-to-play)
- [Project Structure](#-project-structure)
- [Tech Stack](#-tech-stack)
- [Contributing](#-contributing)

---

## 🎯 Overview

**G8-PopBalloon** is a Unity-based 2D arcade game where colorful balloons rise from the bottom of the screen. The player's goal is to click/tap them before they float off the top. Every balloon that escapes costs a heart — pop enough to build your score, or lose all hearts and face game over.

The game emphasizes **quick reflexes**, **score chasing**, and **satisfying pop feedback** through randomized sound effects and instant respawn mechanics.

---

## 🎮 Gameplay

```
╔══════════════════════════════════╗
║  🎈   🎈      🎈      🎈   🎈   ║  ← Balloons rising
║                                  ║
║      🎈            🎈            ║
║                                  ║
║  Score: 12    ♥ ♥ ♡              ║  ← HUD
╚══════════════════════════════════╝
         ↑ Click to Pop!
```

| Action | Result |
|--------|--------|
| 🖱️ Click a balloon | +1 Score, balloon respawns |
| 🎈 Balloon escapes top | −1 Heart |
| 💀 0 Hearts remaining | Game Over |
| 🔄 Restart Button | Scene reloads |

---

## ✨ Features

- 🎨 **Multiple Balloon Types** — Supports an array of balloon prefabs with unique behaviors
- 🔊 **Randomized Pop Sounds** — A pool of 4–8 audio clips plays randomly on each pop for varied feel
- ❤️ **Health System** — 3-heart lives system with visual heart UI using rich text color coding
- 📊 **Live Score Tracking** — Real-time score counter displayed via TextMeshPro
- 🚫 **Game Over Panel** — Clean game over screen with restart functionality
- ♻️ **Smart Respawn System** — Balloons are tracked by index and instantly respawned after escape or pop
- 🛡️ **Post-GameOver Safety** — All balloon activity halts cleanly on game over; no ghost respawns
- 🎯 **Configurable Spawn Points** — Designer-friendly spawn point array in the Inspector

---

## 🧩 Architecture & Scripts

The project follows a clean **Manager → Spawner → Balloon** dependency chain.

```
GameManager (Singleton)
    │
    ├── Manages score, health, UI, audio pool
    ├── References Spawner (set at runtime)
    │
    └── Spawner
            │
            ├── Owns activeBalloons[] array
            ├── SpawnBalloon(index) → Instantiates prefab at spawn point
            └── RespawnBalloon(index) → Destroys old, spawns new
                    │
                    └── Movement (on each Balloon prefab)
                            ├── Moves balloon upward each FixedUpdate
                            ├── Detects escape (y > 6f) → LoseHealth + Respawn
                            └── OnMouseDown → AddScore + play sound + Respawn
```

---

## 🔬 Game Systems Deep Dive

### 1. `GameManager.cs` — Central Controller (Singleton)

The `GameManager` is implemented as a **persistent singleton** to ensure only one instance exists throughout the game.

```csharp
void Awake()
{
    if (Instance == null) Instance = this;
    else Destroy(gameObject);
}
```

**Key Responsibilities:**
- **Balloon Prefab Registry** — Stores all balloon prefabs in `balloonPrefabs[]`; the `Spawner` fetches them by index via `GetBalloonPrefab(int index)`.
- **Audio Pool** — `popSounds[]` holds multiple AudioClips. `GetRandomPopSound()` picks one at random on each pop using `Random.Range`, ensuring audio variety without repetition bias.
- **Score & Health** — `AddScore()` and `LoseHealth()` update the internal state and call `UpdateUI()`. Both methods guard against execution after game over via `isGameOver` flag.
- **Heart UI** — Rich text loop renders filled/empty hearts:
  ```csharp
  hearts += (i < currentHealth) ? "<color=red>♥</color> " : "<color=grey>♥</color> ";
  ```
- **Game Over Flow** — Sets `isGameOver = true`, calls `spawner.DisableAllBalloons()`, and activates the Game Over panel.

---

### 2. `Spawner.cs` — Balloon Lifecycle Manager

The `Spawner` owns the full lifecycle of balloons: creation, tracking, destruction, and respawn.

```csharp
private GameObject[] activeBalloons;   // Indexed array of live balloon GameObjects
private bool isDisabled = false;       // Hard stop flag after game over
```

**Spawn Flow:**
1. On `Start()`, registers itself with `GameManager.Instance.spawner = this`
2. Loops through all balloon prefabs and calls `SpawnBalloon(i)` for each
3. Assigns `balloonIndex` on the `Movement` component for future reference

**Respawn Flow:**
- `RespawnBalloon(index)` is called by `Movement` (on escape or click)
- Checks `isDisabled` first — exits immediately if game is over
- Destroys the existing GameObject at that index, then spawns a fresh one

**DisableAllBalloons():**
- Sets `isDisabled = true` to block future respawns
- Iterates `activeBalloons[]` and calls `SetActive(false)`
- **Also** uses `FindObjectsOfType<Movement>()` to catch any balloons that may have just respawned mid-frame (edge case safety net)

---

### 3. `Movement.cs` — Per-Balloon Behavior

Each balloon prefab carries a `Movement` component that controls its upward motion and click interaction.

```csharp
public float upSpeed = 2f;   // Base vertical speed
public float speed;          // Multiplier (set per prefab for variety)
public int balloonIndex;     // Set by Spawner for self-identification
```

**Update Loop (Escape Detection):**
```csharp
void Update()
{
    if (transform.position.y > 6f)
    {
        GameManager.Instance.LoseHealth();
        GameManager.Instance.spawner.RespawnBalloon(balloonIndex);
    }
}
```

**FixedUpdate (Physics Movement):**
```csharp
void FixedUpdate()
{
    if (!isClicked)
        transform.Translate(0, upSpeed * speed * Time.deltaTime, 0);
}
```
Movement is stopped immediately when `isClicked` becomes true, preventing a balloon from continuing to rise after the player has popped it.

**OnMouseDown (Pop Interaction):**
```csharp
private void OnMouseDown()
{
    if (isClicked) return;          // Prevent double-trigger
    isClicked = true;

    GameManager.Instance.AddScore();

    AudioClip randomClip = GameManager.Instance.GetRandomPopSound();
    if (randomClip != null)
        AudioSource.PlayClipAtPoint(randomClip, transform.position);

    GameManager.Instance.spawner.RespawnBalloon(balloonIndex);
}
```
Uses `AudioSource.PlayClipAtPoint` (a fire-and-forget static method) so the audio continues playing even after the balloon GameObject is destroyed during respawn.

---

## 🚀 Installation

### Prerequisites
- Unity **2022.3 LTS** or later
- TextMeshPro package (included via Package Manager)

### Steps

```bash
# 1. Clone the repository
git clone https://github.com/Chandan-Baskey/G8-PopBalloon.git

# 2. Open with Unity Hub
# File → Open Project → Select the cloned folder

# 3. Open the main scene
# Assets/Scenes/GameScene.unity

# 4. Press Play ▶ in the Unity Editor
```

### Inspector Setup Checklist

| Component | What to Assign |
|-----------|---------------|
| `GameManager` → `balloonPrefabs[]` | Drag all balloon prefabs |
| `GameManager` → `popSounds[]` | Drag 4–8 AudioClip assets |
| `GameManager` → `scoreText` | TextMeshProUGUI in Canvas |
| `GameManager` → `healthText` | TextMeshProUGUI in Canvas |
| `GameManager` → `gameOverPanel` | Game Over UI Panel |
| `Spawner` → `spawnPoints[]` | Empty GameObjects as spawn locations |

---

## 🕹️ How to Play

1. **Launch** the game — balloons begin rising immediately
2. **Click / Tap** any balloon to pop it and earn **+1 point**
3. If a balloon **escapes** past the top of the screen, you lose **1 heart** ❤️
4. You start with **3 hearts** — lose all 3 and it's **Game Over**
5. Hit **Restart** on the Game Over screen to play again
6. Challenge yourself to beat your high score!

---

## 📁 Project Structure

```
G8-PopBalloon/
├── Assets/
│   ├── Scripts/
│   │   ├── GameManager.cs      # Singleton controller — score, health, audio, UI
│   │   ├── Spawner.cs          # Balloon lifecycle — spawn, respawn, disable
│   │   └── Movement.cs         # Per-balloon — rising, escape detection, click pop
│   ├── Prefabs/
│   │   └── Balloons/           # Balloon prefab variants (with Movement component)
│   ├── Scenes/
│   │   └── GameScene.unity     # Main game scene
│   ├── Sounds/
│   │   └── PopSounds/          # AudioClip assets for pop sounds
│   └── UI/
│       └── Canvas/             # Score text, health text, game over panel
├── Game-View.jpg               # In-game screenshot
└── README.md
```

---

## 🛠️ Tech Stack

| Tool | Purpose |
|------|---------|
| **Unity 2022.3+** | Game engine and editor |
| **C#** | Scripting language |
| **TextMeshPro** | High-quality UI text rendering |
| **Unity Physics 2D** | Collider-based click detection via `OnMouseDown` |
| **AudioSource** | Spatial and fire-and-forget audio playback |

---

## 🤝 Contributing

Contributions are welcome! Here are some ideas for improvements:

- 🌈 Add more balloon types with different speeds and point values
- 💥 Add particle effects on pop
- 🏆 Implement a high score leaderboard (PlayerPrefs)
- 📱 Add touch input optimization for mobile
- ⏱️ Add a timed game mode

```bash
# Fork → Branch → Commit → PR
git checkout -b feature/your-feature-name
git commit -m "Add: your feature description"
git push origin feature/your-feature-name
```

---

<div align="center">

Made with ❤️ using Unity | **G8-PopBalloon**

⭐ Star this repo if you enjoyed it!

</div>

