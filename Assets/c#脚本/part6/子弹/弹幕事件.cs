using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class 弹幕事件
{
    public float 触发距离;
    public 通用弹幕Pattern 子弹幕Pattern;
    public 弹幕事件类型 事件类型;
    
  
    //直线参数
    [ConditionalHide(nameof(事件类型), (int)弹幕事件类型.单颗发射)]
    public float 旋转角度;
    
    //扇形参数
    [ConditionalHide(nameof(事件类型), (int)弹幕事件类型.扇形发射)]
    public float 扇形总角度;
    [ConditionalHide(nameof(事件类型), (int)弹幕事件类型.扇形发射)]
    public float 扇形基准相对角度;  
    
    //扇形与空爆通用参数
    [ConditionalHide(nameof(事件类型), (int)弹幕事件类型.扇形发射)]
    [ConditionalHide(nameof(事件类型), (int)弹幕事件类型.空爆发射)]
    public int 发射数量;


    public enum 弹幕事件类型
    {
        扇形发射,
        单颗发射,
        空爆发射
    }
}
