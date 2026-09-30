using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 直线激光显示 : MonoBehaviour
{
    [SerializeField] private LineRenderer 线条;

    private void Awake()
    {
        线条 = GetComponent<LineRenderer>();
        
        线条.useWorldSpace = true;
        线条.positionCount = 2;
        线条.enabled = true;
    }

    public void 更新显示(Vector2 起点, Vector2 方向, 通用激光Pattern pattern)
    {
        线条.startWidth = pattern.宽度;
        线条.endWidth = pattern.宽度;
        线条.startColor = pattern.激光颜色;
        线条.endColor = pattern.激光颜色;
        线条.positionCount = 2;
        线条.SetPosition(0, 起点);
        线条.SetPosition(1, 起点 + 方向 * pattern.长度);
    }
}
