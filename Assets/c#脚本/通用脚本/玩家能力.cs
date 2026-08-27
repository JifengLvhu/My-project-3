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
    private bool 可以控制同伴 = false;
    private bool 可以同伴自爆 = false;
    private bool 可以下砸 = false;
    private bool 可以时间回溯 = false;

    public List<string> 已解锁能力列表 = new List<string>();

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
    }

    public void 处理能力输入()
    {

        if (可以控制同伴 && 当前控制的同伴 != null)
        {
            if (Input.GetKeyDown(按键设置.获取按键("切换控制角色")))
            {
                if (当前控制的同伴 == TY脚本)
                {
                    控制祭风();
                }
                else
                {
                    控制TY();
                }
            }
            if (Input.GetKeyDown(按键设置.获取按键("指定同伴移动")))
            {
                当前控制的同伴.前往指定位置(transform.position);
            }
            if (Input.GetKeyDown(按键设置.获取按键("恢复同伴跟随")))
            {
                当前控制的同伴.恢复跟随();
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
