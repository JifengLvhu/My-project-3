using UnityEngine;

public class 激光运行实例
{
    private readonly 通用激光Pattern Pattern;

    private readonly GameObject 显示对象;
    private readonly 直线激光显示 直线显示;
    
    private Vector2 起始位置;
    private Vector2 初始方向;
    private float 当前角度;
    private float 剩余时间;
    
    public bool 已完成;

    public 激光运行实例(Vector2 生成位置, Vector2 发射方向, 通用激光Pattern pattern, GameObject 传入对象)
    {
        Pattern = pattern;
        起始位置 = 生成位置;
        初始方向 = 发射方向.normalized;
        当前角度 = 0f;
        剩余时间 = Pattern.持续时间;
        显示对象 = 传入对象;

        if (显示对象 != null)
        {
            直线显示 = 显示对象.GetComponent<直线激光显示>();

            if (直线显示 == null)
            {
                Debug.LogError("激光显示预制体缺少直线激光显示组件。", 显示对象);
            }
        }
    }

    public void 更新(float dt)
    {
        if(已完成)
        {
            return;
        }

        剩余时间 -= dt;
        
        Vector2 当前方向 = 旋转方向();
        直线显示?.更新显示(起始位置, 当前方向, Pattern);

        检查玩家命中();
        
        if(剩余时间 <= 0f)
        {
            结束();
            return;
        }
        
        if (Pattern.旋转速度 != 0f)
        {
            当前角度 += Pattern.旋转速度 * dt;
        }
    }

    private void 检查玩家命中()
    {
        if (玩家.Instance == null)
        {
            return;
        }
        
        Vector2 玩家位置 = 玩家.Instance.transform.position;
        Vector2 当前方向 = 旋转方向();

        bool 命中 = Pattern.形状 switch
        {
            通用激光Pattern.激光形状.直线 => 检测直线激光命中(当前方向, 玩家位置),
            通用激光Pattern.激光形状.钢铁 => 检测钢铁激光命中(玩家位置),
            通用激光Pattern.激光形状.月环 => 检测月环激光命中(玩家位置),
            _ => false
        };

        if (命中)
        {
            玩家.Instance.受到伤害(1);
        }
            
    }

    private Vector2 旋转方向()
    {
        return Quaternion.Euler(0, 0, 当前角度) * 初始方向;
    }
    
    private bool 检测直线激光命中(Vector2 当前方向, Vector2 玩家位置)
    {
        Vector2 激光终点 = 起始位置 + 当前方向 * Pattern.长度;
        
        float 距离平方 = 点到线段长度(玩家位置, 起始位置, 激光终点);
        float 半宽 = Pattern.宽度 / 2f;
        
        return 距离平方 <= 半宽 * 半宽;
    }
    
    private bool 检测钢铁激光命中(Vector2 玩家位置)
    {
        float 距离平方 = (玩家位置 - 起始位置).sqrMagnitude;
        float 半径 = Pattern.外半径;
        
        return 距离平方 <= 半径 * 半径;
    }
    
    private bool 检测月环激光命中(Vector2 玩家位置)
    {
        float 距离平方 = (玩家位置 - 起始位置).sqrMagnitude;
        float 内半径 = Pattern.内半径;
        float 外半径 = Pattern.外半径;

        return 距离平方 >= 内半径 * 内半径 && 距离平方 <= 外半径 * 外半径;
    }

    private float 点到线段长度(Vector2 点位置, Vector2 线段起点, Vector2 线段终点)
    {
        Vector2 线段方向 = 线段终点 - 线段起点;
        float 线段长度平方 = 线段方向.sqrMagnitude;

        if (线段长度平方 < 0.001f)
        {
            return (点位置 - 线段起点).sqrMagnitude;
        }
        
        float t = Vector2.Dot(点位置 - 线段起点, 线段方向) / 线段长度平方;
        t = Mathf.Clamp01(t);
        Vector2 最近点 = 线段起点 + t * 线段方向;
        return (点位置 - 最近点).sqrMagnitude;
    }
    
    private void 结束()
    {
        if (已完成) return;
        
        已完成 = true;
        if (显示对象 != null)
        {
            Object.Destroy(显示对象);
        }
    }
    
}
