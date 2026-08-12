# ParticleLab Scene Setup Guide

## Scripts Created
1. **PlayerController.cs** - WASD/Arrow movement + auto-shooting every 0.1s
2. **Bullet.cs** - Bullet movement and collision detection
3. **Enemy.cs** - Enemy health, hit/death effects
4. **EnemyManager.cs** - Spawns 10k enemies, spatial grid activation system
5. **GameEventSO.cs** - ScriptableObject event channel
6. **GameEventListener.cs** - Event listener component
7. **BulletDirectionSetter.cs** - Auto-aim bullets to nearest enemy

## Setup Instructions

### 1. Create Layers
- Project Settings > Tags and Layers
- Add layer: "Enemy"

### 2. Create ScriptableObject Event
- Right-click in Assets/ScriptableObjects
- Create > Events > GameEvent
- Name: "OnBulletSpawnEvent"

### 3. Create Player
- GameObject > 3D Object > Cube (name: Player)
- Add: PlayerController script
- Add: BulletDirectionSetter script
- Add: GameEventListener script
  - Assign OnBulletSpawnEvent
  - Add response: BulletDirectionSetter.OnBulletSpawned

### 4. Create Bullet Prefab
- GameObject > 3D Object > Sphere (scale: 0.2, 0.2, 0.2)
- Add: Rigidbody (Is Kinematic: true)
- Add: Sphere Collider (Is Trigger: true)
- Add: Bullet script
  - Set Enemy Layer mask
- Add Trail Renderer for visual effect
- Save as Prefab in Assets/Prefabs

### 5. Create Enemy Prefab
- GameObject > 3D Object > Cube (scale: 1, 1, 1)
- Set Layer: Enemy
- Add: Capsule Collider (Is Trigger: true)
- Add: Enemy script
- Create hit effect (Particle System - small explosion)
- Create death effect (Particle System - large explosion)
- Save as Prefab in Assets/Prefabs

### 6. Create Ground
- GameObject > 3D Object > Plane (scale: 100, 1, 100)
- Position: (0, 0, 0)

### 7. Create Enemy Manager
- GameObject > Create Empty (name: EnemyManager)
- Add: EnemyManager script
  - Assign Enemy Prefab
  - Set Enemy Count: 10000
  - Set Spawn Area Size: (500, 500)
  - Set Activation Radius: 100
  - Assign Player Transform

### 8. Configure Camera
- Position above player for top-down view
- Or add Cinemachine follow camera

## Features Implemented
✓ WASD + Arrow key movement
✓ Auto-shoot every 0.1s
✓ 10,000 enemies randomly placed
✓ Spatial grid optimization (only activate nearby enemies)
✓ ScriptableObject event system
✓ Hit effects on damage
✓ Death effects with large explosion
✓ Auto-respawn enemies at random positions
✓ Smart bullet targeting to nearest enemy
