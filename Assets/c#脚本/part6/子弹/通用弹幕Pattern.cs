using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "动作/通用弹幕Pattern", fileName = "新弹幕Pattern")]
public class 通用弹幕Pattern : ScriptableObject, IPattern
{
    public IPattern.Pattern类型 类型 => IPattern.Pattern类型.子弹;
    
    [Header("子弹")]
    public GameObject 子弹预制体;
    public string 子弹名称;

    [Header("轨迹设置")]
    public List<弹幕事件> 弹幕事件列表;
    public List<轨迹段> 轨迹段列表;
    
    [Header("发射设置")]
    public IPattern.发射位置模式 发射位置模式 = IPattern.发射位置模式.相对boss坐标;
    public List<Vector2> 发射位置列表 = new List<Vector2>();
    public List<float> 发射角度列表 = new List<float>();

    
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

    public I轨迹 创建轨迹(Vector2 生成位置, Vector2 发射方向)
    {
        return new 分段时序轨迹(生成位置, 发射方向, 轨迹段列表);
    }
}

