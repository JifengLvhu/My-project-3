using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 激光预览实例
{
    private readonly 通用激光Pattern Pattern;

    private readonly GameObject 显示对象;
    private readonly I激光显示 直线显示;
    
    private Vector2 起始位置;
    private Vector2 初始方向;
    private float 当前角度;
    private float 已运行时间;
    
    public bool 已完成;
    
    public 激光预览实例(Vector2 生成位置, Vector2 发射方向, 通用激光Pattern pattern, GameObject 传入对象)
    {
        Pattern = pattern;
        起始位置 = 生成位置;
        初始方向 = 发射方向.normalized;
        当前角度 = 0f;
        已运行时间 = 0f;
        显示对象 = 传入对象;

        if (显示对象 != null)
        {
            直线显示 = 显示对象.GetComponent<I激光显示>();

            if (直线显示 == null)
            {
                Debug.LogError("激光显示预制体缺少直线激光显示组件。", 显示对象);
            }
        }
    }
    
    public void 更新(float dt)
    {
        if(已完成)
        {
            return;
        }

        已运行时间 += dt;
        
        Vector2 当前方向 = 旋转方向();
        Color 当前颜色 = 获取当前颜色();
        
        直线显示?.更新显示(起始位置, 当前方向, Pattern, 当前颜色);
        
        if(已运行时间 >= Pattern.持续时间)
        {
            结束();
            return;
        }
        
        if (Pattern.旋转速度 != 0f)
        {
            当前角度 += Pattern.旋转速度 * dt;
        }
    }
    
    private Color 获取当前颜色()
    {
        float 渐变时长 = Pattern.渐变时间;
        float 剩余时间 = Pattern.持续时间 - 已运行时间;
        float 透明比例 = 1f;

        if (已运行时间 <= 渐变时长)
        {
            透明比例 = 已运行时间 / 渐变时长;
        }
        else if(剩余时间 <= 渐变时长)
        {
            透明比例 = 剩余时间 / 渐变时长;
        }

        透明比例 = Mathf.Clamp01(透明比例);
        
        Color 当前颜色 = Pattern.激光预览颜色;
        当前颜色.a *= 透明比例;
        return 当前颜色;
    }
    
    private Vector2 旋转方向()
    {
        return Quaternion.Euler(0, 0, 当前角度) * 初始方向;
    }
    
    private void 结束()
    {
        if (已完成) return;
        
        已完成 = true;
        if (显示对象 != null)
        {
            对象池.Instance.归还对象(Pattern.激光名称, 显示对象);
        }
    }
}
