# SpawnArea 引用功能更新

## 改动说明

**需求**: 修改 EnemyManager，让刷怪位置从一个引用的 GameObject `SpawnArea` 读取坐标和大小，而不是硬编码 Vector2。

## 实现细节

### 1. EnemyManager.cs

**新增字段**:
```csharp
[SerializeField] private Transform spawnArea;
```

**修改逻辑** (`GetRandomGroundPosition`):
- 如果 `spawnArea` 不为空：
  - 中心点 = `spawnArea.position`
  - 半径 = `spawnArea.localScale.x/z * 5`（Plane 默认 10x10，scale 乘以 5 得半径）
- 否则回退到原有的 `spawnAreaSize` 逻辑（世界原点为中心）

**兼容性**: 现有配置不受影响 — 如果 `spawnArea` 为空，行为与之前完全一致。

### 2. SpawnArea GameObject

自动创建工具会生成一个红色半透明平面作为可视化刷怪区域：

```
名称: SpawnArea
类型: Plane (移除 Collider，仅用于可视化)
位置: (0, 0.01, 0)  // 略高于地面，避免 Z-fighting
缩放: (50, 1, 50)    // 默认 500x500 单位区域
材质: 红色半透明 (alpha 0.2)
```

**区域大小计算公式**:
```
实际刷怪范围 = localScale.x * 10 × localScale.z * 10 单位
例: scale (50, 1, 50) = 500x500 单位
```

### 3. 编辑器工具更新

#### FixReferences.cs
- 自动创建 SpawnArea GameObject（如果不存在）
- 自动将其连线到 `EnemyManager.spawnArea`

#### SceneSetupTool.cs
- 新增按钮 "2.5. Create Spawn Area"（手动创建时可用）

## 使用方式

### 自动设置（推荐）
```
Tools > ParticleLab > Fix All References + Rebuild Scene
```
会自动创建 SpawnArea 并连线。

### 手动调整

1. **移动刷怪区域**:
   - 在 Hierarchy 中选中 SpawnArea
   - 拖动 Position Gizmo 到目标位置

2. **调整区域大小**:
   - 修改 SpawnArea 的 `Transform.localScale.x/z`
   - 例: scale (30, 1, 30) = 300x300 单位

3. **换用其他 GameObject**:
   - SpawnArea 不一定要是 Plane
   - 任何有 Transform 的 GameObject 都可以（例如空对象 + Gizmo）
   - EnemyManager 只读取 `position` 和 `localScale`，忽略 rotation

## 测试验证

- [ ] EnemyManager.spawnArea 已赋值为 SpawnArea
- [ ] 运行游戏，敌人分布在红色平面范围内
- [ ] 移动 SpawnArea，敌人重生位置跟随
- [ ] 缩放 SpawnArea，刷怪范围相应变化
- [ ] spawnArea 为空时，回退到 spawnAreaSize 逻辑（原有行为）

## 可选增强

- 在 EnemyManager 的 OnDrawGizmosSelected 中绘制刷怪范围边界（绿色线框）
- 支持旋转的椭圆/矩形区域（当前只取 x/z，忽略 rotation）
- 多个刷怪区域（数组），每个区域分配一定比例的敌人

---

**更新时间**: 2026-08-13  
**文件变更**: EnemyManager.cs, FixReferences.cs, SceneSetupTool.cs
