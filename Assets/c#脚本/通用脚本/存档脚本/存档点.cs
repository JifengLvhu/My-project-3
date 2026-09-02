using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class 存档点 : MonoBehaviour
{
    public enum 章节名称
    {
        Part0,
        Part1, 
        Part2,
        part3,
        part4,
        part5,
        part6,
        part7,
        partFin
    }
    public 章节名称 所属章节 = 章节名称.Part0;
    public int 存档点ID;

    private KeyCode 交互键;
    private KeyCode 存档界面键;
    private bool 玩家在范围内 = false;
    public bool 存档界面已打开 = false;

    private void Update()
    {
        玩家交互判断();
    }

    private void Awake()
    {
        交互键 = 按键设置.获取按键("交互");
        存档界面键 = 按键设置.获取按键("打开存档页面");
    }

    private void OnTriggerEnter2D(Collider2D 其他触发器)
    {
       
        if (其他触发器.CompareTag("玩家"))
        {
            玩家在范围内 = true;
            //Debug.Log($"进入存档点范围: {存档点ID}");
        }
    }

    private void OnTriggerExit2D(Collider2D 其他触发器)
    {
        if (其他触发器.CompareTag("玩家"))
        {
            玩家在范围内 = false;
            //Debug.Log($"离开存档点范围: {存档点ID}");
        }
    }

    private void 玩家交互判断()
    {
        if (玩家在范围内)
        {

            if (Input.GetKeyDown(交互键))
            {
                触发快捷存档();
            }
            else if (Input.GetKeyDown(存档界面键))
            {
                触发打开存档界面();
            }
        }
    }

    private void 触发快捷存档()
    {
        Debug.Log($"触发存档: {存档点ID}，所属章节: {所属章节}");
        StartCoroutine(快捷存档流程());
    }

    private IEnumerator 快捷存档流程()
    {
        yield return 存档管理器.Instance.捕获屏幕截图();

        玩家 玩家脚本 = FindObjectOfType<玩家>();
        if (玩家脚本 != null)
        {
            I章节管理器[] 所有章节管理器 = FindObjectsOfType<MonoBehaviour>().OfType<I章节管理器>().ToArray();
            object 章节数据 = null;

            if (所有章节管理器.Length > 0)
            {
                I章节管理器 当前章节管理器 = 所有章节管理器[0];
                章节数据 = 当前章节管理器.获取章节存档数据();
            }
            else
            {
                Debug.LogWarning("未找到当前场景的章节管理器，仅保存基础数据。");
            }

            string 章节名 = 所属章节.ToString();
            存档管理器.总存档数据 待保存数据 = 玩家脚本.获取最新存档数据(章节名, 章节数据);

            if (存档管理器.最新截图路径 != null)
            {
                待保存数据.截图路径 = 存档管理器.最新截图路径;
                Debug.Log($"快捷存档截图已保存: {待保存数据.截图路径}");
            }

            存档管理器.Instance.保存存档(0, 待保存数据);
        }
        else
        {
            Debug.LogError("未找到玩家脚本");
        }
    }

    private void 触发打开存档界面()
    {
        if(存档界面已打开)
        {
            return;
        }

        存档界面已打开 = true;
        Debug.Log("触发打开存档界面");
        StartCoroutine(打开存档界面流程());
    }


    private IEnumerator 打开存档界面流程()
    {

        yield return 存档管理器.Instance.捕获屏幕截图();

        玩家 玩家脚本 = FindObjectOfType<玩家>();
        I章节管理器 当前章节管理器 = FindObjectsOfType<MonoBehaviour>().OfType<I章节管理器>().FirstOrDefault();
        object 章节数据 = null;

        if (当前章节管理器 != null)
        {
            章节数据 = 当前章节管理器.获取章节存档数据();
        }

        string 章节名 = 所属章节.ToString();
        存档管理器.总存档数据 当前场景数据 = 玩家脚本.获取最新存档数据(章节名, 章节数据);

        if (存档管理器.最新截图路径 != null)
        {
            当前场景数据.截图路径 = 存档管理器.最新截图路径;
        }

        存档管理器.Instance.待保存数据 = 当前场景数据;
        存档管理器.Instance.打开存档场景(true);
    }

}
