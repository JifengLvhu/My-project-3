using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 直线轨迹 : I轨迹
{
    private Vector2 方向;
    private float 速度;
    private float 沿中线移动总距离;
    
    public 直线轨迹(Vector2 传入方向, float 传入速度)
    {
        方向 = 传入方向.normalized;
        速度 = 传入速度;
        沿中线移动总距离 = 0f;
    }

    public Vector2 移动(float dt)
    {
        沿中线移动总距离 += 速度 * dt;
        return 方向 * (速度 * dt);
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
