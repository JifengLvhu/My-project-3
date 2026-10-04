using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "动作/特效/动静剑", fileName = "新动静剑特效")] 
public class 动静剑Pattern : ScriptableObject, IPattern
{
    public float 预览时间;
    public float 持续时间;
    public bool 要求移动;
    
    [Header("预览参数")]
    public string 动静剑预览名称 = "动静剑预览预制体";
    public GameObject 动静剑预览预制体;
    
    public IPattern.Pattern类型 类型 =>IPattern.Pattern类型.特效;
    
    public IEnumerable<IPattern.发射配置> 获取发射位置列表(Vector2 Boss世界坐标)
    {
        yield return new IPattern.发射配置(Boss世界坐标, Vector2.zero);
    }
}
