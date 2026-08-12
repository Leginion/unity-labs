using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ConfigurationChecker : EditorWindow
{
    private Vector2 scrollPosition;
    private List<string> errors = new List<string>();
    private List<string> warnings = new List<string>();
    private List<string> success = new List<string>();

    [MenuItem("Tools/ParticleLab/Check Configuration")]
    static void ShowWindow()
    {
        var window = GetWindow<ConfigurationChecker>("Configuration Checker");
        window.minSize = new Vector2(400, 500);
        window.CheckConfiguration();
    }

    void OnGUI()
    {
        GUILayout.Label("ParticleLab Configuration Checker", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (GUILayout.Button("Re-check Configuration", GUILayout.Height(30)))
        {
            CheckConfiguration();
        }

        EditorGUILayout.Space();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        if (errors.Count > 0)
        {
            GUILayout.Label("ERRORS (" + errors.Count + ")", EditorStyles.boldLabel);
            foreach (string error in errors)
            {
                EditorGUILayout.HelpBox(error, MessageType.Error);
            }
            EditorGUILayout.Space();
        }

        if (warnings.Count > 0)
        {
            GUILayout.Label("WARNINGS (" + warnings.Count + ")", EditorStyles.boldLabel);
            foreach (string warning in warnings)
            {
                EditorGUILayout.HelpBox(warning, MessageType.Warning);
            }
            EditorGUILayout.Space();
        }

        if (success.Count > 0)
        {
            GUILayout.Label("SUCCESS (" + success.Count + ")", EditorStyles.boldLabel);
            foreach (string s in success)
            {
                EditorGUILayout.HelpBox(s, MessageType.Info);
            }
        }

        EditorGUILayout.Space();
        if (errors.Count == 0 && warnings.Count == 0)
        {
            EditorGUILayout.HelpBox("All checks passed! Ready to play.", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox(
                "Found " + errors.Count + " errors, " + warnings.Count + " warnings.",
                MessageType.Warning
            );
        }

        EditorGUILayout.EndScrollView();
    }

    void CheckConfiguration()
    {
        errors.Clear();
        warnings.Clear();
        success.Clear();

        Debug.Log("Starting configuration check...");

        CheckLayers();
        CheckPrefabs();
        CheckScriptableObjects();
        CheckSceneObjects();

        Repaint();
    }

    void CheckLayers()
    {
        bool enemyLayerExists = false;
        for (int i = 0; i < 32; i++)
        {
            string layerName = LayerMask.LayerToName(i);
            if (layerName == "Enemy")
            {
                enemyLayerExists = true;
                success.Add("[Layer] 'Enemy' layer exists (Layer " + i + ")");
                break;
            }
        }

        if (!enemyLayerExists)
        {
            errors.Add("[Layer] Missing 'Enemy' layer. Add it in Project Settings > Tags and Layers.");
        }
    }

    void CheckPrefabs()
    {
        GameObject bulletPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Bullet.prefab");
        if (bulletPrefab != null)
        {
            success.Add("[Prefab] Bullet.prefab exists");

            Bullet bullet = bulletPrefab.GetComponent<Bullet>();
            if (bullet == null)
            {
                errors.Add("[Bullet Prefab] Missing Bullet component");
            }
            else
            {
                success.Add("[Bullet Prefab] Bullet component attached");
            }
        }
        else
        {
            warnings.Add("[Prefab] Bullet.prefab not found in Assets/Prefabs/");
        }

        GameObject enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Enemy.prefab");
        if (enemyPrefab != null)
        {
            success.Add("[Prefab] Enemy.prefab exists");

            if (enemyPrefab.layer != LayerMask.NameToLayer("Enemy"))
            {
                errors.Add("[Enemy Prefab] Layer not set to 'Enemy'");
            }
            else
            {
                success.Add("[Enemy Prefab] Layer correctly set to 'Enemy'");
            }

            Enemy enemy = enemyPrefab.GetComponent<Enemy>();
            if (enemy == null)
            {
                errors.Add("[Enemy Prefab] Missing Enemy component");
            }
            else
            {
                success.Add("[Enemy Prefab] Enemy component attached");
            }
        }
        else
        {
            warnings.Add("[Prefab] Enemy.prefab not found in Assets/Prefabs/");
        }

        GameObject hitEffect = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/HitEffect.prefab");
        if (hitEffect != null)
        {
            success.Add("[Prefab] HitEffect.prefab exists");
        }
        else
        {
            warnings.Add("[Prefab] HitEffect.prefab not found");
        }

        GameObject deathEffect = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/DeathEffect.prefab");
        if (deathEffect != null)
        {
            success.Add("[Prefab] DeathEffect.prefab exists");
        }
        else
        {
            warnings.Add("[Prefab] DeathEffect.prefab not found");
        }
    }

    void CheckScriptableObjects()
    {
        GameEventSO eventSO = AssetDatabase.LoadAssetAtPath<GameEventSO>("Assets/ScriptableObjects/OnBulletSpawnEvent.asset");
        if (eventSO != null)
        {
            success.Add("[ScriptableObject] OnBulletSpawnEvent.asset exists");
        }
        else
        {
            warnings.Add("[ScriptableObject] OnBulletSpawnEvent.asset not found");
        }
    }

    void CheckSceneObjects()
    {
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            success.Add("[Scene] Player object exists");
            CheckPlayerComponents(player);
        }
        else
        {
            errors.Add("[Scene] Missing 'Player' object in scene");
        }

        EnemyManager enemyManager = Object.FindObjectOfType<EnemyManager>();
        if (enemyManager != null)
        {
            success.Add("[Scene] EnemyManager object exists");
        }
        else
        {
            errors.Add("[Scene] Missing EnemyManager object in scene");
        }

        GameObject ground = GameObject.Find("Ground");
        if (ground != null)
        {
            success.Add("[Scene] Ground object exists");
        }
        else
        {
            warnings.Add("[Scene] Missing 'Ground' object in scene");
        }
    }

    void CheckPlayerComponents(GameObject player)
    {
        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc == null)
        {
            errors.Add("[Player] Missing PlayerController component");
        }
        else
        {
            success.Add("[Player] PlayerController component exists");
        }

        BulletDirectionSetter bds = player.GetComponent<BulletDirectionSetter>();
        if (bds == null)
        {
            errors.Add("[Player] Missing BulletDirectionSetter component");
        }
        else
        {
            success.Add("[Player] BulletDirectionSetter component exists");
        }

        GameEventListener gel = player.GetComponent<GameEventListener>();
        if (gel == null)
        {
            errors.Add("[Player] Missing GameEventListener component");
        }
        else
        {
            success.Add("[Player] GameEventListener component exists");
        }

        Transform spawnPoint = player.transform.Find("BulletSpawnPoint");
        if (spawnPoint == null)
        {
            errors.Add("[Player] Missing BulletSpawnPoint child object");
        }
        else
        {
            success.Add("[Player] BulletSpawnPoint child object exists");
        }
    }
}
