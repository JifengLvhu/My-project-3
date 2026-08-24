using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 玩家能力 : MonoBehaviour
{
    // 同伴脚本引用
    public 同伴1 TY脚本;
    public 同伴1 祭风脚本;

    private 同伴1 当前控制的同伴;


    // 能力开关
    private bool 可以控制同伴 = true;
    private bool 可以同伴自爆 = false;
    private bool 可以下砸 = false;
    private bool 可以时间回溯 = false;




    void Start()
    {
        控制TY();
    }

    void Update()
    {
        
    }


    public void 处理能力输入()
    {
        if(Input.GetKeyDown(按键设置.获取按键("切换控制角色")))
        {
            if(当前控制的同伴 == TY脚本)
            {
                控制祭风();
            }
            else 
            {
                控制TY();
            }
        }

        if(可以控制同伴 && Input.GetKeyDown(按键设置.获取按键("指定同伴移动")))
        {
            当前控制的同伴.前往指定位置(transform.position);
        }
        if(可以控制同伴 && Input.GetKeyDown(按键设置.获取按键("恢复同伴跟随")))
        {
            当前控制的同伴.恢复跟随();
        }
    }

    public void 解锁能力(string 能力名称)
    {
        switch (能力名称)
        {
            case "控制同伴移动":
                可以控制同伴 = true;
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
}
