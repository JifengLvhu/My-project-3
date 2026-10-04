using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 行为调度器 : MonoBehaviour
{
    public static 行为调度器 Instance { get; private set; }
    
    readonly List<激光运行实例> 激光实例列表 = new List<激光运行实例>();
    readonly List<激光预览实例> 激光预览实例列表 = new List<激光预览实例>();
    
    readonly List<推拉预览实例> 推拉预览实例列表 = new List<推拉预览实例>();
    
    readonly List<动静剑预览实例> 动静剑预览实例列表 = new List<动静剑预览实例>();
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        for(int i = 激光实例列表.Count - 1; i >= 0; i--)
        {
            激光运行实例 实例 = 激光实例列表[i];
            实例.更新(Time.deltaTime);
            if(实例.已完成)
            {
                激光实例列表.RemoveAt(i);
            }
        }
        
        for(int i = 激光预览实例列表.Count - 1; i >= 0; i--)
        {
            激光预览实例 实例 = 激光预览实例列表[i];
            实例.更新(Time.deltaTime);
            if(实例.已完成)
            {
                激光预览实例列表.RemoveAt(i);
            }
        }
        
        for (int i = 推拉预览实例列表.Count - 1; i >= 0; i--)
        {
            推拉预览实例 实例 = 推拉预览实例列表[i];
            实例.更新(Time.fixedDeltaTime);
            if (实例.已完成)
            {
                推拉预览实例列表.RemoveAt(i);
            }
        }
    }
    
    private void FixedUpdate()
    {
        for (int i = 动静剑预览实例列表.Count - 1; i >= 0; i--)
        {
            动静剑预览实例 实例 = 动静剑预览实例列表[i];
            实例.更新状态();
            if (实例.已完成)
            {
                动静剑预览实例列表.RemoveAt(i);
            }
        }
    }

    private void LateUpdate()
    {
        for (int i = 动静剑预览实例列表.Count - 1; i >= 0; i--)
        {
            动静剑预览实例 实例 = 动静剑预览实例列表[i];
            实例.更新位置();
            if (实例.已完成)
            {
                动静剑预览实例列表.RemoveAt(i);
            }
        }
    }

    public void 发射(Vector2 生成位置, Vector2 发射方向, IPattern pattern)
    {
        if(pattern == null) return;
        
        if(pattern is 通用弹幕Pattern 子弹Pattern)
        {
            StartCoroutine(发射子弹协程(生成位置, 发射方向, 子弹Pattern));
        }
        else if (pattern is 通用激光Pattern 激光Pattern)
        {
            StartCoroutine(发射激光全流程协程(生成位置, 发射方向, 激光Pattern));
        }
        else
        {
            运行特效(生成位置, 发射方向, pattern);
        }
    }

    private IEnumerator 发射子弹协程(Vector2 生成位置,Vector2 发射方向,通用弹幕Pattern pattern)
    {
        yield return new WaitUntil(() => 对象池.Instance.已就绪);
        对象池获取请求 请求 = 对象池.Instance.异步获取对象(pattern.子弹名称, pattern.子弹预制体, 生成位置, Quaternion.identity);
        yield return new WaitUntil(() => 请求.已完成);
        if(!请求.成功) yield break;
        GameObject 子弹 = 请求.对象;
        if(子弹 == null) yield break;

        I轨迹 轨迹 = pattern.创建轨迹(子弹.transform.position,发射方向);
        var 基础子弹脚本 = 子弹.GetComponent<基础子弹>();
        基础子弹脚本.初始化(pattern);
        基础子弹脚本.发射(轨迹);
            
        yield return null;
    }
        
    //单独激光发射，包含预览阶段
    private IEnumerator 发射激光全流程协程(Vector2 生成位置, Vector2 发射方向, 通用激光Pattern pattern)
    {
        yield return StartCoroutine(发射激光协程(生成位置, 发射方向, pattern, true));
    }
    
    public void 正式发射激光(Vector2 生成位置, Vector2 发射方向, 通用激光Pattern pattern)
    {
        if (pattern == null)
        {
            return;
        }

        StartCoroutine(发射激光协程(生成位置, 发射方向, pattern, false));
    }
    
    private IEnumerator 发射激光协程(Vector2 生成位置, Vector2 发射方向, 通用激光Pattern pattern, bool 是否包含预览阶段)
    {
        yield return new WaitUntil(() => 对象池.Instance.已就绪);
        
        对象池获取请求 请求 = 对象池.Instance.异步获取对象(pattern.激光名称, pattern.激光显示预制体, 生成位置, Quaternion.identity);
        yield return new WaitUntil(() => 请求.已完成);
        if(!请求.成功) yield break;
        
        激光运行实例 实例 = new 激光运行实例(生成位置, 发射方向, pattern, 请求.对象, 是否包含预览阶段);
        激光实例列表.Add(实例);
    }
    
    public void 预览激光(Vector2 生成位置, Vector2 发射方向, 通用激光Pattern pattern)
    {
        if (pattern == null)
        {
            return;
        }

        StartCoroutine(发射激光预览协程(生成位置, 发射方向, pattern));
    }
    
    private IEnumerator 发射激光预览协程(Vector2 生成位置, Vector2 发射方向, 通用激光Pattern pattern)
    {
        yield return new WaitUntil(() => 对象池.Instance.已就绪);
        
        对象池获取请求 请求 = 对象池.Instance.异步获取对象(pattern.激光名称, pattern.激光显示预制体, 生成位置, Quaternion.identity);
        yield return new WaitUntil(() => 请求.已完成);
        if(!请求.成功) yield break;
        
        激光预览实例 实例 = new 激光预览实例(生成位置, 发射方向, pattern, 请求.对象);
        激光预览实例列表.Add(实例);
    }
    
    private void 运行特效(Vector2 生成位置,Vector2 发射方向, IPattern pattern)
    {
        if(pattern is 推拉玩家Pattern 推拉Pattern)
        {
            StartCoroutine(运行推拉玩家特效协程(生成位置, 推拉Pattern));
        }
        else if(pattern is 动静剑Pattern 动静Pattern)
        {
            StartCoroutine(运行动静剑特效协程(动静Pattern));
        }
    }

    private IEnumerator 运行推拉玩家特效协程(Vector2 作用点, 推拉玩家Pattern 推拉Pattern)
    {
        
        yield return new WaitUntil(() => 对象池.Instance.已就绪);
        
        int 箭头数量 = Mathf.Max(推拉Pattern.预览箭头数量, 0);
        
        List<GameObject> 箭头列表 = new List<GameObject>();
        
        for(int i = 0; i < 箭头数量; i++)
        {
            float 角度 = i * 360f / Mathf.Max(1, 箭头数量);

            Vector2 径向方向 = Quaternion.Euler(0f, 0f, 角度) * Vector2.right;

            Vector2 初始位置 = 作用点 + 径向方向 * 推拉Pattern.预览箭头半径;
            对象池获取请求 请求 = 对象池.Instance.异步获取对象(推拉Pattern.预览箭头名称, 推拉Pattern.预览箭头预制体, 初始位置, Quaternion.identity);
            yield return new WaitUntil(() => 请求.已完成);
            if(!请求.成功) yield break;
            箭头列表.Add(请求.对象);
        }
        
        推拉预览实例 预览实例 = new 推拉预览实例(作用点, 推拉Pattern, 箭头列表);
        推拉预览实例列表.Add(预览实例);
        
        yield return new WaitForSeconds(推拉Pattern.预览时间);
        预览实例.结束();
        
        Vector2 玩家位置 = 玩家.Instance.transform.position;  
        if(推拉Pattern.是否拉力)
        {
            玩家.Instance.收到推拉(作用点 - 玩家位置, 推拉Pattern.距离);
        }
        else
        {       
            玩家.Instance.收到推拉(玩家位置 - 作用点, 推拉Pattern.距离);
        }
    }
    
    private IEnumerator 运行动静剑特效协程(动静剑Pattern 动静Pattern)
    {
        yield return new WaitUntil(() => 对象池.Instance.已就绪);
        
        对象池获取请求 请求 = 对象池.Instance.异步获取对象(动静Pattern.动静剑预览名称, 动静Pattern.动静剑预览预制体, Vector2.zero, Quaternion.identity);
        yield return new WaitUntil(() => 请求.已完成);
        if(!请求.成功) yield break;
        
        动静剑预览实例 预览实例 = new 动静剑预览实例(动静Pattern, 请求.对象);
        动静剑预览实例列表.Add(预览实例);
        
        yield return new WaitForSeconds(动静Pattern.预览时间);
        float 计时器 = 0f;
        Rigidbody2D 玩家刚体 = 玩家.Instance.GetComponent<Rigidbody2D>();
        while (计时器 < 动静Pattern.持续时间)
        {
            if (动静Pattern.要求移动)
            {
                if(玩家刚体.velocity.magnitude < 0.1f)
                {
                    玩家.Instance.受到伤害(1);
                    break;
                }
            }
            else
            {
                if(玩家刚体.velocity.magnitude > 0.1f)
                {
                    玩家.Instance.受到伤害(1);
                    break;
                }
            }
            
            计时器 += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        
        预览实例.结束();
    }
    
}
