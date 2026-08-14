using UnityEngine;
using UnityEditor;

public class SceneSetupTool : EditorWindow
{
    [MenuItem("Tools/ParticleLab/Scene Setup")]
    static void ShowWindow()
    {
        GetWindow<SceneSetupTool>("Scene Setup");
    }

    void OnGUI()
    {
        GUILayout.Label("ParticleLab Scene Setup", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (GUILayout.Button("1. Create Ground"))
        {
            CreateGround();
        }

        if (GUILayout.Button("2. Create Player"))
        {
            CreatePlayer();
        }

        if (GUILayout.Button("2.5. Create Spawn Area"))
        {
            CreateSpawnArea();
        }

        if (GUILayout.Button("3. Create Bullet Prefab Template"))
        {
            CreateBulletTemplate();
        }

        if (GUILayout.Button("4. Create Enemy Prefab Template"))
        {
            CreateEnemyTemplate();
        }

        if (GUILayout.Button("5. Create Hit Effect Template"))
        {
            CreateHitEffectTemplate();
        }

        if (GUILayout.Button("6. Create Death Effect Template"))
        {
            CreateDeathEffectTemplate();
        }

        if (GUILayout.Button("7. Create ScriptableObject Event"))
        {
            CreateGameEvent();
        }

        if (GUILayout.Button("8. Setup Camera Follow (Perspective)"))
        {
            SetupCamera();
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Use these buttons to quickly set up scene objects. Remember to:\n" +
            "- Create 'Enemy' layer in Tags & Layers\n" +
            "- Assign references in Inspector after creation\n" +
            "- Save prefabs in Assets/Prefabs folder", MessageType.Info);
    }

    void SetupCamera()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            mainCam = camObj.AddComponent<Camera>();
            camObj.tag = "MainCamera";
            camObj.AddComponent<AudioListener>();
        }

        CameraFollowPlayer followScript = mainCam.GetComponent<CameraFollowPlayer>();
        if (followScript == null)
        {
            followScript = mainCam.gameObject.AddComponent<CameraFollowPlayer>();
        }

        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            SerializedObject so = new SerializedObject(followScript);
            so.FindProperty("target").objectReferenceValue = player.transform;
            so.ApplyModifiedProperties();
        }

        mainCam.orthographic = false;
        mainCam.fieldOfView = 60f;
        mainCam.transform.position = new Vector3(0, 20, 0);
        mainCam.transform.rotation = Quaternion.Euler(90, 0, 0);

        Selection.activeGameObject = mainCam.gameObject;
        Debug.Log("Camera follow setup complete. Perspective mode enabled (FOV 60).");
    }

    /// <summary>
    /// 本项目是 URP，"Sprites/Default" 和 "Particles/Standard Unlit" 都是 Built-in RP 的
    /// shader，Shader.Find 会返回 null 并导致材质丢失（渲染成紫色）。这里按 URP 优先取。
    /// </summary>
    static Material CreateUnlitMaterial(Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit")
                     ?? Shader.Find("Universal Render Pipeline/Unlit")
                     ?? Shader.Find("Sprites/Default");

        if (shader == null)
        {
            Debug.LogError("[SceneSetupTool] 找不到可用的 unlit shader，材质会显示为紫色。");
            return null;
        }

        var mat = new Material(shader) { color = color };
        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
        return mat;
    }

    void CreateGround()
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(100, 1, 100);
        
        Selection.activeGameObject = ground;
        Debug.Log("Ground created at (0, 0, 0) with scale (100, 1, 100)");
    }

    void CreatePlayer()
    {
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Cube);
        player.name = "Player";
        player.transform.position = new Vector3(0, 0.5f, 0);

        player.AddComponent<PlayerController>();
        player.AddComponent<BulletDirectionSetter>();
        player.AddComponent<GameEventListener>();

        GameObject bulletSpawn = new GameObject("BulletSpawnPoint");
        bulletSpawn.transform.parent = player.transform;
        bulletSpawn.transform.localPosition = new Vector3(0, 0.5f, 0);

        Selection.activeGameObject = player;
        Debug.Log("Player created. Assign bullet prefab and event channel in Inspector.");
    }

    void CreateSpawnArea()
    {
        GameObject spawnArea = GameObject.CreatePrimitive(PrimitiveType.Plane);
        spawnArea.name = "SpawnArea";
        spawnArea.transform.position = new Vector3(0, 0.01f, 0);
        spawnArea.transform.localScale = new Vector3(50, 1, 50);

        var renderer = spawnArea.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = CreateUnlitMaterial(new Color(1f, 0f, 0f, 0.2f));
            if (mat != null)
            {
                if (mat.HasProperty("_Surface")) mat.SetFloat("_Surface", 1f);
                renderer.sharedMaterial = mat;
            }
        }

        var collider = spawnArea.GetComponent<Collider>();
        if (collider != null) DestroyImmediate(collider);

        Selection.activeGameObject = spawnArea;
        Debug.Log("SpawnArea created (red semi-transparent plane). Assign to EnemyManager.spawnArea.");
    }

    void CreateBulletTemplate()
    {
        GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bullet.name = "Bullet";
        bullet.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        Rigidbody rb = bullet.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        SphereCollider col = bullet.GetComponent<SphereCollider>();
        col.isTrigger = true;

        bullet.AddComponent<Bullet>();

        TrailRenderer trail = bullet.AddComponent<TrailRenderer>();
        trail.time = 0.3f;
        trail.startWidth = 0.1f;
        trail.endWidth = 0.01f;
        trail.material = CreateUnlitMaterial(Color.yellow);
        trail.startColor = Color.yellow;
        trail.endColor = Color.red;

        Selection.activeGameObject = bullet;
        Debug.Log("Bullet template created. Set Enemy layer mask and save as prefab.");
    }

    void CreateEnemyTemplate()
    {
        GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Cube);
        enemy.name = "Enemy";
        enemy.transform.localScale = Vector3.one;

        CapsuleCollider col = enemy.AddComponent<CapsuleCollider>();
        col.isTrigger = true;

        enemy.AddComponent<Enemy>();

        Selection.activeGameObject = enemy;
        Debug.Log("Enemy template created. Set layer to 'Enemy', assign effects, and save as prefab.");
    }

    void CreateHitEffectTemplate()
    {
        GameObject hitEffect = new GameObject("HitEffect");
        ParticleSystem ps = hitEffect.AddComponent<ParticleSystem>();
        
        var main = ps.main;
        main.startColor = new Color(1f, 0.6f, 0f, 1f);
        main.startSize = 0.2f;
        main.startSpeed = 3f;
        main.startLifetime = 0.5f;
        main.maxParticles = 20;
        main.loop = false;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, 20)
        });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.3f;

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.material = CreateUnlitMaterial(Color.white);

        Selection.activeGameObject = hitEffect;
        Debug.Log("Hit effect created. Save as prefab and assign to Enemy prefab.");
    }

    void CreateDeathEffectTemplate()
    {
        GameObject deathEffect = new GameObject("DeathEffect");
        ParticleSystem ps = deathEffect.AddComponent<ParticleSystem>();
        
        var main = ps.main;
        main.startColor = new Color(1f, 0.3f, 0f, 1f);
        main.startSize = 0.5f;
        main.startSpeed = 8f;
        main.startLifetime = 1f;
        main.maxParticles = 100;
        main.loop = false;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, 100)
        });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 1f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(Color.yellow, 0f),
                new GradientColorKey(Color.red, 0.5f),
                new GradientColorKey(Color.black, 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0.5f, 0.5f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        colorOverLifetime.color = gradient;

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0, 1, 1, 0));

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.material = CreateUnlitMaterial(Color.white);

        Selection.activeGameObject = deathEffect;
        Debug.Log("Death effect created. Save as prefab and assign to Enemy prefab.");
    }

    void CreateGameEvent()
    {
        string path = "Assets/ScriptableObjects";
        if (!AssetDatabase.IsValidFolder(path))
        {
            AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
        }

        GameEventSO asset = ScriptableObject.CreateInstance<GameEventSO>();
        AssetDatabase.CreateAsset(asset, path + "/OnBulletSpawnEvent.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;

        Debug.Log("GameEvent ScriptableObject created at " + path + "/OnBulletSpawnEvent.asset");
    }
}
