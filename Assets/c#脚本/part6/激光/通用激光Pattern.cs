using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "动作/通用激光Pattern", fileName = "通用激光Pattern")]
public class 通用激光Pattern : ScriptableObject, IPattern
{
    public IPattern.Pattern类型 类型 => IPattern.Pattern类型.激光;
    
    [Header("通用参数")] 
    public float 持续时间 = 3f;
    public float 预览时间 = 1f;
    public float 渐变时间 = 0.15f;
    public float 旋转速度;

    [Header("激光显示")] 
    public string 激光名称;
    public GameObject 激光显示预制体;
    public Color 激光颜色 = Color.red; 
    public Color 激光预览颜色 = Color.yellow;

    [Header("形状")] 
    public 激光形状 形状 = 激光形状.直线;

    [ConditionalHide(nameof(形状), (int)激光形状.直线)]
    public float 长度;
    [ConditionalHide(nameof(形状), (int)激光形状.直线)]
    public float 宽度;

    [ConditionalHide(nameof(形状), (int)激光形状.钢铁)]
    [ConditionalHide(nameof(形状), (int)激光形状.月环)]
    public float 外半径;
    [ConditionalHide(nameof(形状), (int)激光形状.月环)]
    public float 内半径;
    
    
    [Header("发射设置")]
    public IPattern.发射位置模式 发射位置模式 = IPattern.发射位置模式.相对boss坐标;
    public List<Vector2> 发射位置列表 = new List<Vector2>();
    public List<float> 发射角度列表 = new List<float>();
    
    public enum 激光形状
    {
        直线,
        钢铁,
        月环
    }
    
    public IEnumerable<IPattern.发射配置> 获取发射位置列表(Vector2 Boss世界坐标)
    {
        int 配置数量 = Mathf.Min(发射位置列表.Count, 发射角度列表.Count);
        
        for(int i = 0; i < 配置数量; i++)
        {
            Vector2 发射位置 = 发射位置列表[i];

            if(发射位置模式 == IPattern.发射位置模式.相对boss坐标)
            {
                发射位置 += Boss世界坐标;
            }
            Vector2 发射方向 = 工具库.角度转向量(发射角度列表[i]);
            
            yield return new IPattern.发射配置(发射位置, 发射方向);
        }
    }
}
