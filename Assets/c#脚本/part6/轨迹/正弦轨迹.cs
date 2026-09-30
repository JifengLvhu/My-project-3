using UnityEngine;

public class 正弦轨迹 : I轨迹
{
    private Vector2 方向;
    private Vector2 法线;
    private float 速度;
    private float 振幅 = 0.5f;
    private float 频率 = 2f;
    private float 已前进距离;
    private float 上一帧偏移;
    
    public 正弦轨迹(Vector2 传入方向, float 传入速度,float 传入振幅,float 传入频率)
    {
        方向 = 传入方向.normalized;
        速度 = 传入速度;
        振幅 = 传入振幅;
        频率 = 传入频率;
        已前进距离 = 0f;
        上一帧偏移 = 0f;
        
        法线 = Vector2.Perpendicular(方向);
    }

    public Vector2 移动(float dt)
    {
        float 前进步长 = 速度 * dt;
        已前进距离 += 前进步长;

        float 横向偏移 = Mathf.Sin(频率 * 已前进距离) * 振幅;
        Vector2 前进距离 = 方向 * 前进步长;
        Vector2 正弦偏移 = 法线 * (横向偏移 - 上一帧偏移);
        上一帧偏移 = 横向偏移;
        return 前进距离 + 正弦偏移;
    }
    
    public float 获取沿中线总距离()
    {
        return 已前进距离;
    }

    public Vector2 获取方向()
    {
        return 方向;
    }
}
