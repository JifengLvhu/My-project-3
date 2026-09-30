using System.Collections.Generic;
using UnityEngine;

public class 基础子弹 : MonoBehaviour, I池对象重置
{

    private int 伤害值 = 1;
    private Vector2 飞行方向 = Vector2.zero;
    public string 子弹名称;
    
    private I轨迹 轨迹;
    
    private List<弹幕事件> 子弹事件列表;
    private HashSet<弹幕事件> 已触发事件;
    private 通用弹幕Pattern 当前Pattern;

    void FixedUpdate()
    {
        if(轨迹 == null) return;
        Vector2 偏移 = 轨迹.移动(Time.fixedDeltaTime);
        transform.Translate(偏移,Space.World);
        
        float 沿中线总距离 = 轨迹.获取沿中线总距离();
        foreach (var 弹幕事件 in 子弹事件列表)
        {
            if(已触发事件.Contains(弹幕事件)) continue;
            
            if(沿中线总距离 >= 弹幕事件.触发距离)
            {
                执行事件(弹幕事件);
                已触发事件.Add(弹幕事件);
            }
        }
    }

    
    public void 发射(I轨迹 传入轨迹)
    {
        轨迹 = 传入轨迹;
        飞行方向 = 轨迹.获取方向();
    }

    private void OnTriggerEnter2D(Collider2D 其他触发器)
    {
        if (其他触发器.CompareTag("玩家"))
        {
            玩家.Instance.受到伤害(伤害值);
            对象池.Instance.归还对象(子弹名称, gameObject);
        }
    }

    public void 初始化(通用弹幕Pattern pattern)
    {
        当前Pattern = pattern;
        已触发事件 = new HashSet<弹幕事件>();
        子弹事件列表 = new List<弹幕事件>(pattern.弹幕事件列表);
    }

    public void 池对象重置()
    {
        轨迹 = null;
        if(已触发事件 != null)
        {
            已触发事件.Clear();
        }
        子弹事件列表 = null;
    }
    
    private void 执行事件(弹幕事件 弹幕事件)
    {
        Vector2 母方向 = 轨迹.获取方向();
        Vector2 发射原点 = transform.position;
        switch(弹幕事件.事件类型)
        {
            case 弹幕事件.弹幕事件类型.单颗发射:
            {   
                Quaternion 旋转 = Quaternion.Euler(0,0,弹幕事件.旋转角度);
                Vector2 子方向 = 旋转 * 母方向;
                弹幕发射器.Instance.发射(发射原点, 子方向, 弹幕事件.子弹幕Pattern);
                break;
            }
            
            case 弹幕事件.弹幕事件类型.扇形发射:
            {
                if(弹幕事件.发射数量 <= 1) break;

                float 起始角度 = 弹幕事件.扇形基准相对角度 - 弹幕事件.扇形总角度 / 2f;
                float 角度步长 = 弹幕事件.扇形总角度 / (弹幕事件.发射数量 - 1);
                for(int i = 0; i < 弹幕事件.发射数量; i++)
                {
                    float 当前角度 = 起始角度 + i * 角度步长;
                    Quaternion 旋转 = Quaternion.Euler(0,0,当前角度);
                    Vector2 子方向 = 旋转 * 母方向;
                    弹幕发射器.Instance.发射(发射原点, 子方向, 弹幕事件.子弹幕Pattern);
                }
                break;
            }

            case 弹幕事件.弹幕事件类型.空爆发射:
            {
                if(弹幕事件.发射数量 <= 1) break;

                float 角度步长 = 360f / 弹幕事件.发射数量;
                for(int i = 0; i < 弹幕事件.发射数量; i++)
                {
                    float 当前角度 = i * 角度步长;
                    Quaternion 旋转 = Quaternion.Euler(0,0,当前角度);
                    Vector2 子方向 = 旋转 * 母方向;
                    弹幕发射器.Instance.发射(发射原点, 子方向, 弹幕事件.子弹幕Pattern);
                }
                break;
            }
        }
    }
}
