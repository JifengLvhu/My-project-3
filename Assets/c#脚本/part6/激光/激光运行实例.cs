using UnityEngine;

public class 激光运行实例
{
    private readonly 通用激光Pattern Pattern;

    private readonly GameObject 显示对象;
    private readonly I激光显示 直线显示;
    
    private Vector2 起始位置;
    private Vector2 初始方向;
    private float 当前角度;
    private float 已运行时间;

    private readonly bool 是否包含预览阶段;
    private readonly float 总持续时间;
    
    public bool 已完成;

    public 激光运行实例(Vector2 生成位置, Vector2 发射方向, 通用激光Pattern pattern, GameObject 传入对象, bool 传入预览)
    {
        Pattern = pattern;
        起始位置 = 生成位置;
        初始方向 = 发射方向.normalized;
        当前角度 = 0f;
        已运行时间 = 0f;
        
        this.是否包含预览阶段 = 传入预览;
        总持续时间 = 传入预览 ? Pattern.预览时间  + Pattern.持续时间: Pattern.持续时间;
        
        显示对象 = 传入对象;

        if (显示对象 != null)
        {
            直线显示 = 显示对象.GetComponent<I激光显示>();

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

        已运行时间 += dt;
        
        Vector2 当前方向 = 旋转方向();
        Color 当前颜色 = 获取当前颜色();
        
        直线显示?.更新显示(起始位置, 当前方向, Pattern, 当前颜色);

        bool 已进入攻击阶段 = !是否包含预览阶段 || 已运行时间 >= Pattern.预览时间;
        
        if(已进入攻击阶段)
        {
            检查玩家命中();
        }
        
        if(已运行时间 >= 总持续时间)
        {
            结束();
            return;
        }
        
        if (Pattern.旋转速度 != 0f)
        {
            当前角度 += Pattern.旋转速度 * dt;
        }
    }

    private Color 获取当前颜色()
    {
        float 渐变时长 = Mathf.Max(Pattern.渐变时间, 0f);
        
        if(!是否包含预览阶段)
        {
            return 获取攻击阶段颜色(已运行时间, Pattern.持续时间, Pattern.激光颜色, 渐变时长);
        }

        if (已运行时间 < Pattern.预览时间)
        {
            return 获取预览阶段颜色(已运行时间, Pattern.激光预览颜色, 渐变时长);
        }
        
        float 攻击阶段时间 = 已运行时间 - Pattern.预览时间;
        
        return 获取攻击阶段颜色(攻击阶段时间, Pattern.持续时间, Pattern.激光颜色, 渐变时长);
    }
    
    private Color 获取攻击阶段颜色(float 当前时间, float 攻击持续时间, Color 目标颜色, float 渐变时长)
    {
        if (渐变时长 <= 0f)
        {
            return 目标颜色;
        }

        if (攻击持续时间 <= 0f)
        {
            return Color.clear;
        }

        float 透明比例 = 1f;
        float 剩余时间 = 攻击持续时间 - 当前时间;

        if (剩余时间 <= 渐变时长)
        {
            透明比例 = 剩余时间 / 渐变时长;
        }
        
        Color 当前颜色 = 目标颜色;
        当前颜色.a *= 透明比例;
        return 当前颜色;
    }
    
    private Color 获取预览阶段颜色(float 当前时间, Color 目标颜色, float 渐变时长)
    {
        if (渐变时长 <= 0f)
        {
            return 目标颜色;
        }
        
        float 透明比例 = Mathf.Clamp01(当前时间 / 渐变时长);

        Color 当前颜色 = 目标颜色;
        当前颜色.a *= 透明比例;
        return 当前颜色;
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
            对象池.Instance.归还对象(Pattern.激光名称, 显示对象);
        }
    }
    
}
