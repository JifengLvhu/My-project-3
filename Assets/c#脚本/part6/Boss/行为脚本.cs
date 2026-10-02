using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 行为脚本 : MonoBehaviour
{
    public List<行为段> 行为段列表;

    private int 当前段索引;
    private float 当前段计时器;
    private int 当前段运行轮数;
    private bool 正在等待;
    private bool 正在执行;
    private bool 全部完成;

    
    private void Start()
    {
        if (行为段列表 == null || 行为段列表.Count == 0)
        {
            enabled = false;
            return;
        }

        当前段索引 = -1;
        当前段计时器 = 0f;
        切换到下一段();
    }

    private void Update()
    {
        if (正在等待)
        {
            当前段计时器 += Time.deltaTime;
            if (当前段计时器 >= 行为段列表[当前段索引].等待时间)
            {
                正在等待 = false;
                切换到下一段();
            }
        }
        
        if(正在执行)
        {
            当前段计时器 += Time.deltaTime;
            全部完成 = true;
            foreach (var 配置 in 行为段列表[当前段索引].配置列表 )
            {
                if(配置.是否完成) continue;
                if(配置.正在运行)
                {
                    全部完成 = false;
                    continue;
                }
                if (配置.发射轮数 <= 0 )
                {
                    配置.是否完成 = true;
                    continue;
                }
                
                if(当前段计时器 >= 配置.首轮延迟 + 配置.已发射轮数 * 配置.发射间隔)
                {
                    运行配置(配置);
                    if(配置.已发射轮数 >= 配置.发射轮数)
                    {
                        配置.是否完成 = true;
                    }
                }
                if (!配置.是否完成)
                {
                    全部完成 = false;
                }
            }

            if (全部完成)
            {
                正在执行 = false;
                切换到下一段();
            }
        }
    }
    
    private void 切换到下一段()
    {
        if(当前段索引 < 行为段列表.Count - 1)
        {
            当前段索引++;
            当前段计时器 = 0f;
            当前段运行轮数 = 0;
        }
        else
        {
            return;
        }

        if (行为段列表[当前段索引].行为类型 == 行为段.类型.等待)
        {
            正在等待 = true;
        }
        else if(行为段列表[当前段索引].行为类型 == 行为段.类型.动作)
        {
            全部完成 = false;
            正在执行 = true;
        }
        
        
    }
    
    private void 运行配置(配置 传入配置)
    {
        if (传入配置.pattern == null)
        {
            Debug.LogError("行为配置没有设置 Pattern。", this);
            传入配置.是否完成 = true;
            return;
        }

        IPattern Pattern = 传入配置.pattern as IPattern;

        if (Pattern == null)
        {
            Debug.LogError(
                $"对象 {传入配置.pattern.name} 没有实现 IPattern。",
                传入配置.pattern);

            传入配置.是否完成 = true;
            return;
        }
        
        传入配置.已发射轮数++;
        Vector2 Boss世界坐标 = transform.position;
        
        if(Pattern is 预知未来Pattern 预知Pattern)
        {
            StartCoroutine(运行预知未来协程(传入配置, 预知Pattern));
            return;
        }
        
        foreach(var 发射配置 in Pattern.获取发射位置列表(Boss世界坐标))
        {
            switch (Pattern.类型)
            {
                case IPattern.Pattern类型.子弹:
                    通用弹幕Pattern 子弹Pattern = Pattern as 通用弹幕Pattern;
                    行为调度器.Instance.发射(发射配置.发射位置, 发射配置.发射方向, 子弹Pattern);
                    break;
            
                case IPattern.Pattern类型.激光:
                    通用激光Pattern 激光Pattern = Pattern as 通用激光Pattern;
                    行为调度器.Instance.发射(发射配置.发射位置, 发射配置.发射方向, 激光Pattern);
                    break;
                
                case IPattern.Pattern类型.特效:
                    行为调度器.Instance.发射(发射配置.发射位置, 发射配置.发射方向, Pattern);
                    break;
            }
        }
    }

    private IEnumerator 运行预知未来协程(配置 传入配置, 预知未来Pattern 预知Pattern)
    {
        传入配置.正在运行 = true;
        
        if (预知Pattern.激光列表 == null || 预知Pattern.激光列表.Count == 0)
        {
            Debug.LogWarning("预知未来没有配置激光列表。", 预知Pattern);

            传入配置.正在运行 = false;
            传入配置.是否完成 = true;
            yield break;
        }
        
        Vector2 Boss世界坐标 = transform.position;
        
        //预览激光列表中每个激光的攻击
        foreach(var 激光Pattern in 预知Pattern.激光列表)
        {
            if (激光Pattern == null)
            {
                continue;
            }
            
            foreach(var 发射配置 in 激光Pattern.获取发射位置列表(Boss世界坐标))
            {
                行为调度器.Instance.预览激光(发射配置.发射位置, 发射配置.发射方向, 激光Pattern);
            }
            
            yield return new WaitForSeconds(激光Pattern.预览时间 + 0.5f);
        }
        
        //发射激光列表中每个激光的攻击
        foreach(var 激光Pattern in 预知Pattern.激光列表)
        {
            if (激光Pattern == null)
            {
                continue;
            }
            
            foreach(var 发射配置 in 激光Pattern.获取发射位置列表(Boss世界坐标))
            {
                行为调度器.Instance.正式发射激光(发射配置.发射位置, 发射配置.发射方向, 激光Pattern);
            }
            
            yield return new WaitForSeconds(激光Pattern.持续时间 + 0.5f);
        }
        传入配置.正在运行 = false;
        传入配置.是否完成 = true;
    }
}
