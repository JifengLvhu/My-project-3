using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class 暂停菜单管理器 : MonoBehaviour
{
    public static 暂停菜单管理器 Instance{ get; private set; }

    public Button 返回游戏按钮;
    public Button 读取存档按钮;
    public Button 设置页面按钮;
    public Button 返回主页按钮;

    private float 当前时间缩放;

    void Awake()
    {
        if(Instance == null)
        {   
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        当前时间缩放  = Time.timeScale;

        返回游戏按钮.onClick.AddListener(() => 返回游戏());
        读取存档按钮.onClick.AddListener(() => 打开读档界面());
        设置页面按钮.onClick.AddListener(() => 打开设置界面());
        返回主页按钮.onClick.AddListener(() => 返回主页());

        Time.timeScale = 0f;
    }

    private void 返回游戏()
    {
        Time.timeScale = 当前时间缩放;

        SceneManager.UnloadSceneAsync("Pause");
    }

    private void 打开读档界面()
    {
        SceneManager.LoadScene("Save", LoadSceneMode.Additive);
    }

    private void 打开设置界面()
    {
        SceneManager.LoadScene("Settings", LoadSceneMode.Additive);
    }

    private void 返回主页()
    {
        Time.timeScale = 当前时间缩放;

        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
}
