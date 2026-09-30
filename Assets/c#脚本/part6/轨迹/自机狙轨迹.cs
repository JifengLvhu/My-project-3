using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 自机狙轨迹 : I轨迹
{
    private Vector2 方向;
    private float 速度;
    private float 沿中线移动总距离;
    
    public 自机狙轨迹(Vector2 生成位置, float 传入速度)
    {
        方向 = ((Vector2)玩家.Instance.transform.position - 生成位置).normalized;
        速度 = 传入速度;
    }
    
    public Vector2 移动(float dt)
    {
        float 移动距离 = 速度 * dt;
        沿中线移动总距离 += 移动距离;
        return 方向 * 移动距离;
    }
    
    public float 获取沿中线总距离()
    {
        return 沿中线移动总距离;
    }
    
    public Vector2 获取方向()
    {
        return 方向;
    }
}
