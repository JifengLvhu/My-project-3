using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "动作/特效/推拉玩家", fileName = "新推拉玩家特效")] 
public class 推拉玩家Pattern : ScriptableObject, IPattern
{
    public IPattern.Pattern类型 类型 =>IPattern.Pattern类型.特效;

    [Header("推拉参数")]
    public bool 是否拉力;
    public float 距离 = 10f;

    [Header("时间")]
    public float 预览时间 = 1f;
    
    public IPattern.发射位置模式 发射位置模式 = IPattern.发射位置模式.相对boss坐标;
    public List<Vector2> 作用点列表 = new List<Vector2>();
    
    public IEnumerable<IPattern.发射配置> 获取发射位置列表(Vector2 Boss世界坐标)
    {
        foreach (var 作用点 in 作用点列表)
        {
            if(发射位置模式 == IPattern.发射位置模式.相对boss坐标)
            {
                yield return new IPattern.发射配置(Boss世界坐标 + 作用点, Vector2.zero);
            }
            else
            {
                yield return new IPattern.发射配置(作用点, Vector2.zero);
            }
        }
    }
}

