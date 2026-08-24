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

        if (GUILayout.Button("同步加载场景 LoadScene"))
        {
            SceneManager.LoadScene(sceneName);
        }
        if (GUILayout.Button("异步加载场景 LoadSceneAsync"))
        {
            SceneManager.LoadSceneAsync(sceneName);
        }
        if (GUILayout.Button("卸载场景"))
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }
        if (GUILayout.Button("重新加载当前场景"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
