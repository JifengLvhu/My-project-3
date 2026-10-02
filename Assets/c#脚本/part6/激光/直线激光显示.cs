using UnityEngine;

public class 直线激光显示 : MonoBehaviour,I激光显示,I池对象重置
{
    private LineRenderer 线条;

    private void Awake()
    {
        线条 = GetComponent<LineRenderer>();
        
        线条.useWorldSpace = true;
        线条.positionCount = 2;
        线条.enabled = true;
    }

    public void 更新显示(Vector2 起点, Vector2 方向, 通用激光Pattern pattern, Color 显示颜色)
    {
        线条.startWidth = pattern.宽度;
        线条.endWidth = pattern.宽度;
        线条.startColor = 显示颜色;
        线条.endColor = 显示颜色;
        线条.positionCount = 2;
        线条.SetPosition(0, 起点);
        线条.SetPosition(1, 起点 + 方向 * pattern.长度);
    }

    public void 池对象重置()
    {
        线条.startWidth = 0f;
        线条.endWidth = 0f;
        线条.endColor = Color.clear;
        线条.startColor = Color.clear;
    }
}
