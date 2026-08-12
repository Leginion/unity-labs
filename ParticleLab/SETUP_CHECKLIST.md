# ParticleLab 场景设置检查清单

## 脚本文件 ✅ (已完成)
- [x] PlayerController.cs - 玩家移动和自动射击
- [x] Bullet.cs - 子弹行为
- [x] BulletDirectionSetter.cs - 智能瞄准
- [x] Enemy.cs - 敌人生命和特效
- [x] EnemyManager.cs - 10k敌人管理和空间网格
- [x] GameEventSO.cs - ScriptableObject事件通道
- [x] GameEventListener.cs - 事件监听器
- [x] ParticleEffectConfig.cs - 粒子配置
- [x] SceneSetupTool.cs - Unity编辑器工具

## Unity中需要完成的步骤

### 1. 创建层级 (Layers)
- [ ] Project Settings > Tags and Layers
- [ ] 添加层级: "Enemy"

### 2. 使用编辑器工具创建对象
打开: `Tools > ParticleLab > Scene Setup`

- [ ] 点击 "1. Create Ground"
- [ ] 点击 "2. Create Player"
- [ ] 点击 "3. Create Bullet Prefab Template"
- [ ] 点击 "4. Create Enemy Prefab Template"
- [ ] 点击 "5. Create Hit Effect Template"
- [ ] 点击 "6. Create Death Effect Template"
- [ ] 点击 "7. Create ScriptableObject Event"

### 3. 保存预制体
- [ ] Bullet → 拖到 `Assets/Prefabs/` 成为预制体
- [ ] Enemy → 设置Layer为"Enemy" → 拖到 `Assets/Prefabs/`
- [ ] HitEffect → 拖到 `Assets/Prefabs/`
- [ ] DeathEffect → 拖到 `Assets/Prefabs/`

### 4. 配置Player组件
选中Player对象，在Inspector中设置：

**PlayerController**
- [ ] Bullet Prefab → 拖入Bullet预制体
- [ ] Bullet Spawn Point → 拖入Player下的BulletSpawnPoint子对象

**BulletDirectionSetter**
- [ ] Player → 拖入Player自身
- [ ] Enemy Layer → 选择"Enemy"层
- [ ] Detection Range → 50

**GameEventListener**
- [ ] Game Event → 拖入OnBulletSpawnEvent
- [ ] Response → 添加事件 → 选择BulletDirectionSetter.OnBulletSpawned

### 5. 配置Bullet预制体
打开Bullet预制体编辑：
- [ ] Bullet组件 > Enemy Layer → 选择"Enemy"

### 6. 配置Enemy预制体
打开Enemy预制体编辑：
- [ ] 确认Layer设置为"Enemy"
- [ ] Enemy组件 > Hit Effect Prefab → 拖入HitEffect预制体
- [ ] Enemy组件 > Death Effect Prefab → 拖入DeathEffect预制体

### 7. 创建EnemyManager
- [ ] Hierarchy > Create Empty GameObject (命名: EnemyManager)
- [ ] 添加EnemyManager脚本
- [ ] Enemy Prefab → 拖入Enemy预制体
- [ ] Player → 拖入Player对象
- [ ] Enemy Count → 10000
- [ ] Spawn Area Size → (500, 500)
- [ ] Activation Radius → 100
- [ ] Grid Cell Size → 50

### 8. 相机设置
- [ ] 调整Main Camera位置到合适俯视角度
- [ ] 建议位置: (0, 50, -30)
- [ ] 建议旋转: (60, 0, 0)

### 9. 测试运行
- [ ] 点击Play按钮
- [ ] 测试WASD移动
- [ ] 测试方向键移动
- [ ] 验证自动射击（每0.1秒）
- [ ] 验证子弹追踪最近敌人
- [ ] 验证敌人受击特效
- [ ] 验证敌人死亡特效
- [ ] 验证敌人重生

## 功能验证

- [ ] 玩家移动流畅
- [ ] 子弹自动发射（10次/秒）
- [ ] 子弹追踪最近敌人
- [ ] 敌人被击中播放橙色小爆炸
- [ ] 敌人死亡播放大型爆炸（黄→红→黑渐变）
- [ ] 死亡敌人在随机位置重生
- [ ] 只有玩家附近100单位内的敌人激活
- [ ] 帧率稳定（60+ FPS）

## 性能优化建议

如果遇到性能问题：
- [ ] 减少Enemy Count到5000或更少
- [ ] 减少Activation Radius到50
- [ ] 增加Grid Cell Size到100
- [ ] 减少粒子数量（Hit: 10, Death: 50）
- [ ] 关闭不必要的粒子效果模块

## 可选增强

- [ ] 添加对象池管理子弹
- [ ] 添加UI显示击杀数
- [ ] 添加音效
- [ ] 使用Cinemachine相机跟随
- [ ] 添加敌人AI移动
- [ ] 添加不同类型武器

---

**提示**: 详细手动设置步骤请查看 `Assets/Scripts/README.md`
