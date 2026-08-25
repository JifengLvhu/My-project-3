using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.UI;


public class 主页面管理器 : MonoBehaviour
{
    public Button 开始新游戏按钮;
    public Button 继续游戏按钮;
    public Button 设置按钮;
    public Button 退出游戏按钮;

    void Start()
    {
        开始新游戏按钮.onClick.AddListener(开始新游戏);
        继续游戏按钮.onClick.AddListener(继续游戏);
        设置按钮.onClick.AddListener(设置);
        退出游戏按钮.onClick.AddListener(退出游戏);

    }

    public void 开始新游戏()
    {
        GameManager.Instance.加载场景("Part1");
    }

    public void 继续游戏()
    {
        存档管理器.Instance.打开存档场景(false);
    }

    public void 设置()
    {
        SceneManager.LoadScene("settings", LoadSceneMode.Additive);
    }

    public void 退出游戏()
    {

        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
