# 可视化配置指南

## 场景结构图

```
Hierarchy 场景结构
├── Main Camera (摄像机)
│   └── Position: (0, 50, -30)
│   └── Rotation: (60, 0, 0)
├── Directional Light (光源)
├── Ground (地面 - Plane)
│   └── Scale: (100, 1, 100)
├── Player (玩家 - Cube) ⭐
│   ├── PlayerController 组件
│   ├── BulletDirectionSetter 组件
│   ├── GameEventListener 组件
│   └── BulletSpawnPoint (子对象)
│       └── Local Position: (0, 0.5, 0)
└── EnemyManager (Empty GameObject) ⭐
    └── EnemyManager 组件
```

## 步骤4: 预制体保存流程图

```
Hierarchy 中的临时对象
(编辑器工具创建的)
        |
        | 拖拽到 Project 面板
        v
Assets/Prefabs/ 文件夹
├── Bullet.prefab
├── Enemy.prefab
├── HitEffect.prefab
└── DeathEffect.prefab
        |
        | 保存完成后
        v
删除 Hierarchy 中的临时对象
(预制体已保存，不再需要)
```

## 步骤5: 组件引用连接图

### 5.1 Player 对象配置

```
Player GameObject
│
├── PlayerController 组件
│   ├── Move Speed: 10
│   ├── Shoot Interval: 0.1
│   ├── Bullet Prefab: ───→ Bullet.prefab
│   ├── Bullet Spawn Point: ───→ BulletSpawnPoint (子对象)
│   └── On Bullet Spawn Event: ───→ OnBulletSpawnEvent.asset
│
├── BulletDirectionSetter 组件
│   ├── Player: ───→ Player (自己)
│   ├── Enemy Layer: [✓] Enemy
│   └── Detection Range: 50
│
├── GameEventListener 组件
│   ├── Game Event: ───→ OnBulletSpawnEvent.asset
│   └── Response: 
│       └── BulletDirectionSetter.OnBulletSpawned
│
└── BulletSpawnPoint (子对象)
    └── Local Pos: (0, 0.5, 0)
```

### 5.2 Bullet 预制体配置

```
Bullet.prefab
│
├── Bullet 组件
│   ├── Speed: 20
│   ├── Lifetime: 5
│   └── Enemy Layer: [✓] Enemy
│
└── Trail Renderer (可选)
    ├── Time: 0.3
    ├── Start Width: 0.1
    ├── End Width: 0.01
    ├── Start Color: 黄色
    └── End Color: 红色
```

### 5.3 Enemy 预制体配置

```
Enemy.prefab
Layer: Enemy ⚠️
│
└── Enemy 组件
    ├── Max Health: 3
    ├── Hit Effect Prefab: ───→ HitEffect.prefab
    ├── Death Effect Prefab: ───→ DeathEffect.prefab
    └── On Enemy Death Event: (可选)
```

### 5.4 EnemyManager 配置

```
EnemyManager (Empty GameObject)
│
└── EnemyManager 组件
    ├── Enemy Prefab: ───→ Enemy.prefab
    ├── Enemy Count: 10000
    ├── Spawn Area Size: (500, 500)
    ├── Activation Radius: 100
    ├── Player: ───→ Player GameObject
    └── Grid Cell Size: 50
```

## 运行时工作流程图

```
游戏开始
   │
   v
EnemyManager.Start()
生成 10,000 个敌人
放入空间网格
   │
   v
每帧 Update()
   │
   ├─→ Player Update
   │   ├─ 检测输入
   │   ├─ 移动
   │   └─ 自动射击 (每 0.1 秒)
   │       │
   │       v
   │   生成子弹
   │   触发 OnBulletSpawnEvent
   │       │
   │       v
   │   GameEventListener
   │   调用 BulletDirectionSetter
   │       │
   │       v
   │   查找最近敌人
   │   设置子弹方向
   │       │
   │       v
   │   子弹飞行 + 碰撞检测
   │       │
   │       v (击中敌人)
   │   Enemy.TakeDamage()
   │   播放受击特效
   │       │
   │       v (生命值 = 0)
   │   Enemy.Die()
   │   播放死亡特效
   │   通知 EnemyManager
   │       │
   │       v
   │   EnemyManager.RespawnEnemy()
   │   随机位置重生
   │   更新空间网格
   │
   └─→ EnemyManager Update
       ├─ 查找玩家周围网格
       └─ 激活/停用敌人
```

## 空间网格优化示意图

```
整个游戏区域 (500 x 500 单位)

    停用敌人区域        激活区域          停用敌人区域
        ░░░░         (半径100)             ░░░░
        ░░░░      ┌──────────┐             ░░░░
        ░░░░      │          │             ░░░░
        ░░░░      │  ●●  ●●  │             ░░░░
        ░░░░      │ ●● ●●● ● │             ░░░░
        ░░░░      │  ● ★ ●●  │             ░░░░
        ░░░░      │ ●●●●● ●● │             ░░░░
        ░░░░      │  ●●● ●●  │             ░░░░
        ░░░░      │          │             ░░░░
        ░░░░      └──────────┘             ░░░░
        ░░░░                               ░░░░

░ = 停用的敌人 (Deactivated)
● = 激活的敌人 (Activated)
★ = 玩家 (Player)

网格大小: 50x50 单位
只检查周围 3x3 = 9 个网格
性能提升: 10,000 → ~100 活跃敌人
```

## 网格坐标系统

```
每个格子 50x50 单位，玩家在 (0,0)，只遍历周围 9 格

┌──────┬──────┬──────┐
│(-1,1)│ (0,1)│ (1,1)│
├──────┼──────┼──────┤
│(-1,0)│★(0,0)│ (1,0)│
├──────┼──────┼──────┤
│(-1,-1)│(0,-1)│(1,-1)│
└──────┴──────┴──────┘
```

## 粒子特效对比

```
受击特效 (HitEffect)          死亡特效 (DeathEffect)

    橙色小爆炸                    大型爆炸渐变

      ● ●                          ●   ●   ●
    ● ● ● ●                    ●   ●   ●   ●   ●
      ● ●                          ●   ●   ●

粒子数: 20                    粒子数: 100
半径: 0.3                     半径: 1.0
颜色: 橙色固定                颜色: 黄→红→黑渐变
尺寸: 0.2                     尺寸: 0.5→0 渐变
速度: 3                       速度: 8
生命: 0.5秒                   生命: 1.0秒
```

## 检查清单符号说明

- ✅ = 已完成
- ⚠️ = 重要配置
- ⭐ = 关键对象

## 快速参考 - 必须连接的引用

### Player 对象 (3个组件)
1. PlayerController
   - Bullet Prefab → Bullet.prefab
   - Bullet Spawn Point → BulletSpawnPoint子对象
   - On Bullet Spawn Event → OnBulletSpawnEvent.asset

2. BulletDirectionSetter
   - Player → Player自身
   - Enemy Layer → Enemy (勾选)

3. GameEventListener
   - Game Event → OnBulletSpawnEvent.asset
   - Response → BulletDirectionSetter.OnBulletSpawned

### Bullet 预制体
- Enemy Layer → Enemy (勾选)

### Enemy 预制体
- Layer → Enemy
- Hit Effect Prefab → HitEffect.prefab
- Death Effect Prefab → DeathEffect.prefab

### EnemyManager
- Enemy Prefab → Enemy.prefab
- Player → Player对象
