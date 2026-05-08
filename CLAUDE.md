# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

CatDetective is a Unity 6 (6000.3.11f1) 3D interactive game using the Universal Render Pipeline (URP). It is a CS480 course project featuring a cat detective character navigating indoor environments with dialogue, NPC interaction, and object-picking mechanics.

## Unity Workflow

This is a Unity project — there are no CLI build or test commands. All development happens through the Unity Editor:

- **Open the project**: Launch Unity Hub → Open → select this folder
- **Build**: File → Build Settings in the Unity Editor
- **Run**: Press Play in the Unity Editor toolbar
- **Scripts compile automatically** when Unity regains focus after a file is saved
- C# scripts are edited externally (VS Code or Visual Studio) and Unity reloads them on focus

The solution file `CatDetective.sln` can be opened in Visual Studio or VS Code for IntelliSense/debugging support.

## Key Packages

- **com.unity.render-pipelines.universal** (17.3.0) — URP; all materials must use URP-compatible shaders
- **com.unity.inputsystem** (1.19.0) — New Input System; use `Input.GetKey` (legacy) or the new `InputSystem` API; both are currently active
- **com.unity.ai.navigation** (2.0.11) — NavMesh baking and agents for AI movement
- **com.unity.probuilder** (6.0.9) — In-editor geometry/level building
- **com.unity.timeline** (1.8.11) — Cutscene and sequenced animation

## Scene Architecture

Each scene is self-contained and tests/implements one mechanic:

| Scene | Purpose |
|---|---|
| `MainScene` | Primary gameplay: cat character + third-person camera |
| `CharacterScene` | Character movement variants and camera experiments |
| `DialogueScene` | NPC proximity detection and dialogue system |
| `PickupScene` | Raycast-based object selection with outline highlighting |
| `Lobby` | Hub/menu area with rain VFX and extended environment |

## Script Organization

Scripts live in `Assets/Scripts/` organized by scene:

```
Scripts/
  CharacterScene/   — movement scripts (transform, physics, camera-relative)
  MainScene/        — camera controllers
  DialogueScene/    — dialogue data, manager, trigger, NPC proximity
  PickupScene/      — simple movement, outline selection, click-to-hide
```

### Core Systems

**Movement** (`CharacterScene/`)
- `CharacterMovement.cs` — WASD transform-based movement; drives `isWalking` Animator bool
- `CatMovementPhysics.cs` — Rigidbody.MovePosition version; use in FixedUpdate; requires frozen X/Z rotation
- `CatMovementCameraRelative.cs` — Projects movement onto camera's XZ plane

**Camera** (`MainScene/`)
- `ThirdPersonShooterCamera.cs` — Mouse-look camera (pitch –20°→60°, yaw free); exposes `Yaw` property consumed by `CatMovementPhysics`
- `ThirdPersonCameraFollow.cs` — Lerp/Slerp follow; no mouse control
- `CameraFollow.cs` — Minimal fixed-offset follow

**Dialogue** (`DialogueScene/`)
- `Dialogue.cs` — Serializable data class (array of sentences); attach to NPC GameObjects
- `DialogueTrigger.cs` — Calls `DialogueManager.StartDialogue()` when triggered
- `DialogueManager.cs` — Singleton-style (found via `FindObjectOfType`); uses a `Queue<string>` and a typewriter coroutine (0.05 s/char); controls `DialogueCanvas` Animator (`isOpen` bool)
- `NPCProximity.cs` — Sphere-checks player each frame (radius 3 f); shows interact prompt; E key triggers dialogue

**Interaction** (`PickupScene/`)
- `outlineSelection.cs` — Finds nearest "selectable"-tagged object within 5 f; dynamically adds/removes `Outline` component (magenta, width 7)
- `ClickToHide.cs` — Raycasts from mouse on click; triggers dialogue then hides the hit object
- `Outline.cs` — Third-party QuickOutline asset; do not modify

## Conventions

- Interactive objects must have the **"selectable"** tag for `outlineSelection` and `ClickToHide` to detect them.
- The `DialogueManager` is located at runtime via `FindObjectOfType` — there must be exactly one in the scene.
- Physics-based movement scripts go in `FixedUpdate`; camera and input reading go in `Update`/`LateUpdate`.
- Animation state is driven by Animator booleans (`isWalking`, `isOpen`); parameter names must match exactly.
- All new materials should use URP shaders (Universal Render Pipeline/Lit or Unlit) — Built-in shaders will render pink.
