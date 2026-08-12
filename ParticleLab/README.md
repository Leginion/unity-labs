# ParticleLab - High Performance Particle Shooting Scene

Unity项目，实现大规模敌人射击场景，使用空间网格优化和粒子特效系统。

## 已实现功能

### ✅ 玩家系统
- WASD / 方向键移动控制
- 每0.1秒自动射击子弹
- 子弹生成点配置
- 通过ScriptableObject事件系统发射子弹

### ✅ 子弹系统
- 自动寻找最近敌人
- 平滑追踪目标
- 碰撞检测（Trigger）
- Trail Renderer拖尾效果
- 自动销毁（5秒生命周期）

### ✅ 敌人系统
- 10,000敌人随机分布在地面
- 空间网格优化（XY Grid）
  - 只激活玩家周围100单位内的敌人
  - 网格大小：50x50单位
- 血量系统（默认3点生命值）
- 受击特效（小型爆炸粒子）
- 死亡特效（大型爆炸粒子）
- 死亡后随机位置重生

### ✅ ScriptableObject事件系统
- GameEventSO - 事件通道
- GameEventListener - 事件监听器
- 解耦子弹生成与目标瞄准逻辑

### ✅ 粒子特效
- Hit Effect（橙色，20粒子，快速爆发）
- Death Effect（红橙渐变，100粒子，大范围爆炸）
- 颜色渐变：黄→红→黑
- 尺寸渐变：大→小
- 透明度渐变：实→透

### ✅ 性能优化
- 空间网格划分（Spatial Grid）
- 只激活视野范围内敌人
- 延迟激活/停用（半径 * 1.2倍）
- 高效网格查询算法

## 文件结构

```
Assets/
├── Scripts/
│   ├── PlayerController.cs          # 玩家移动和射击
│   ├── Bullet.cs                     # 子弹行为
│   ├── BulletDirectionSetter.cs     # 智能瞄准系统
│   ├── Enemy.cs                      # 敌人逻辑
│   ├── EnemyManager.cs               # 敌人管理器（网格优化）
│   ├── GameEventSO.cs                # SO事件通道
│   ├── GameEventListener.cs          # 事件监听组件
│   ├── ParticleEffectConfig.cs       # 粒子效果配置
│   ├── README.md                     # 详细设置指南
│   └── Editor/
│       └── SceneSetupTool.cs         # 编辑器一键设置工具
├── ScriptableObjects/                # SO资源
├── Prefabs/                          # 预制体
└── Scenes/
    └── SampleScene.unity             # 主场景
```

## 快速开始

### 1. Unity中打开项目
```
Unity版本: 2022.3 LTS 或更高
渲染管线: URP (Universal Render Pipeline)
```

### 2. 使用编辑器工具
在Unity菜单栏：`Tools > ParticleLab > Scene Setup`

依次点击按钮：
1. Create Ground
2. Create Player
3. Create Bullet Prefab Template
4. Create Enemy Prefab Template
5. Create Hit Effect Template
6. Create Death Effect Template
7. Create ScriptableObject Event

### 3. 配置层级
Project Settings > Tags and Layers > 添加 "Enemy" 层

### 4. 连接引用
在Inspector中设置：
- Player组件引用
- Bullet Prefab引用
- Enemy Prefab引用
- Hit/Death Effect Prefab引用
- ScriptableObject Event引用

### 5. 运行场景
点击Play按钮，使用WASD或方向键移动，自动射击

## 技术特点

### 空间网格优化
- 将500x500单位区域划分为50x50网格
- O(1)网格单元查找
- 只遍历玩家周围9个网格单元
- 从10,000敌人减少到约100个活跃敌人

### 事件驱动架构
- 使用ScriptableObject解耦系统
- 子弹生成通过事件通知
- 易于扩展新监听器

### 粒子系统设计
- Burst模式（瞬间爆发）
- 颜色/尺寸/透明度生命周期曲线
- 自动销毁（避免内存泄漏）

## 性能指标

- **敌人总数**: 10,000
- **同时激活**: ~100（基于100单位半径）
- **射击频率**: 10次/秒
- **预期帧率**: 60+ FPS（取决于硬件）

## 可扩展方向

- [ ] 对象池系统（减少Instantiate调用）
- [ ] 更多敌人AI行为
- [ ] 不同类型的子弹
- [ ] UI显示（击杀数、血量等）
- [ ] 音效系统
- [ ] 多种粒子特效变体
- [ ] 相机跟随系统（Cinemachine）

## 开发者备注

所有核心脚本使用C#编写，遵循Unity最佳实践。编辑器工具简化场景搭建流程，适合快速原型开发和学习参考。

查看 `Assets/Scripts/README.md` 获取详细的手动设置步骤。

---

## 最新更新 (2026-08-13)

### 新增功能

#### 1. **固定帧率射击** ⏱️
使用 FixedUpdate 确保射击频率恒定，不受帧率波动影响。

#### 2. **正交相机跟随** 📷
- 相机平滑跟随玩家移动
- 正交投影模式（无透视变形）
- 俯视视角，适合俯视射击游戏

#### 3. **敌人追逐玩家** 🏃
- 激活的敌人会缓慢追逐玩家
- 移动速度 2 单位/秒（慢于玩家）
- 增加游戏挑战性

#### 4. **受击红色闪烁** 💥
- 敌人受击瞬间变红
- 0.1秒后恢复原色
- 清晰的视觉反馈

### 快速设置

1. **更新 Enemy 预制体**
   打开 `Enemy.prefab`，新增参数已自动添加：
   - Chase Speed: 2
   - Flash Color: Red
   - Flash Duration: 0.1

2. **设置相机**
   使用编辑器工具：
   ```
   Tools > ParticleLab > Scene Setup > 8. Setup Camera Follow
   ```

3. **运行测试**
   按 Play，体验新功能！

详细说明请查看 [FEATURE_UPDATE.md](FEATURE_UPDATE.md)
