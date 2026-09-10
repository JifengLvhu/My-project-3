using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.IO;

public class 存档页面管理器 : MonoBehaviour
{
    public static 存档页面管理器 Instance { get; private set; }

    [Header("UI 引用")]
    public GameObject 存档界面面板;
    public TMP_Text 模式提示文本;
    public Button 关闭按钮;
    public 存档位UI[] 所有存档格子;
    public Sprite 默认缩略图 = null;

    private bool 当前为存档模式 = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("初始化存档界面管理器");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        GameManager.Instance.暂停游戏();
        关闭按钮.onClick.AddListener(() => 关闭界面());
    }

    public void 初始化界面(bool 模式)
    {
        当前为存档模式 = 模式;

        模式提示文本.text = 模式 ? "存储" : "读取";

        刷新所有格子();
    }

    private void 刷新所有格子()
    {
        foreach(var 格子 in 所有存档格子)
        {
            int 存档编号 = 格子.档位编号;

            存档管理器.总存档数据 数据 = 存档管理器.Instance.读取存档(存档编号);

            if(数据 != null)
            {
                格子.章节名文本.text = 数据.当前章节;
                格子.保存时间文本.text = 数据.存档时间;

                if(!string.IsNullOrEmpty(数据.截图路径))
                {
                    string 完整路径 = Path.Combine(Application.persistentDataPath, 数据.截图路径);

                    if(File.Exists(完整路径))
                    {
                        byte[] 图片字节 = File.ReadAllBytes(完整路径);

                        Texture2D 纹理 = new Texture2D(2, 2);
                        纹理.LoadImage(图片字节);

                        格子.缩略图.sprite = Sprite.Create(纹理, new Rect(0, 0, 纹理.width, 纹理.height), new Vector2(0.5f, 0.5f));
                    }
                    else
                    {
                        格子.缩略图.sprite = 默认缩略图;
                    }
                }
                else
                {
                    格子.缩略图.sprite = 默认缩略图;
                }

                格子.按钮组件.onClick.RemoveAllListeners();
                格子.按钮组件.onClick.AddListener(() => {
                    点击存档格子(存档编号, 数据);
                });
            }
            else
            {
                格子.章节名文本.text = "暂无存档数据";
                格子.保存时间文本.text = "";

                格子.按钮组件.onClick.RemoveAllListeners();
                格子.按钮组件.onClick.AddListener(() => {
                    点击存档格子(存档编号, null);
                });
            }
        }
    }

    private void 点击存档格子(int 存档编号,存档管理器.总存档数据 数据)
    {
        if(当前为存档模式)
        {
            Debug.Log($"正在存储存档 {存档编号}");
            触发存档流程(存档编号);
        }
        else
        {
            if (数据 != null)
            {
                玩家.Instance.解冻主角();
                存档管理器.Instance.待应用数据 = 数据;
                string 目标场景名 = 数据.当前章节;
                Debug.Log($"正在读取存档 {存档编号}，目标场景: {目标场景名}");
                对象池.Instance.清空所有池();
                GameManager.Instance.加载场景(目标场景名);
                关闭界面();
            }
            else
            {
                Debug.Log($"存档 {存档编号} 为空，无法读取");
            }
        }
    }

    private void 触发存档流程(int 存档编号)
    {
        if (存档管理器.Instance.待应用数据 == null)
        {
            Debug.LogError("没有待保存的数据缓存，无法存档！请确保通过存档点打开存档界面。");
            return;
        }
        存档管理器.总存档数据 待保存数据 = 存档管理器.Instance.待保存数据;

        存档管理器.Instance.保存存档(存档编号, 待保存数据);
        Debug.Log($"已将缓存数据保存到档位 {存档编号}，章节: {待保存数据.当前章节}");

        刷新所有格子();
    }

    public void 关闭界面()
    {
        存档点[] 所有存档点 = FindObjectsOfType<存档点>();
        foreach(var 存档点实例 in 所有存档点)
        {
            存档点实例.存档界面已打开 = false;
        }

        清理纹理精灵资源();

        if(当前为存档模式)
        {
            GameManager.Instance.恢复暂停();
        }

        //Debug.Log("关闭存档界面");
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("Save");
    }

    private void 清理纹理精灵资源()
    {
        foreach(var 格子 in 所有存档格子)
        {
            if(格子.缩略图 != null && 格子.缩略图.sprite != null)
            {
                if(格子.缩略图.sprite.texture != null)
                {
                    Destroy(格子.缩略图.sprite.texture);
                }
                Sprite sp = 格子.缩略图.sprite;
                Destroy(sp);

                格子.缩略图.sprite = null;
            }
        }
    }

}
