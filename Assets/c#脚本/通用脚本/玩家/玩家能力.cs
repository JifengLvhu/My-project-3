using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 玩家能力 : MonoBehaviour
{
    // 同伴脚本引用
    private 同伴1 TY脚本 = null;
    private 同伴1 祭风脚本 = null;
    private 同伴1 当前控制的同伴;


    // 能力开关
    public bool 可以控制同伴 = false;
    public bool 可以同伴自爆 = false;
    public bool 可以下砸 = false;
    public bool 可以时间回溯 = false;

    public List<string> 已解锁能力列表 = new List<string>();

    //下砸
    private float 上次下砸输入时间;



    public IEnumerator 复活协程(GameObject 同伴)
    {
        Debug.Log("同伴开始复活");
        同伴.SetActive(false);

        yield return new WaitForSeconds(10f);
        同伴.SetActive(true);
    }

    public void 刷新同伴引用()
    {
        同伴1[] 所有同伴 = FindObjectsOfType<同伴1>();
        foreach(var 同伴 in 所有同伴)
        {
            if(同伴.同伴编号 == 1)
            {
                TY脚本 = 同伴;
            }
            else if(同伴.同伴编号 == 2)
            {
                祭风脚本 = 同伴;
            }
        }

        当前控制的同伴 = TY脚本;
        //Debug.Log($"已刷新同伴引用，当前同伴脚本为{当前控制的同伴}");
    }

    public void 处理能力输入()
    {
        if (可以控制同伴 && 当前控制的同伴 != null)
        {
            切换控制();
            if (当前控制的同伴.gameObject.activeInHierarchy)
            {
                基础控制();
                自爆控制();
            }
        }
        if(可以下砸)
        {
            下砸控制();
        }
    }

    private void 切换控制()
    {
        if (Input.GetKeyDown(按键设置.获取按键("切换控制同伴")))
        {
            if (当前控制的同伴 == TY脚本)
            {
                控制祭风();
            }
            else
            {
                控制TY();
            }
            Debug.Log($"当前控制同伴为{当前控制的同伴}");
        }
    }

    private void 基础控制()
    {
        
        if (Input.GetKeyDown(按键设置.获取按键("指定同伴位置")))
        {
            当前控制的同伴.前往指定位置(transform.position);
        }
        if (Input.GetKeyDown(按键设置.获取按键("恢复同伴跟随")))
        {
            当前控制的同伴.恢复跟随();
        }
    }

    private void 自爆控制()
    {
        if (Input.GetKeyDown(按键设置.获取按键("同伴自爆")))
        {
            if (可以同伴自爆 && 当前控制的同伴 != null)
            {
                当前控制的同伴.执行自爆();
                Debug.Log("同伴自爆");
            }
        }
    }

    private void 下砸控制()
    {   
        if (Input.GetKeyDown(按键设置.获取按键("下砸")) && !玩家.Instance.射线触地检测())
        {
            if (Time.time - 上次下砸输入时间 < 0.5f)
            {
                Debug.Log("成功下砸");
                上次下砸输入时间 = -999f;
                玩家.Instance.下砸();
            }
            else
            {
                上次下砸输入时间 = Time.time;
            }
        }
    }

    public void 解锁能力(string 能力名称)
    {
        if (!已解锁能力列表.Contains(能力名称))
        {
            已解锁能力列表.Add(能力名称);
        }

        switch (能力名称)
        {
            case "控制同伴移动":
                可以控制同伴 = true;
                break;

            case "控制同伴自爆":
                可以同伴自爆 = true;
                break;

            case "下砸":
                可以下砸 = true;
                break;

            default:
                break;
        }
    }

    public void 控制TY()
    {
        当前控制的同伴 = TY脚本;
    }
    public void 控制祭风()
    {
        当前控制的同伴 = 祭风脚本;
    }

    public List<string> 获取已解锁能力列表()
    {
        return new List<string>(已解锁能力列表);
    }

    public void 应用存档数据(List<string> 存档列表)
    {
        已解锁能力列表 = new List<string>(存档列表);
    }
}
