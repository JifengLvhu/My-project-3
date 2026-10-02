using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class 行为段
{
    public 类型 行为类型 = 类型.等待;

    [ConditionalHide(nameof(行为类型), (int)类型.等待)]
    public float 等待时间 = 1f;

    [ConditionalHide(nameof(行为类型), (int)类型.动作)]
    public List<配置> 配置列表;

    public enum 类型
    {
        等待,
        动作
    }
}

[System.Serializable]
public class 配置
{
    public ScriptableObject pattern;
    public int 发射轮数;
    public int 已发射轮数;
    public float 发射间隔;
    public float 首轮延迟;
    public bool 是否完成;
    
    [System.NonSerialized]
    public bool 正在运行;
}
