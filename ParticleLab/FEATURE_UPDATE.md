# ParticleLab 功能更新

## 新增功能

### 1. FixedUpdate 固定帧率射击 ✅
**文件**: `PlayerController.cs`

**改动**:
- 自动射击逻辑从 `Update()` 移到 `FixedUpdate()`
- 使用 `Time.fixedDeltaTime` 替代 `Time.deltaTime`
- 确保射击频率不受帧率波动影响

**效果**: 无论帧率如何，都精确按照每0.1秒（每秒10发）的频率射击

---

### 2. 正交相机跟随玩家 ✅
**文件**: `CameraFollowPlayer.cs` (新建)

**功能**:
- 平滑跟随玩家移动
- 自动设置正交模式（Orthographic）
- 可配置跟随偏移、平滑速度、正交尺寸

**参数**:
- Target: Player对象
- Offset: (0, 20, 0) - 相机在玩家正上方20单位
- Smooth Speed: 5 - 跟随平滑度
- Use Orthographic: true - 使用正交投影
- Orthographic Size: 15 - 正交视野大小

**使用**: 
1. 添加到 Main Camera
2. 或使用编辑器工具：`Tools > ParticleLab > Scene Setup > 8. Setup Camera Follow`

---

### 3. 敌人追逐玩家 ✅
**文件**: `Enemy.cs`

**改动**:
- 新增 `chaseSpeed` 参数（默认2，慢速移动）
- 新增 `ChasePlayer()` 方法
- 激活状态的敌人会缓慢向玩家移动

**行为**:
- 只有激活状态（玩家附近100单位内）的敌人才会追逐
- 移动速度远低于玩家（2 vs 10），玩家可轻松躲避
- 自动忽略Y轴，只在地面平面移动

---

### 4. 受击红色闪烁 ✅
**文件**: `Enemy.cs`

**改动**:
- 新增 `flashColor` 参数（默认红色）
- 新增 `flashDuration` 参数（默认0.1秒）
- 新增 `FlashRed()` 协程
- 受击时材质颜色瞬间变红，0.1秒后恢复原色

**实现细节**:
- 使用协程避免重复闪烁
- 自动保存和恢复原始颜色
- 与受击粒子特效同时播放

---

## 更新的文件

### 修改的文件
1. **PlayerController.cs**
   - 射击逻辑移至 FixedUpdate
   - 使用 Time.fixedDeltaTime

2. **Enemy.cs**
   - 添加追逐玩家逻辑
   - 添加红色闪烁效果
   - 新增参数：chaseSpeed, flashColor, flashDuration
   - 新增方法：ChasePlayer(), FlashRed()

3. **EnemyManager.cs**
   - 新增 `GetPlayerTransform()` 公共方法
   - 供敌人获取玩家引用

4. **SceneSetupTool.cs**
   - 新增按钮 "8. Setup Camera Follow"
   - 自动配置相机跟随

### 新建的文件
5. **CameraFollowPlayer.cs** (新)
   - 完整的相机跟随系统
   - 正交模式支持
   - 平滑跟随算法

---

## 配置检查清单

### Enemy 预制体配置
打开 Enemy.prefab，确认以下参数：

```
Enemy 组件:
├── Max Health: 3
├── Chase Speed: 2  ← 新增
├── Flash Color: (1, 0, 0, 1) 红色  ← 新增
├── Flash Duration: 0.1  ← 新增
├── Hit Effect Prefab: HitEffect
└── Death Effect Prefab: DeathEffect
```

### Main Camera 配置
选中 Main Camera，添加 CameraFollowPlayer 组件：

```
Camera Follow Player 组件:
├── Target: Player (拖入)
├── Offset: (0, 20, 0)
├── Smooth Speed: 5
├── Use Orthographic: ✓
└── Orthographic Size: 15
```

或使用编辑器工具一键设置：
`Tools > ParticleLab > Scene Setup > 8. Setup Camera Follow`

---

## 测试验证

### 固定帧率射击
- [ ] 打开 Profiler，观察射击频率
- [ ] 无论帧率高低，子弹数量稳定在 10发/秒

### 相机跟随
- [ ] 移动玩家，相机平滑跟随
- [ ] 相机始终在玩家正上方俯视
- [ ] 正交模式：无透视变形
- [ ] 边缘对象不会因距离变小/变大

### 敌人追逐
- [ ] 靠近敌人时，敌人开始缓慢移动
- [ ] 敌人移动方向始终朝向玩家
- [ ] 玩家移动速度快于敌人，可以躲避
- [ ] 远离时，敌人停止追逐并停用

### 红色闪烁
- [ ] 子弹击中敌人，敌人瞬间变红
- [ ] 0.1秒后恢复原色
- [ ] 连续击中不会导致闪烁叠加
- [ ] 与橙色受击特效同时显示

---

## 性能影响

### 敌人追逐
- **影响**: 约100个激活敌人每帧计算移动
- **开销**: 极小（简单的向量运算）
- **优化**: 已限制只有激活状态的敌人才移动

### 红色闪烁
- **影响**: 每次受击启动一个协程
- **开销**: 极小（0.1秒后自动清理）
- **优化**: isFlashing 标记防止重复协程

### 相机跟随
- **影响**: LateUpdate 每帧一次插值计算
- **开销**: 可忽略（单个对象）

**预期帧率**: 60+ FPS（无明显性能下降）

---

## 下一步建议

### 可选增强
- [ ] 添加敌人转向动画
- [ ] 多种敌人移动模式（巡逻、包抄）
- [ ] 相机震动反馈
- [ ] 击中粒子特效颜色与闪烁同步
- [ ] 敌人碰撞检测（避免重叠）

### 调优参数
- 敌人追逐速度（太快/太慢）
- 相机跟随平滑度（太生硬/太迟缓）
- 闪烁持续时间（太快/太慢）
- 正交相机尺寸（视野太大/太小）

---

## 完成时间
2026-08-13

所有新功能已实现并集成到现有系统中。
