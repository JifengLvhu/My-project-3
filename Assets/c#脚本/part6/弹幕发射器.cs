using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 弹幕发射器 : MonoBehaviour
{
    public static 弹幕发射器 Instance { get; private set; }
    
    public readonly List<激光运行实例> 激光实例列表 = new List<激光运行实例>();
    
    void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
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
            StartCoroutine(发射激光协程(
                生成位置,
                发射方向,
                激光Pattern));
        }
    }

    private IEnumerator 发射子弹协程(Vector2 生成位置,Vector2 发射方向,通用弹幕Pattern pattern)
    {
        for(int i = 0; i < pattern.每轮数量; i++)
        {
            yield return new WaitUntil(() => 对象池.Instance.已就绪);
            对象池获取请求 请求 = 对象池.Instance.异步获取对象(pattern.子弹名称, pattern.子弹预制体, 生成位置, Quaternion.identity);
            yield return new WaitUntil(() => 请求.已完成);
            if(!请求.成功)yield break;
            GameObject 子弹 = 请求.对象;
            if(子弹 == null) continue;

            I轨迹 轨迹 = pattern.创建轨迹(子弹.transform.position,发射方向);
            var 基础子弹脚本 = 子弹.GetComponent<基础子弹>();
            基础子弹脚本.初始化(pattern);
            基础子弹脚本.发射(轨迹);
            
            yield return null;
        }
    }
    
    private IEnumerator 发射激光协程(Vector2 生成位置, Vector2 发射方向, 通用激光Pattern pattern)
    {
        yield return new WaitUntil(() => 对象池.Instance.已就绪);
        
        对象池获取请求 请求 = 对象池.Instance.异步获取对象(pattern.激光名称, pattern.激光显示预制体, 生成位置, Quaternion.identity);
        yield return new WaitUntil(() => 请求.已完成);
        if(!请求.成功) yield break;
        
        激光运行实例 实例 = new 激光运行实例(生成位置, 发射方向, pattern, 请求.对象);
        激光实例列表.Add(实例);
    }
}
