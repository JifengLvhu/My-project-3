using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPattern
{
    Pattern类型 类型 { get; }

    public IEnumerable<发射配置> 获取发射位置列表(Vector2 boss世界坐标);
    
    public enum Pattern类型
    {
        子弹,
        激光,
        特效
    }
    
    public struct 发射配置
    {
        public Vector2 发射位置;
        public Vector2 发射方向;
        public 发射配置(Vector2 位置, Vector2 方向)
        {
            发射位置 = 位置;
            发射方向 = 方向;
        }
    }

    public enum 发射位置模式
    {
        绝对世界坐标,
        相对boss坐标
    }
}
