using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "弹幕/通用弹幕Pattern", fileName = "新弹幕Pattern")]
public class 通用弹幕Pattern : ScriptableObject, IPattern
{
    public IPattern.Pattern类型 类型 = IPattern.Pattern类型.子弹;
    
    [Header("子弹")]
    public GameObject 子弹预制体;
    public string 子弹名称;

    [Header("发射参数")]
    public float 发射间隔;
    public int 每轮数量;

    [Header("轨迹设置")]
    public List<弹幕事件> 弹幕事件列表;
    public List<轨迹段> 轨迹段列表;

    

    public I轨迹 创建轨迹(Vector2 生成位置, Vector2 发射方向)
    {
        return new 分段时序轨迹(生成位置, 发射方向, 轨迹段列表);
    }
}