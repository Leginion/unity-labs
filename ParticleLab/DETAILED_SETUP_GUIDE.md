# 详细设置指南 - 步骤4和5

## 步骤4: 保存预制体 (Prefabs)

### 4.1 保存 Bullet 预制体
1. 在 Hierarchy 中选中 "Bullet" 对象
2. 拖拽到 Project 面板的 `Assets/Prefabs/` 文件夹
3. 松开鼠标，预制体创建完成
4. Hierarchy 中的 Bullet 对象文字会变成蓝色（表示已链接到预制体）
5. 现在可以删除 Hierarchy 中的 Bullet 对象（预制体已保存）

### 4.2 保存 Enemy 预制体
1. 在 Hierarchy 中选中 "Enemy" 对象
2. **重要**: 先设置 Layer
   - 在 Inspector 面板顶部找到 "Layer" 下拉框
   - 点击下拉框，选择 "Enemy" 层
   - 如果弹出对话框询问是否应用到子对象，点击 "Yes, change children"
3. 拖拽到 `Assets/Prefabs/` 文件夹，保存为预制体
4. 删除 Hierarchy 中的 Enemy 对象

### 4.3 保存 HitEffect 预制体
1. 在 Hierarchy 中选中 "HitEffect" 对象
2. 拖拽到 `Assets/Prefabs/` 文件夹
3. 保存完成后删除 Hierarchy 中的原对象

### 4.4 保存 DeathEffect 预制体
1. 在 Hierarchy 中选中 "DeathEffect" 对象
2. 拖拽到 `Assets/Prefabs/` 文件夹
3. 保存完成后删除 Hierarchy 中的原对象

**结果**: `Assets/Prefabs/` 文件夹应该有4个预制体文件：
- Bullet.prefab
- Enemy.prefab
- HitEffect.prefab
- DeathEffect.prefab

---

## 步骤5: 配置组件引用

### 5.1 配置 Player 对象

#### 在 Hierarchy 中选中 "Player" 对象

#### 5.1.1 配置 PlayerController 组件
在 Inspector 面板找到 "Player Controller" 组件：

**Move Speed**: 10 (默认值，可调整)
**Shoot Interval**: 0.1 (默认值，表示每0.1秒射击一次)

**Bullet Prefab**:
1. 点击右侧的圆形图标 (⊙)，或者
2. 从 Project 面板的 `Assets/Prefabs/` 拖拽 "Bullet" 预制体到这个字段

**Bullet Spawn Point**:
1. 点击右侧的圆形图标 (⊙)
2. 在弹出窗口中选择 "Scene" 标签
3. 找到并选择 "Player > BulletSpawnPoint" 子对象
   - 或者直接从 Hierarchy 中拖拽 "BulletSpawnPoint" 到这个字段

**On Bullet Spawn Event**:
1. 从 Project 面板的 `Assets/ScriptableObjects/`
2. 拖拽 "OnBulletSpawnEvent" 资源到这个字段

#### 5.1.2 配置 BulletDirectionSetter 组件
在 Inspector 面板找到 "Bullet Direction Setter" 组件：

**Player**:
1. 点击右侧的圆形图标 (⊙)
2. 选择 "Player" 对象本身
   - 或者从 Hierarchy 拖拽 "Player" 到这个字段

**Enemy Layer**:
1. 点击下拉框
2. 选中 "Enemy" 层（打勾）
3. 确保只勾选了 Enemy 层

**Detection Range**: 50 (默认值，可调整)

#### 5.1.3 配置 GameEventListener 组件
在 Inspector 面板找到 "Game Event Listener" 组件：

**Game Event**:
1. 从 Project 面板的 `Assets/ScriptableObjects/`
2. 拖拽 "OnBulletSpawnEvent" 到这个字段

**Response** (UnityEvent):
1. 点击 "Response" 下方的 "+" 按钮（添加事件监听）
2. 会出现一个新的事件槽
3. 将 "Player" 对象拖拽到左侧的对象字段（显示 "None (Object)"）
4. 点击右侧的函数下拉框（显示 "No Function"）
5. 选择: `BulletDirectionSetter > OnBulletSpawned (GameObject)`

**完整路径**: BulletDirectionSetter.OnBulletSpawned

---

### 5.2 配置 Bullet 预制体

#### 打开预制体编辑模式
1. 在 Project 面板双击 "Bullet" 预制体，或
2. 右键点击 > Open Prefab

#### 配置 Bullet 组件
在 Inspector 面板找到 "Bullet" 组件：

**Speed**: 20 (默认值)
**Lifetime**: 5 (默认值)

**Enemy Layer**:
1. 点击下拉框
2. 选中 "Enemy" 层（打勾）
3. 确保只勾选了 Enemy 层

#### 可选：配置 Trail Renderer
如果想要更好的视觉效果：
- Time: 0.3
- Start Width: 0.1
- End Width: 0.01
- Start Color: 黄色 (#FFFF00)
- End Color: 红色 (#FF0000)

完成后点击左上角的 "< " 返回场景

---

### 5.3 配置 Enemy 预制体

#### 打开预制体编辑模式
在 Project 面板双击 "Enemy" 预制体

#### 确认 Layer 设置
在 Inspector 顶部确认 Layer 已设置为 "Enemy"

#### 配置 Enemy 组件
在 Inspector 面板找到 "Enemy" 组件：

**Max Health**: 3 (默认值，表示3点生命值)

**Hit Effect Prefab**:
1. 从 Project 面板的 `Assets/Prefabs/`
2. 拖拽 "HitEffect" 预制体到这个字段

**Death Effect Prefab**:
1. 从 Project 面板的 `Assets/Prefabs/`
2. 拖拽 "DeathEffect" 预制体到这个字段

**On Enemy Death Event**:
1. （可选）如果需要死亡事件通知
2. 可以创建另一个 GameEventSO 并拖拽到这里
3. 不设置也能正常运行

完成后点击左上角的 "< " 返回场景

---

### 5.4 配置 EnemyManager

#### 创建 EnemyManager 对象
1. 在 Hierarchy 右键 > Create Empty
2. 重命名为 "EnemyManager"
3. 添加 "EnemyManager" 脚本组件
   - 在 Inspector 点击 "Add Component"
   - 搜索 "EnemyManager"
   - 点击添加

#### 配置 EnemyManager 组件
在 Inspector 面板找到 "Enemy Manager" 组件：

**Enemy Prefab**:
1. 从 Project 面板的 `Assets/Prefabs/`
2. 拖拽 "Enemy" 预制体到这个字段

**Enemy Count**: 10000
- 这会生成10,000个敌人
- 如果性能不足可以改成 5000 或更少

**Spawn Area Size**: 
- X: 500
- Y: 500
- 表示在 500x500 单位的区域内随机生成

**Activation Radius**: 100
- 只激活距离玩家 100 单位内的敌人
- 可以根据性能调整

**Player**:
1. 从 Hierarchy 拖拽 "Player" 对象到这个字段

**Grid Cell Size**: 50
- 空间网格的单元格大小
- 50x50 单位一个网格
- 优化查询性能

---

## 快速检查清单

### Player 对象配置 ✓
- [ ] PlayerController.Bullet Prefab = Bullet 预制体
- [ ] PlayerController.Bullet Spawn Point = BulletSpawnPoint 子对象
- [ ] PlayerController.On Bullet Spawn Event = OnBulletSpawnEvent
- [ ] BulletDirectionSetter.Player = Player 对象
- [ ] BulletDirectionSetter.Enemy Layer = Enemy (勾选)
- [ ] GameEventListener.Game Event = OnBulletSpawnEvent
- [ ] GameEventListener.Response = BulletDirectionSetter.OnBulletSpawned

### Bullet 预制体配置 ✓
- [ ] Bullet.Enemy Layer = Enemy (勾选)

### Enemy 预制体配置 ✓
- [ ] Layer = Enemy
- [ ] Enemy.Hit Effect Prefab = HitEffect 预制体
- [ ] Enemy.Death Effect Prefab = DeathEffect 预制体

### EnemyManager 配置 ✓
- [ ] Enemy Prefab = Enemy 预制体
- [ ] Player = Player 对象
- [ ] Enemy Count = 10000
- [ ] Spawn Area Size = (500, 500)
- [ ] Activation Radius = 100
- [ ] Grid Cell Size = 50

---

## 常见问题

### Q: 找不到 "Enemy" 层？
A: 需要先创建层级：
1. Edit > Project Settings > Tags and Layers
2. 找到 "Layers" 部分
3. 在空白的 User Layer 中输入 "Enemy"

### Q: GameEventListener 的 Response 找不到函数？
A: 确保：
1. 对象槽拖入了 Player 对象（不是预制体）
2. Player 上挂载了 BulletDirectionSetter 组件
3. 下拉菜单选择路径: BulletDirectionSetter > OnBulletSpawned

### Q: 子弹不会射向敌人？
A: 检查：
1. Bullet 预制体的 Enemy Layer 是否勾选了 Enemy
2. BulletDirectionSetter.Enemy Layer 是否勾选了 Enemy
3. Enemy 预制体的 Layer 是否设置为 Enemy
4. GameEventListener 的 Response 是否正确配置

### Q: 敌人没有特效？
A: 检查：
1. Enemy 预制体是否分配了 HitEffect 和 DeathEffect
2. 特效预制体是否正确保存
3. 特效预制体的 Particle System 是否配置正确

### Q: 场景中没有敌人出现？
A: 检查：
1. EnemyManager 是否在场景中（Hierarchy）
2. EnemyManager.Enemy Prefab 是否分配
3. EnemyManager.Player 是否分配
4. Console 是否有错误信息

---

## 完成后测试

按下 Play 按钮，应该看到：
1. ✓ 地面上有一个方块（Player）
2. ✓ WASD 可以移动
3. ✓ 每秒自动射出 10 个子弹（每 0.1 秒一个）
4. ✓ 子弹朝最近的敌人飞行
5. ✓ 靠近玩家的敌人变成红色（激活状态，如果有材质）
6. ✓ 子弹击中敌人时播放橙色爆炸特效
7. ✓ 敌人死亡时播放大型爆炸特效（黄→红→黑）
8. ✓ 死亡敌人在随机位置重生

如果一切正常，恭喜！场景设置完成！🎉
