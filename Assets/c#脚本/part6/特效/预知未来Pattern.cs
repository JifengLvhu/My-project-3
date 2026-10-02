using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "动作/特效/预知未来", fileName = "新预知未来特效")] 
public class 预知未来Pattern : ScriptableObject, IPattern
{
    public IPattern.Pattern类型 类型 =>IPattern.Pattern类型.特效;
    
    public List<通用激光Pattern> 激光列表;
    
    //无真实使用，仅用于符合接口要求，真正的发射位置由激光Pattern提供
    public IEnumerable<IPattern.发射配置> 获取发射位置列表(Vector2 Boss世界坐标)
    {
        yield return new IPattern.发射配置(Boss世界坐标, Vector2.zero);
    }
}
