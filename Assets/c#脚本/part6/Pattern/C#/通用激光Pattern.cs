using UnityEngine;

[CreateAssetMenu(menuName = "弹幕/通用激光Pattern", fileName = "通用激光Pattern")]
public class 通用激光Pattern : ScriptableObject, IPattern
{
    public IPattern.Pattern类型 类型 = IPattern.Pattern类型.激光;
    
    [Header("通用参数")] 
    public float 持续时间 = 3f;
    [Min(0f)]
    public float 旋转速度;

    [Header("激光显示")] 
    public string 激光名称;
    public GameObject 激光显示预制体;
    public Color 激光颜色 = Color.red; 

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
    
    public enum 激光形状
    {
        直线,
        钢铁,
        月环
    }
}
