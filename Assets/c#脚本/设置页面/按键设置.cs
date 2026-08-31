using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class 按键设置 : MonoBehaviour
{

    [System.Serializable]
    public class 按键配置
    {
        public string 操作名称;
        public TMP_Text 按键提示;
        public TMP_Text 当前按键;
        public Button 设置按键;
        public Button 重置按键;
        public string 默认按键;
        public string 当前按键值;
    }

    public List<按键配置> 按键列表 = new List<按键配置>();
    public TMP_Text 提示文本;
    public GameObject 提示面板;
    public Button 全局重置按键;

    private bool 正在设置按键 = false;
    private 按键配置 当前配置;

    private readonly List<KeyCode> 允许的按键列表 = new List<KeyCode>();

    private void 初始化允许的按键列表()
    {
        //A-Z
        for (int i = (int)KeyCode.A; i <= (int)KeyCode.Z; i++)
        {
            允许的按键列表.Add((KeyCode)i);
        }

        //0-9
        for (int i = (int)KeyCode.Alpha0; i <= (int)KeyCode.Alpha9; i++)
        {
            允许的按键列表.Add((KeyCode)i);
        }

        var 常用功能键 = new KeyCode[]
        {
        KeyCode.Space,
        KeyCode.LeftShift,
        KeyCode.RightShift,
        KeyCode.LeftControl,
        KeyCode.RightControl,
        KeyCode.LeftAlt,
        KeyCode.RightAlt
        };

        foreach (var 按键 in 常用功能键)
        {
            if (!允许的按键列表.Contains(按键))
            {
                允许的按键列表.Add(按键);
            }
        }

        //小键盘0-9
        for (int i = 0; i <= 9; i++)
        {
            允许的按键列表.Add((KeyCode)System.Enum.Parse(typeof(KeyCode), $"Keypad{i}"));
        }

        var 主键盘方向键 = new KeyCode[]
        {
            KeyCode.LeftArrow,
            KeyCode.RightArrow,
            KeyCode.UpArrow,
            KeyCode.DownArrow
        };

        foreach (var 按键 in 主键盘方向键)
        {
            if (!允许的按键列表.Contains(按键))
            {
                允许的按键列表.Add(按键);
            }
        }
    }

    private void Start()
    {
        初始化允许的按键列表();
        加载设置();
        更新按键显示();

        /*
        Debug.Log("------ 按键设置初始化情况 ------");
        foreach (var 配置 in 按键列表)
        {
            KeyCode 当前按键 = 获取按键(配置.操作名称);
            Debug.Log($"操作: {配置.操作名称} -> 当前按键值: {配置.当前按键值} -> 获取按键结果: {当前按键}");
        }
        Debug.Log("--------------------------------");
        */

        foreach (var 配置 in 按键列表)
        {
            var 绑定配置 = 配置;

            配置.设置按键.onClick.AddListener(() =>
            {
                开始设置(绑定配置);
            });

            配置.重置按键.onClick.AddListener(() =>
            {
                重置单个按键(绑定配置);
            });
        }

        全局重置按键.onClick.AddListener(() => { 
            重置所有按键(); 
        });
    }

    void Update()
    {
        if (正在设置按键)
        {
            foreach (KeyCode 按键 in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(按键))
                {
                    设置按键(按键);
                    正在设置按键 = false;
                    提示面板.SetActive(false);
                    break;
                }
            }
        }
    }

    public static KeyCode 获取按键 (string 按键名称) //需要在未加载脚本时也可以调用，所以设置为静态函数
    {
        string 按键名 = PlayerPrefs.GetString(按键名称, "");

        if(System.Enum.TryParse(按键名,out KeyCode 结果))
        {
            return 结果;
        }
        else
        {
            return KeyCode.None;
        }
    }

    private void 开始设置(按键配置 配置)
    {
        正在设置按键 = true;
        当前配置 = 配置;
        提示面板.SetActive(true);
        提示文本.text = "请按下想要设置" + 配置.操作名称 + "的按键";
    }

    private void 设置按键(KeyCode 按键)
    {
        if(正在设置按键 && 当前配置 != null)
        {
            if(!允许的按键列表.Contains(按键))
            {
                提示文本.text = "该按键不允许设置，请选择其他按键";
                return;
            }

            foreach (var 配置 in 按键列表)
            {
                if (配置 != 当前配置 && 配置.当前按键值 == 按键.ToString())
                {
                    提示文本.text = "该按键已被其他操作使用，请选择其他按键";
                    return;
                }
            }

            当前配置.当前按键值 = 按键.ToString();
            当前配置.当前按键.text = 按键.ToString();
            保存设置();
        }
    }

    private void 重置单个按键(按键配置 配置)
    {
        配置.当前按键值 = 配置.默认按键;
        配置.当前按键.text = 配置.默认按键;
        保存设置();
    }

    private void 重置所有按键()
    {
        foreach (var 配置 in 按键列表)
        {
            配置.当前按键值 = 配置.默认按键;
            配置.当前按键.text = 配置.默认按键;
        }
        保存设置();
    }

    private void 保存设置()
    {
        foreach (var 配置 in 按键列表)
        {
            PlayerPrefs.SetString(配置.操作名称, 配置.当前按键值);
        }
        PlayerPrefs.Save();
    }

    private void 加载设置()
    {
        foreach (var 配置 in 按键列表)
        {
            配置.当前按键值 = PlayerPrefs.GetString(配置.操作名称, 配置.默认按键);
        }
    }

    private void 更新按键显示()
    {
        foreach (var 配置 in 按键列表)
        {
            配置.当前按键.text = 配置.当前按键值;
        }
    }

    
}
