using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    private bool 正在加载 = false;
    private string 当前关卡场景名 = null;
    private float 当前时间缩放;

    private bool 游戏已暂停 = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        SceneManager.LoadScene("Main",LoadSceneMode.Additive);

        按键设置 按键设置实例 = FindObjectOfType<按键设置>();
        if (按键设置实例 != null)
        {
            Debug.Log("------ 按键设置初始化情况 ------");
            foreach (var 配置 in 按键设置实例.按键列表)
            {
                Debug.Log($"操作: {配置.操作名称} -> 按键: {配置.当前按键值}");
            }
            Debug.Log("--------------------------------");
        }
        else
        {
            Debug.LogWarning("未找到按键设置实例，无法打印按键情况");
        }
    }

    public IEnumerator 传送玩家(string 目标场景名, string 目标出生点名)
    {
        正在加载 = true;
        yield return 加载场景协程(目标场景名);

        while(正在加载)
        {
            yield return null;
        }

        Scene 新场景 = SceneManager.GetSceneByName(目标场景名);

        GameObject 目标出生点 = 工具库.查找对象(新场景,目标出生点名);
        if (目标出生点 != null)
        {
            玩家.Instance.transform.position = 目标出生点.transform.position;
        }
    }


    public IEnumerator 开始新游戏()
    {
        Debug.Log("开始新游戏");

        yield return 存档管理器.Instance.待应用数据 = 存档管理器.Instance.获取游戏初始化数据();
        加载场景("Part0");
    }

    public void 加载场景(string 目标场景名)
    {
        if(!正在加载)
        {
            正在加载 = true;
            StartCoroutine(加载场景协程(目标场景名));
        }
    }

    private IEnumerator 加载场景协程(string 目标场景名)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        加载页面管理器 管理器 = FindObjectOfType<加载页面管理器>();
        if(管理器 != null)
        {
            管理器.执行加载流程(目标场景名);
        }
        else
        {
            Debug.LogError("找不到加载页面管理器");
            正在加载 = false;
        }
    }

    public IEnumerator 加载完毕(string 新关卡场景名)
    {
        yield return 卸载多余场景(新关卡场景名);
        正在加载 = false;

        恢复暂停();
    }

    private IEnumerator 卸载多余场景(string 新关卡场景名)
    {
        string[] ui场景列表 = { "Save", "Pause", "Settings", "Main" };
        foreach(string 场景名 in ui场景列表)
        {
            Scene 场景 = SceneManager.GetSceneByName(场景名);
            if(场景.isLoaded)
            {
                SceneManager.UnloadSceneAsync(场景);
            }
        }

        string 旧关卡场景名 = 当前关卡场景名;
        当前关卡场景名 = 新关卡场景名;

        if (!string.IsNullOrEmpty(旧关卡场景名))
        {
            Scene 旧场景 = SceneManager.GetSceneByName(旧关卡场景名);
            if (旧场景.isLoaded)
            {
                AsyncOperation 卸载操作 = SceneManager.UnloadSceneAsync(旧场景);
                while (!卸载操作.isDone)
                {
                    yield return null;
                }
            }
        }
    }

    public void 卸载加载场景()
    {
        SceneManager.UnloadSceneAsync("Loading");
    }

    public void 暂停游戏()
    {
        if (游戏已暂停) return;

        Debug.Log("游戏暂停");

        玩家.Instance.游戏已被暂停 = true;
        当前时间缩放 = Time.timeScale;
        Time.timeScale = 0f;
        游戏已暂停 = true;
    }

    public void 恢复暂停()
    {
        if (!游戏已暂停) return;

        Time.timeScale = 当前时间缩放;
        游戏已暂停 = false;
        玩家.Instance.游戏已被暂停 = false;

        //Debug.Log("暂停恢复");
    }
}
