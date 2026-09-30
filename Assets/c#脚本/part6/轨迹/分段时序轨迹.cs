using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 分段时序轨迹 : I轨迹
{
    private Vector2 当前方向;
    private int 当前段索引;
    private float 本段已飞行距离;
    private List<轨迹段> 轨迹段列表;
    private I轨迹 当前子轨迹;
    private float 沿中线移动总距离;
    private Vector2 当前位置;
    
    public 分段时序轨迹(Vector2 生成位置, Vector2 初始方向, List<轨迹段> 传入轨迹段列表)
    {
        当前位置 = 生成位置;
        当前方向 = 初始方向.normalized;
        轨迹段列表 = 传入轨迹段列表;
        当前段索引 = 0;
        本段已飞行距离 = 0f;
        沿中线移动总距离 = 0f;

        加载子轨迹段();
    }
    
    private void 加载子轨迹段()
    {
        轨迹段 当前段 = 轨迹段列表[当前段索引];
        
        当前子轨迹 = 当前段.创建轨迹(当前位置, 当前方向);
        当前方向 = 当前子轨迹.获取方向();
    }

    public Vector2 移动(float dt)
    {
        Vector2 位移 = 当前子轨迹.移动(dt);
        当前位置 += 位移;
        
        float 本帧前进距离 = Vector2.Dot(位移, 当前方向);
        本段已飞行距离 += 本帧前进距离;
        沿中线移动总距离 += 本帧前进距离;
        
        if(本段已飞行距离 >= 轨迹段列表[当前段索引].本段飞行距离)
        {
            切换到下一段();
        }

        return 位移;
    }

    private void 切换到下一段()
    {
        if(当前段索引 >= 轨迹段列表.Count - 1)
        {
            return;
        }
        
        当前段索引++;
        
        轨迹段 当前轨迹段 = 轨迹段列表[当前段索引];
        
        Quaternion 旋转 = Quaternion.Euler(0, 0, 当前轨迹段.相对旋转角度);
        当前方向 = 旋转 * 当前方向;
        
        本段已飞行距离 = 0f;
        加载子轨迹段();
    
    }
    
    public float 获取沿中线总距离()
    {
        return 沿中线移动总距离;
    }

    public Vector2 获取方向()
    {
        return 当前方向;
    }
}
