using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 阀门 : MonoBehaviour
{
    public enum 阀门类型 { 进水阀,排水阀}

    public 阀门类型 类型;
    public float 流速 = 1f;

    public 水池 目标水池;

    private bool 开关 = false;

    public void 开启() => 开关 = true;
    public void 关闭() => 开关 = false;
    public void 切换() => 开关 = !开关;


    private KeyCode 交互键;
    private bool 玩家在范围内 = false;


    private void Awake()
    {
        交互键 = 按键设置.获取按键("交互");
    }

    private void Update()
    {

        水位处理();
        if(玩家在范围内 && Input.GetKeyDown(交互键))
        {
            切换();
        }
        
    }

    private void OnTriggerEnter2D(Collider2D 其他触发器)
    {

        if (其他触发器.CompareTag("玩家"))
        {
            玩家在范围内 = true;
        }
    }

    private void OnTriggerExit2D(Collider2D 其他触发器)
    {
        if (其他触发器.CompareTag("玩家"))
        {
            玩家在范围内 = false;
        }
    }

    private void 水位处理()
    {
        if (开关 && 目标水池 != null)
        {
            float 变化水位 = Time.deltaTime * 流速;

            if (类型 == 阀门类型.进水阀)
            {
                目标水池.设置水位(目标水池.当前水位 + 变化水位);
            }
            else if (类型 == 阀门类型.排水阀)
            {
                目标水池.设置水位(目标水池.当前水位 - 变化水位);
            }
        }
        else
        {
            return;
        }
    }
}
