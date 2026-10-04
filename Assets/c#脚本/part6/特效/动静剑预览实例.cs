using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class 动静剑预览实例
{
    private GameObject 预览对象根节点;
    private TMP_Text 预览对象文本;
    private SpriteRenderer 预览对象渲染器;
    private Rigidbody2D 玩家刚体;
    private 动静剑Pattern Pattern;
    private bool 是否要求移动;
    public bool 已完成;
    
    public 动静剑预览实例(动静剑Pattern pattern, GameObject 传入对象)
    {
        玩家刚体 = 玩家.Instance.GetComponent<Rigidbody2D>();
        
        预览对象根节点 = 传入对象;
        预览对象根节点.transform.position = 玩家.Instance.transform.position;
        
        预览对象文本 = 预览对象根节点.GetComponentInChildren<TMP_Text>();
        预览对象渲染器 = 预览对象根节点.GetComponentInChildren<SpriteRenderer>();
        
        Pattern = pattern;
        是否要求移动 = pattern.要求移动;
        已完成 = false;

        if (是否要求移动)
        {
            预览对象文本.text = "保持移动！";
        }
        else
        {
            预览对象文本.text = "保持静止！";
        }
    }
    
    public void 更新位置()
    {
        if (已完成 || 预览对象根节点 == null)
        {
            return;
        }

        预览对象根节点.transform.position = 玩家.Instance.transform.position;
    }
    
    public void 更新状态()
    {
        if (已完成 || 玩家刚体 == null)
        {
            return;
        }

        Color 目标颜色;

        if (是否要求移动)
        {
            目标颜色 = 玩家刚体.velocity.magnitude < 0.1f ? Color.red : Color.green;
        }
        else
        {
            目标颜色 = 玩家刚体.velocity.magnitude < 0.1f ? Color.green : Color.red;
        }

        目标颜色.a = 0.8f;
        预览对象渲染器.color = 目标颜色;
    }

    public void 结束()
    {
        if(已完成)
        {
            return;
        }   
        
        对象池.Instance.归还对象(Pattern.动静剑预览名称, 预览对象根节点);
        
        已完成 = true;
    }
}
