# Dimension Dash

A 2D mobile auto-runner built with Unity. Run, jump, and flip between two dimensions — REALITY and
SHADOW — to make platforms and obstacles solid or ghosted.

## Controls

| Action           | Keyboard   | Touch            |
| ---------------- | ---------- | ---------------- |
| Jump (hold)      | `D`        | Right half       |
| Switch dimension | `A`        | Left half        |

## Getting started

1. Install **Unity 6000.6.7f1** via Unity Hub (see `ProjectSettings/ProjectVersion.txt`).
2. Clone and open the project folder in Unity Hub.
3. Open `Assets/Scenes/TestScene.unity` and press Play.

## Project layout

```
Assets/
  Scenes/      Game scenes (in build settings)
  Scripts/     Game code
  Prefabs/     Player, platforms, obstacles, UI
  Sprites/ Materials/ Particles/ Fonts/ Audio/
  TextMesh Pro/, Adaptive Performance/, UI Toolkit/   Package-generated, don't edit
Packages/      Package manifest
ProjectSettings/
```

## Development

- Recommended editor: VS Code with the **Unity** and **C# Dev Kit** extensions
  (Unity → Settings → External Tools → External Script Editor = Visual Studio Code).
- Assets use *Force Text* serialization; always commit `.meta` files with their assets.
- Optional Unity SmartMerge for scene/prefab conflicts (run once per clone):

  ```sh
  git config merge.unityyamlmerge.name "Unity SmartMerge"
  git config merge.unityyamlmerge.driver "'/Applications/Unity/Hub/Editor/6000.6.7f1/Unity.app/Contents/Helpers/UnityYAMLMerge' merge -p %O %B %A %A"
  git config merge.unityyamlmerge.recursive binary
  ```

- Builds go in `Builds/` (git-ignored).
