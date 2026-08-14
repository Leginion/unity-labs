using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class RemoveCameraFollow
{
    [MenuItem("Tools/ParticleLab/Remove Camera Follow")]
    static void Remove()
    {
        var cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("[RemoveCameraFollow] 找不到 Main Camera");
            return;
        }

        var follow = cam.GetComponent<CameraFollowPlayer>();
        if (follow != null)
        {
            Object.DestroyImmediate(follow);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            EditorSceneManager.SaveOpenScenes();
            Debug.Log("[RemoveCameraFollow] 已从 Main Camera 移除 CameraFollowPlayer 组件并保存场景");
        }
        else
        {
            Debug.Log("[RemoveCameraFollow] Main Camera 上没有 CameraFollowPlayer 组件");
        }
    }
}
