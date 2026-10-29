# Dimension Dash

2D mobile auto-runner (Unity 6, `6000.6.7f1`, URP 2D, Input System). The player runs right
automatically; jump and switch between two dimensions (REALITY / SHADOW) to make platforms and
obstacles solid or ghosted.

## Layout

- `Assets/Scripts/` — all game code (flat, no asmdef, compiles into `Assembly-CSharp`)
  - `GameManager` — self-bootstrapping persistent singleton (`RuntimeInitializeOnLoadMethod`, not placed in
    scenes): attempt counter, `RestartLevel` / `ReplayLevel`, target frame rate
  - `LevelManager` — per-scene singleton (`DefaultExecutionOrder(-100)`): play state (Playing/Dead/Complete),
    current `Dimension`, static `DimensionChanged` event, kill height, death/finish flow
  - `Dimension` — `Dimension { Reality, Shadow }` (world state), `DimensionPresence { Reality, Shadow, Both }`
  - `DimensionListener` (abstract) — subscribes to `DimensionChanged`; tints a SpriteRenderer, Tilemap or Camera
    - `DimensionColor` — recolours with the world (player, floor, camera background)
    - `DimensionObject` — exists in one dimension or both; ghosted (no collision, faded) when absent
  - `PlayerController` — velocity-driven run, fixed-arc jump (buffer + coyote), overlap-box ground check,
    contact-normal deaths, visual-only spin; input via serialized `InputAction`s + touch halves
  - `CameraController` — aspect-independent framing (min visible width), vertical dead-zone follow
  - `DimensionTile` (Tile) — authoring tile: kind (Floor/Block/Spike) + `DimensionPresence` + runtime tile
  - `LevelTilemap` — on load, copies the authoring tilemap's tiles onto the runtime layers, then hides it
  - `SafeArea` — fits a RectTransform to `Screen.safeArea`; `LevelCompleteUI` — end screen
  - `StringHelper` — tag/layer name constants; keep in sync with Tags & Layers in ProjectSettings
- `Assets/Scenes/TestScene.unity` — the only scene in the build (contains level 1)
- `Assets/Tiles/` — the 7 `DimensionTile`s (Floor, Block/Spike × Both/Reality/Shadow), the `Level` palette,
  and `Runtime/` (the white Block/Spike tiles the layers use; not for painting)
- `Assets/Prefabs/` — Player, Level (Finish Line), FX (death particles), UI
- `Assets/Settings/` — URP pipeline asset, 2D renderer, URP global settings
- `Assets/TextMesh Pro/` — imported TMP essentials; do not edit

Sprites use URP's `Sprite-Lit-Default` material (no 2D lights in the scene, so they render at full
brightness); particles use `ParticlesUnlit`. Active input handling is Input System only — the legacy
`Input` class throws at runtime.

## Building levels

A level is a `Grid` (cell size 1) with:

- `Level` — the **only tilemap you paint on** (`LevelTilemap` component). Paint any tile from the `Level`
  palette here; mixed sections (any kind, any dimension) can be selected and moved together. Tiles show their
  dimension's colour as a preview.
- `Runtime Layers` — `Floor` (DimensionColor), `Blocks_Both/Reality/Shadow` (Ground layer, DimensionObject),
  `Spikes_Both/Reality/Shadow` (`Obstacle` tag, DimensionObject). **Keep them empty**: `LevelTilemap` fills them
  on load. Each has a static Rigidbody2D and a TilemapCollider2D merged into a CompositeCollider2D (geometry
  type Polygons — the ground check needs it). Dimension colours and ghost alpha are set on these layers;
  keep the authoring tiles' preview colours in sync.

Rules:

- Keep all geometry on whole cells. The floor top is y = 0; the player is 1.25 tall, a spike 0.87.
- Jump height is ~2.45 above the surface: step-ups of 2 cells max. A row the player runs under needs
  2 free cells above the surface, 3 if a hanging spike is there.
- Rotate a hanging spike 180° with `[` / `]` on the brush; the rotation is copied to the runtime layer.
- To add a new tile type, create a `DimensionTile` asset (kind, presence, runtime tile) and add it to the
  palette; a new kind also needs a runtime layer entry on `LevelTilemap`.
- A new level scene needs a `LevelManager` (player + UI references), the Player and Canvas prefabs, and a
  Main Camera with `CameraController` + `DimensionColor`. No GameManager.

## Controls

Keyboard: `D` / `Space` / `Up` (hold) = jump, `A` = switch dimension; gamepad South = jump, West = switch
(bindings are editable on the Player prefab). Touch: right half = jump (hold), left half = switch. A touch
keeps the role of the half it started in.

## Driving the Unity Editor

The Editor is usually open and reachable through the Unity CLI (`unity`, via the `com.unity.pipeline`
package; see the `unity:unity-cli` skill). Use it instead of guessing:

- `unity status` — is the Editor connected (`ready`)?
- `unity recompile --project-path .` — recompile and get compile errors as JSON. Run after every `.cs` edit.
- `unity command console get` — read the Editor console.
- `unity command script eval -- --code '...'` — run C# in the Editor (method body: no `using`, fully
  qualify types). Use it for scene/prefab/asset/settings changes.
- Play-mode check: `editor.playmode start`, `capture game_view`, `editor.playmode stop`.
  `capture game_view --save_path` resolves relative to `Assets/`: delete the capture with
  `AssetDatabase.DeleteAsset` right after copying it out.
- `runtime simulate_key` doesn't hold keys while the Editor is unfocused (the Input System resets devices).
  To hold input in a test, add a virtual Keyboard device and queue its state every frame from
  `InputSystem.onBeforeUpdate`; remove the device and reload the domain afterwards.
- The play view is usually the Device Simulator (notched phone). `PlayModeWindow.SetViewType` /
  `SetCustomRenderingResolution` switch to the Game view at a given resolution; restore the Simulator after.

## Working rules

- Only edit `.cs` files directly. Scenes, prefabs, `.asset` and `.meta` files are Unity YAML; change them
  through the Editor (Unity CLI above), never by hand, and never delete/rename a `.meta` file separately
  from its asset.
- When creating, moving or renaming a script, the class name must match the file name, and the `.meta`
  must move with it (use `AssetDatabase.MoveAsset`), or references in scenes/prefabs break.
- Never read or edit `Library/`, `Temp/`, `Logs/`, `obj/`, `UserSettings/` — generated by Unity.
- Prefer Unity 6 APIs: `Rigidbody2D.linearVelocity`, `FindObjectsByType<T>()`, `FindAnyObjectByType`
  (`FindFirstObjectByType` and `GetInstanceID` are obsolete in 6.6 — use `GetEntityId`).
- Subscribe to `LevelManager.DimensionChanged` (derive from `DimensionListener`) instead of searching the scene.
- Physics-driven objects should be moved in `FixedUpdate` via the Rigidbody, not `transform.Translate`.
- Keep the package list minimal: add a package only when code or assets actually use it.
