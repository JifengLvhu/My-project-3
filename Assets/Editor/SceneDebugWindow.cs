using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class SceneDebugWindow : EditorWindow
{
    private string sceneName = "";

    [MenuItem("调试/场景调试工具")]
    static void OpenWindow()
    {
        GetWindow<SceneDebugWindow>("场景调试");
    }

    void OnGUI()
    {
        sceneName = EditorGUILayout.TextField("场景名称", sceneName);

        if (GUILayout.Button("加载场景 LoadSceneAsync"))
        {
            if (GameManager.Instance != null)
                GameManager.Instance.加载场景(sceneName);
            else
                Debug.LogError("GameManager 不存在");
        }

        if (GUILayout.Button("卸载场景"))
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }

        if (GUILayout.Button("重新加载当前场景"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("触发全部史莱姆分裂"))
        {
            if (!EditorApplication.isPlaying)
            {
                Debug.LogWarning("请进入运行模式再执行该功能！");
                return;
            }
            史莱姆[] allSlimes = Object.FindObjectsOfType<史莱姆>();
            int count = 0;
            foreach (var slime in allSlimes)
            {
                slime.分裂();
                count++;
            }
            Debug.Log($"已触发 {count} 个史莱姆执行分裂");
        }

        // ==========新增：归还全部史莱姆到对象池，保留最后1个 ==========
        if (GUILayout.Button("归还史莱姆到对象池(保留最后1个)"))
        {
            if (!EditorApplication.isPlaying)
            {
                Debug.LogWarning("请进入运行模式再执行该功能！");
                return;
            }

            史莱姆[] allSlimes = Object.FindObjectsOfType<史莱姆>();
            if (allSlimes.Length <= 1)
            {
                Debug.Log($"当前场景史莱姆数量:{allSlimes.Length}，无需归还");
                return;
            }

            // 跳过最后一个，前面全部归还对象池
            int returnCount = 0;
            for (int i = 0; i < allSlimes.Length - 1; i++)
            {
                var slime = allSlimes[i];
                if (slime.史莱姆预制体 != null)
                {
                    对象池.Instance.归还对象("史莱姆", slime.gameObject);
                    returnCount++;
                }
            }
            Debug.Log($"归还 {returnCount} 个史莱姆，保留1个存活");
        }
    }
}
