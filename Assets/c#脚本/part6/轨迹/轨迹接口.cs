using UnityEngine;

public interface I轨迹
{
    Vector2 移动(float dt);
    float 获取沿中线总距离();
    Vector2 获取方向();
}
