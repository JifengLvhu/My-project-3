using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 月环激光显示 : MonoBehaviour,I激光显示,I池对象重置
{
    private MeshRenderer Mr;
    private Material Mat;
    private float 原始外半径;

    private void Awake()
    {
        Mr = GetComponent<MeshRenderer>();
        Mat = Mr.material;
        原始外半径 = Mat.GetFloat("OuterRadius");
    }

    public void 更新显示(Vector2 起点, Vector2 方向, 通用激光Pattern pattern, Color 显示颜色)
    {
        float 目标缩放 = pattern.外半径 / 原始外半径;
        transform.localScale = new Vector3(目标缩放, 目标缩放, 1);
        
        Mat.SetFloat("InnerRadius", pattern.内半径 / 目标缩放);
        Mat.SetColor("Col", 显示颜色);

        方向 = 方向.normalized;
        float 角度 = Mathf.Atan2(方向.y, 方向.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, 角度);
        transform.position = 起点;
    }

    public void 池对象重置()
    {
        transform.localScale = Vector3.one;
        Mat.SetColor("Col", Color.clear);
        Mat.SetFloat("Soft", 0.01f);
    }
}