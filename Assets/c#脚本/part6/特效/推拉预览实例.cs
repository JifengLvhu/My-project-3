using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 推拉预览实例 
{
    private readonly 推拉玩家Pattern Pattern;
    private readonly Vector2 作用点;
    private readonly List<GameObject> 箭头列表 = new List<GameObject>();
    
    private float 已运行时间;
    
    public bool 已完成;
    
    public 推拉预览实例(Vector2 生成位置, 推拉玩家Pattern pattern, List<GameObject> 传入箭头列表)
    {
        Pattern = pattern;
        作用点 = 生成位置;
        已运行时间 = 0f;
        箭头列表 = 传入箭头列表;
    }

    public void 更新(float dt)
    {
        if(已完成)
        {
            return;
        }

        已运行时间 += dt;

        float 摆动值 = Mathf.Sin(已运行时间 * Pattern.预览箭头移动速度);
        
        for(int i = 0; i < 箭头列表.Count; i++)
        {
            GameObject 箭头 = 箭头列表[i];
            if (箭头 == null)
            {
                continue;
            }
            
            float 角度 = i * 360f / Mathf.Max(1, 箭头列表.Count);
            
            Vector2 方向 = Quaternion.Euler(0, 0, 角度) * Vector2.right;
            
            float 当前半径 = Pattern.预览箭头半径 + 摆动值 * Pattern.预览箭头移动距离;

            箭头.transform.position = 作用点 + 方向 * 当前半径;

            设置箭头方向(箭头, 方向);
        }
    }

    private void 设置箭头方向(GameObject 箭头, Vector2 方向)
    {
        Vector2 箭头方向 = Pattern.是否拉力? -方向 : 方向;
        
        float 角度 = Mathf.Atan2(箭头方向.y, 箭头方向.x) * Mathf.Rad2Deg;
        
        //箭头默认朝上
        箭头.transform.rotation = Quaternion.Euler(0, 0, 角度 - 90f);
    }

    public void 结束()
    {
        if(已完成)
        {
            return;
        }
        
        已完成 = true;
        
        foreach(GameObject 箭头 in 箭头列表)
        {
            if (箭头 != null)
            {
                对象池.Instance.归还对象(Pattern.预览箭头名称, 箭头);
            }
        }
        箭头列表.Clear();
    }
}
