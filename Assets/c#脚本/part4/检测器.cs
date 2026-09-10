using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 检测器 : MonoBehaviour
{
    public bool 已经触发 = false;

    void OnTriggerEnter2D(Collider2D 其他触发器)
    {
        if (已经触发) return;
        已经触发 = true;

        if(其他触发器.tag == "玩家")
        {
            Debug.Log("玩家进入了检测器");

            史莱姆[] 所有史莱姆 = Object.FindObjectsByType<史莱姆>(FindObjectsSortMode.None);
            foreach(var 史莱姆实例 in 所有史莱姆)
            {
                史莱姆实例.分裂();
            }
        }
    }
}
