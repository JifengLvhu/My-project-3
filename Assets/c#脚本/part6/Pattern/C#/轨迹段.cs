using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class 轨迹段
{
    public float 本段飞行距离;
    public float 相对旋转角度;

    public 子轨迹类型 轨迹类型;
    
    public float 速度;
    
    [ConditionalHide(nameof(轨迹类型), (int)子轨迹类型.正弦)]
    public float 振幅;
    [ConditionalHide(nameof(轨迹类型), (int)子轨迹类型.正弦)]
    public float 频率;

    public enum 子轨迹类型
    {
        直线,
        正弦,
        自机狙
    }
    
    public I轨迹 创建轨迹(Vector2 生成位置, Vector2 发射方向)
    {
        switch (轨迹类型)
        {
            case 子轨迹类型.直线:
                return new 直线轨迹(发射方向, 速度);
            case 子轨迹类型.正弦:
                return new 正弦轨迹(发射方向, 速度, 振幅, 频率);
            case 子轨迹类型.自机狙:
                return new 自机狙轨迹(生成位置,速度);
            default:
                return new 直线轨迹(发射方向, 速度);
        }
    }
    
}
