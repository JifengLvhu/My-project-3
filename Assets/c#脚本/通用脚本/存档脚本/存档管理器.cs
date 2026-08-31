using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;


public class 存档管理器 : MonoBehaviour
{
    public static 存档管理器 Instance { get; private set; }

    public 总存档数据 待应用数据;//加载后应用的数据
    public 总存档数据 待保存数据;//存档点与存档页面管理器传递通道
    private const int 最大存档数量 = 16;
    private 总存档数据[] 存档列表;
    private bool 当前为存档模式 = false;
    public static string 最新截图路径;


    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;

            存档列表 = new 总存档数据[最大存档数量];
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    public 总存档数据 获取游戏初始化数据()
    {
        return new 总存档数据
        {
            当前章节 = "Part0",
            基础数据 = new 基础存档数据
            {
                当前生命值 = 玩家.Instance.最大生命值,
                当前氧气量 = 玩家.Instance.最大氧气量,
                玩家位置 = Vector3.zero
            },
            能力数据 = new 玩家能力存档数据(),
            剧情数据 = new 剧情存档数据(),

            Part2数据 = new Part2存档数据()

        };
    }
    
    public void 保存存档(int 存档位置 ,总存档数据 数据存档)
    {
        if(存档位置 < 0 || 存档位置 >= 最大存档数量)
        {
            return;
        }

        数据存档.存档时间 = DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss");

        存档列表[存档位置] = 数据存档;

        string json内容 = JsonUtility.ToJson(数据存档, true);
        string 存档路径 = Path.Combine(Application.persistentDataPath, $"SaveGameData_{存档位置}.json");

        File.WriteAllText(存档路径, json内容);
        Debug.Log($"游戏已保存到档位 {存档位置} ({存档路径})");
    }

    public 总存档数据 读取存档(int 存档位置)
    {

        if (存档位置 < 0 || 存档位置 >= 最大存档数量)
        {
            return null;
        }

        if(存档列表[存档位置] != null)
        {
            return 存档列表[存档位置];
        }

        string  存档路径 = Path.Combine(Application.persistentDataPath, $"SaveGameData_{存档位置}.json");

        if(File.Exists(存档路径))
        {
            string json内容 = File.ReadAllText(存档路径);

            总存档数据 数据存档 = JsonUtility.FromJson<总存档数据>(json内容);
            存档列表[存档位置] = 数据存档;

            Debug.Log($"游戏已从{存档路径}读取");
            return 数据存档;
        }
        else 
        {
            //Debug.Log($"存档文件不存在");
            return null;
        }
    }

    public IEnumerator 死亡后快捷读档协程()
    {

        if(读取存档(0) != null)
        {
            触发快捷读档();
        }
        else
        {
            GameManager.Instance.StartCoroutine(GameManager.Instance.开始新游戏());
        }

        yield return null;
    }

    public void 触发快捷读档()
    {
        总存档数据 数据 = 读取存档(0);
        if (数据 == null) return;

        待应用数据 = 数据;

        string 目标章节 = 数据.当前章节;

        Debug.Log($"正在加载{目标章节}");
        GameManager.Instance.加载场景(目标章节);
    }

    public void 打开存档场景(bool 是存档模式)
    {
        当前为存档模式 = 是存档模式;
        SceneManager.LoadScene("Save", LoadSceneMode.Additive);

        待应用数据 = new 总存档数据();
    }


    public void 清空存档数据缓存()
    {
        待应用数据 = null;
        //Debug.Log("存档数据缓存已清空");
    }

    private void OnSceneLoaded(Scene 场景, LoadSceneMode 加载模式)
    {
        if(场景.name == "Save")
        {
            存档页面管理器 管理器 = FindObjectOfType<存档页面管理器>();
            if(管理器 != null)
            {
                管理器.初始化界面(当前为存档模式);
            }
        }
    }

    public void 应用待应用存档数据()
    {
        if (待应用数据 != null)
        {
            玩家.Instance.应用存档数据(待应用数据);
            清空存档数据缓存();
        }
    }

    public IEnumerator 捕获屏幕截图()
    {
        yield return new WaitForEndOfFrame();
        string 时间戳 = DateTime.Now.ToString("yyyyMMddHHmmss");
        string 文件名 = $"Screenshot_{时间戳}.png";

        string 目录路径 = Path.Combine(Application.persistentDataPath, "Screenshots");
        if(!Directory.Exists(目录路径))
        {
            Directory.CreateDirectory(目录路径);
        }
        string 完整路径 = Path.Combine(目录路径, 文件名);

        ScreenCapture.CaptureScreenshot(完整路径);

        string 相对路径 = Path.Combine("Screenshots", 文件名);

        最新截图路径 = 相对路径;
    }

    [Serializable]
    public class 基础存档数据
    {
        public int 当前生命值;
        public float 当前氧气量;
        public Vector3 玩家位置;
    }

    [Serializable]
    public class 玩家能力存档数据
    {
        public List<string> 已解锁能力列表 = new List<string>();
    }

    [Serializable]
    public class 剧情存档数据
    {
        public List<string> 已触发剧情列表 = new List<string>();
    }


    //P0无专属数据

    //P1无专属数据

    //P2
    [Serializable]
    public class Part2存档数据
    {
        public List<水池存档单元> 水池列表 = new List<水池存档单元>();
    }

    [Serializable]
    public class 水池存档单元
    {
        public int 水池ID;
        public float 当前水位高度;
    }

    [Serializable]
    public class 总存档数据
    {
        public string 当前章节;
        public 玩家能力存档数据 能力数据;
        public 剧情存档数据 剧情数据;
        public 基础存档数据 基础数据;
        public Part2存档数据 Part2数据;

        public string 存档时间;
        public string 截图路径;
    }
}
