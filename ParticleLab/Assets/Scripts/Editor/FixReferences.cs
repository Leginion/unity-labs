using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// 一键修复预制体引用并重建场景对象。
/// 解决：Bullet 的 enemyLayer 为空、Enemy 的特效引用为空、场景缺少 Player/Ground/EnemyManager。
/// </summary>
public static class FixReferences
{
    const string PrefabDir = "Assets/Prefabs/";
    const string EventAsset = "Assets/ScriptableObjects/OnBulletSpawnEvent.asset";

    [MenuItem("Tools/ParticleLab/Fix All References + Rebuild Scene")]
    static void FixAll()
    {
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        if (enemyLayer < 0)
        {
            Debug.LogError("[FixReferences] 找不到 'Enemy' 层，请先在 Tags and Layers 中创建。");
            return;
        }

        var bulletPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabDir + "Bullet.prefab");
        var enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabDir + "Enemy.prefab");
        var hitEffect = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabDir + "HitEffect.prefab");
        var deathEffect = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabDir + "DeathEffect.prefab");
        var spawnEvent = AssetDatabase.LoadAssetAtPath<GameEventSO>(EventAsset);

        if (bulletPrefab == null || enemyPrefab == null)
        {
            Debug.LogError("[FixReferences] 缺少 Bullet.prefab 或 Enemy.prefab。");
            return;
        }

        FixBulletPrefab(bulletPrefab, enemyLayer);
        FixEnemyPrefab(enemyPrefab, enemyLayer, hitEffect, deathEffect);
        RebuildScene(bulletPrefab, enemyPrefab, spawnEvent, enemyLayer);

        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();

        Debug.Log("[FixReferences] 全部修复完成，场景已保存。");
    }

    static void FixBulletPrefab(GameObject prefab, int enemyLayer)
    {
        var bullet = prefab.GetComponent<Bullet>();
        if (bullet == null)
        {
            Debug.LogError("[FixReferences] Bullet.prefab 缺少 Bullet 组件。");
            return;
        }

        var so = new SerializedObject(bullet);
        // LayerMask 只勾选 Enemy 层；勾成 Everything 会让子弹一出生就撞到玩家并自毁
        so.FindProperty("enemyLayer").intValue = 1 << enemyLayer;
        so.ApplyModifiedPropertiesWithoutUndo();

        PrefabUtility.SavePrefabAsset(prefab);
        Debug.Log("[FixReferences] Bullet.enemyLayer -> Enemy (bits " + (1 << enemyLayer) + ")");
    }

    static void FixEnemyPrefab(GameObject prefab, int enemyLayer, GameObject hit, GameObject death)
    {
        if (prefab.layer != enemyLayer)
        {
            prefab.layer = enemyLayer;
            Debug.Log("[FixReferences] Enemy.prefab layer -> Enemy");
        }

        var enemy = prefab.GetComponent<Enemy>();
        if (enemy == null)
        {
            Debug.LogError("[FixReferences] Enemy.prefab 缺少 Enemy 组件。");
            return;
        }

        var so = new SerializedObject(enemy);
        if (hit != null) so.FindProperty("hitEffectPrefab").objectReferenceValue = hit;
        if (death != null) so.FindProperty("deathEffectPrefab").objectReferenceValue = death;
        so.FindProperty("chaseSpeed").floatValue = 2f;
        so.FindProperty("flashColor").colorValue = Color.red;
        so.FindProperty("flashDuration").floatValue = 0.1f;
        so.ApplyModifiedPropertiesWithoutUndo();

        // 图元自带的实体 BoxCollider 与新增的 trigger CapsuleCollider 会重复占位，移除前者
        var box = prefab.GetComponent<BoxCollider>();
        if (box != null)
        {
            Object.DestroyImmediate(box, true);
            Debug.Log("[FixReferences] 移除 Enemy.prefab 上重复的 BoxCollider");
        }

        PrefabUtility.SavePrefabAsset(prefab);
        Debug.Log("[FixReferences] Enemy 特效引用与新参数已写入");
    }

    static void RebuildScene(GameObject bulletPrefab, GameObject enemyPrefab, GameEventSO spawnEvent, int enemyLayer)
    {
        // Ground
        var ground = GameObject.Find("Ground");
        if (ground == null)
        {
            ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.localScale = new Vector3(100, 1, 100);
            Debug.Log("[FixReferences] 创建 Ground");
        }

        // SpawnArea（可视化刷怪区域）
        var spawnArea = GameObject.Find("SpawnArea");
        if (spawnArea == null)
        {
            spawnArea = GameObject.CreatePrimitive(PrimitiveType.Plane);
            spawnArea.name = "SpawnArea";
            spawnArea.transform.position = new Vector3(0, 0.01f, 0); // 略高于地面，避免 Z-fighting
            spawnArea.transform.localScale = new Vector3(50, 1, 50);  // 500x500 单位区域（Plane 默认 10x10）

            // 半透明材质，便于在编辑器中看到刷怪范围
            var renderer = spawnArea.GetComponent<Renderer>();
            if (renderer != null)
            {
                var mat = new Material(Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Sprites/Default"));
                mat.color = new Color(1f, 0f, 0f, 0.2f);
                if (mat.HasProperty("_Surface")) mat.SetFloat("_Surface", 1f); // Transparent
                if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", new Color(1f, 0f, 0f, 0.2f));
                renderer.sharedMaterial = mat;
            }

            // 移除碰撞体，这只是可视化标记
            var collider = spawnArea.GetComponent<Collider>();
            if (collider != null) Object.DestroyImmediate(collider);

            Debug.Log("[FixReferences] 创建 SpawnArea (红色半透明平面，scale 50 = 500x500 区域)");
        }

        // Player
        var player = GameObject.Find("Player");
        if (player == null)
        {
            player = GameObject.CreatePrimitive(PrimitiveType.Cube);
            player.name = "Player";
            player.transform.position = new Vector3(0, 0.5f, 0);
            Debug.Log("[FixReferences] 创建 Player");
        }

        var spawnPoint = player.transform.Find("BulletSpawnPoint");
        if (spawnPoint == null)
        {
            var sp = new GameObject("BulletSpawnPoint");
            sp.transform.SetParent(player.transform);
            sp.transform.localPosition = new Vector3(0, 0.5f, 0);
            spawnPoint = sp.transform;
        }

        var pc = player.GetComponent<PlayerController>() ?? player.AddComponent<PlayerController>();
        var pcSO = new SerializedObject(pc);
        pcSO.FindProperty("bulletPrefab").objectReferenceValue = bulletPrefab;
        pcSO.FindProperty("bulletSpawnPoint").objectReferenceValue = spawnPoint;
        pcSO.FindProperty("onBulletSpawnEvent").objectReferenceValue = spawnEvent;
        pcSO.ApplyModifiedPropertiesWithoutUndo();

        var bds = player.GetComponent<BulletDirectionSetter>() ?? player.AddComponent<BulletDirectionSetter>();
        var bdsSO = new SerializedObject(bds);
        bdsSO.FindProperty("player").objectReferenceValue = player.transform;
        bdsSO.FindProperty("enemyLayer").intValue = 1 << enemyLayer;
        bdsSO.ApplyModifiedPropertiesWithoutUndo();

        // GameEventListener: 事件通道 + 指向 BulletDirectionSetter.OnBulletSpawned 的持久化回调
        var gel = player.GetComponent<GameEventListener>() ?? player.AddComponent<GameEventListener>();
        var gelSO = new SerializedObject(gel);
        gelSO.FindProperty("gameEvent").objectReferenceValue = spawnEvent;

        var calls = gelSO.FindProperty("response.m_PersistentCalls.m_Calls");
        calls.ClearArray();
        calls.InsertArrayElementAtIndex(0);
        var call = calls.GetArrayElementAtIndex(0);
        call.FindPropertyRelative("m_Target").objectReferenceValue = bds;
        call.FindPropertyRelative("m_TargetAssemblyTypeName").stringValue = typeof(BulletDirectionSetter).AssemblyQualifiedName;
        call.FindPropertyRelative("m_MethodName").stringValue = "OnBulletSpawned";
        // 必须是 EventDefined(0)：让回调接收 Raise() 传入的运行时参数。
        // 用 Object(2) 会让 Unity 改用序列化的固定实参，并以 UnityEngine.Object 去匹配
        // OnBulletSpawned(GameObject)，从而抛 ArgumentException。
        call.FindPropertyRelative("m_Mode").enumValueIndex = 0;
        call.FindPropertyRelative("m_CallState").enumValueIndex = 2; // RuntimeOnly
        var args = call.FindPropertyRelative("m_Arguments");
        args.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName").stringValue = typeof(GameObject).AssemblyQualifiedName;
        gelSO.ApplyModifiedPropertiesWithoutUndo();

        // EnemyManager
        var em = Object.FindObjectOfType<EnemyManager>();
        if (em == null)
        {
            em = new GameObject("EnemyManager").AddComponent<EnemyManager>();
            Debug.Log("[FixReferences] 创建 EnemyManager");
        }
        var emSO = new SerializedObject(em);
        emSO.FindProperty("enemyPrefab").objectReferenceValue = enemyPrefab;
        emSO.FindProperty("spawnArea").objectReferenceValue = spawnArea.transform;
        emSO.FindProperty("player").objectReferenceValue = player.transform;
        emSO.ApplyModifiedPropertiesWithoutUndo();

        // Camera（固定视角，不跟随玩家）
        var cam = Camera.main;
        if (cam != null)
        {
            // 移除可能已存在的 CameraFollowPlayer 组件
            var oldFollow = cam.GetComponent<CameraFollowPlayer>();
            if (oldFollow != null)
            {
                Object.DestroyImmediate(oldFollow);
                Debug.Log("[FixReferences] 已移除 Main Camera 上的 CameraFollowPlayer 组件");
            }

            cam.orthographic = false;
            cam.fieldOfView = 60f;
            cam.transform.position = new Vector3(0, 20, 0);
            cam.transform.rotation = Quaternion.Euler(90, 0, 0);
        }

        Debug.Log("[FixReferences] 场景对象与引用已重建");
    }
}
