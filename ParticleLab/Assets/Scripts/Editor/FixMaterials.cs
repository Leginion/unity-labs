using UnityEngine;
using UnityEditor;

/// <summary>
/// 修复 URP 下的紫色（shader 丢失）材质。
/// 起因：SceneSetupTool 用 Shader.Find("Sprites/Default") / ("Particles/Standard Unlit")
/// 创建材质，这两个都是 Built-in RP 的 shader，在 URP 项目里 Find 返回 null，
/// 于是材质没被序列化，prefab 里留下 m_Materials: [{fileID: 0}]。
/// </summary>
public static class FixMaterials
{
    const string MatDir = "Assets/Materials";
    const string PrefabDir = "Assets/Prefabs/";

    [MenuItem("Tools/ParticleLab/Fix Magenta Materials (URP)")]
    static void FixAll()
    {
        if (!AssetDatabase.IsValidFolder(MatDir))
            AssetDatabase.CreateFolder("Assets", "Materials");

        var trailMat = CreateOrLoad("BulletTrail", "Universal Render Pipeline/Particles/Unlit",
            new Color(1f, 0.85f, 0.2f, 1f));
        var particleMat = CreateOrLoad("ParticleAdditive", "Universal Render Pipeline/Particles/Unlit",
            Color.white);

        if (trailMat == null || particleMat == null)
        {
            Debug.LogError("[FixMaterials] 无法创建材质，URP 粒子 shader 未找到。请确认项目使用 URP。");
            return;
        }

        AssignTrail(PrefabDir + "Bullet.prefab", trailMat);
        AssignParticle(PrefabDir + "HitEffect.prefab", particleMat);
        AssignParticle(PrefabDir + "DeathEffect.prefab", particleMat);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[FixMaterials] 完成。材质已保存到 " + MatDir);
    }

    static Material CreateOrLoad(string name, string shaderName, Color color)
    {
        string path = MatDir + "/" + name + ".mat";
        var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existing != null && existing.shader != null && existing.shader.name != "Hidden/InternalErrorShader")
            return existing;

        var shader = Shader.Find(shaderName);
        if (shader == null)
        {
            // 逐级回退，覆盖不同 URP 版本的 shader 命名
            shader = Shader.Find("Universal Render Pipeline/Unlit")
                  ?? Shader.Find("Sprites/Default");
        }
        if (shader == null) return null;

        var mat = new Material(shader);
        mat.color = color;
        // 加色混合，让拖尾和爆炸有发光感
        if (mat.HasProperty("_Surface")) mat.SetFloat("_Surface", 1f);   // Transparent
        if (mat.HasProperty("_Blend")) mat.SetFloat("_Blend", 1f);       // Additive
        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);

        if (existing != null)
        {
            existing.shader = shader;
            existing.color = color;
            EditorUtility.SetDirty(existing);
            return existing;
        }

        AssetDatabase.CreateAsset(mat, path);
        Debug.Log("[FixMaterials] 创建材质 " + path + " (shader: " + shader.name + ")");
        return mat;
    }

    static void AssignTrail(string prefabPath, Material mat)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null) { Debug.LogWarning("[FixMaterials] 找不到 " + prefabPath); return; }

        var trail = prefab.GetComponent<TrailRenderer>();
        if (trail == null) { Debug.LogWarning("[FixMaterials] " + prefabPath + " 无 TrailRenderer"); return; }

        trail.sharedMaterial = mat;
        PrefabUtility.SavePrefabAsset(prefab);
        Debug.Log("[FixMaterials] " + prefabPath + " TrailRenderer 材质已赋值");
    }

    static void AssignParticle(string prefabPath, Material mat)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null) { Debug.LogWarning("[FixMaterials] 找不到 " + prefabPath); return; }

        var psr = prefab.GetComponent<ParticleSystemRenderer>();
        if (psr == null) { Debug.LogWarning("[FixMaterials] " + prefabPath + " 无 ParticleSystemRenderer"); return; }

        psr.sharedMaterial = mat;
        PrefabUtility.SavePrefabAsset(prefab);
        Debug.Log("[FixMaterials] " + prefabPath + " ParticleSystemRenderer 材质已赋值");
    }
}
