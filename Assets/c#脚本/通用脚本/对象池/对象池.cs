using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I池对象重置
{
    void 池对象重置();
}


public sealed class 对象池获取请求
{
    public bool 已完成;
    public bool 成功;
    public GameObject 对象;

    public string 对象名;
    public GameObject 预制体;
    public Vector2 位置;
    public Quaternion 生成旋转;
}


public class 对象池 : MonoBehaviour
{
    public static 对象池 Instance {  get; private set; }

    private Transform 池父节点;
    private Dictionary<string, 对象池数据> 池字典 = new Dictionary<string, 对象池数据>();
    
    private readonly Queue<对象池获取请求> 获取请求队列 = new Queue<对象池获取请求>();

    [SerializeField]
    private int 每帧最大激活数量 = 20;
    
    public bool 已就绪 = true;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            池父节点 = new GameObject("对象池容器").transform;
            DontDestroyOnLoad(池父节点.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if(!已就绪) return;

        int 本帧处理数量 = 0;

        while (本帧处理数量 < 每帧最大激活数量 &&
               获取请求队列.Count > 0)
        {
            对象池获取请求 请求 = 获取请求队列.Dequeue();
            
            请求.对象 = 获取对象(请求.对象名, 请求.预制体, 请求.位置, 请求.生成旋转);
            请求.成功 = 请求.对象 != null;
            请求.已完成 = true;

            本帧处理数量++;
        }
    }

    public void 预加载对象(string 对象名, GameObject 预制体, int 预加载数量)
    {
        已就绪 = false;
        if(!池字典.ContainsKey(对象名))
        {
            池字典.Add(对象名, new 对象池数据());
        }

        Transform 目标父节点 = 获取分类父节点(对象名);

        for(int i = 0; i < 预加载数量; i++)
        {
            GameObject 新对象 = Instantiate(预制体, 目标父节点);
            新对象.SetActive(false);
            池字典[对象名].池队列.Enqueue(新对象);
        }
        已就绪 = true;
    }

    private Transform 获取分类父节点(string 对象名)
    {
        if (!池字典.ContainsKey(对象名))
        {
            池字典.Add(对象名, new 对象池数据());
        }

        if (池字典.ContainsKey(对象名) && 池字典[对象名].分类父节点 != null)
        {
            return 池字典[对象名].分类父节点;
        }

        GameObject 分类容器 = new GameObject(对象名 + "类");
        分类容器.transform.SetParent(池父节点);
        池字典[对象名].分类父节点 = 分类容器.transform;
        return 分类容器.transform;
    }

    public 对象池获取请求 异步获取对象(string 对象名, GameObject 预制体, Vector2 生成位置, Quaternion 生成旋转)
    {
        对象池获取请求 请求 = new 对象池获取请求
        {
            对象名 = 对象名,
            预制体 = 预制体,
            位置 = 生成位置,
            生成旋转 = 生成旋转
        };

        if (string.IsNullOrEmpty(对象名) || 预制体 == null)
        {
            请求.已完成 = true;
            请求.成功 = false;
            return 请求;
        }
        
        获取请求队列.Enqueue(请求);
        return 请求;
    }


    public GameObject 获取对象(string 对象名, GameObject 预制体, Vector2 生成位置, Quaternion 生成旋转)
    {
        if (!已就绪) return null;
        
        if(池字典.ContainsKey(对象名) && 池字典[对象名].池队列.Count > 0)
        {
            GameObject 闲置对象 = 池字典[对象名].池队列.Dequeue();
            return 激活对象(对象名, 闲置对象, 生成位置, 生成旋转);
        }

        if (预制体 != null)
        {
            GameObject 新对象 = Instantiate(预制体, 获取分类父节点(对象名));
            新对象.SetActive(false);
            return 激活对象(对象名, 新对象, 生成位置, 生成旋转);
        }

        return null;
    }

    private GameObject 激活对象(string 对象名, GameObject 对象, Vector2 生成位置, Quaternion 生成旋转)
    {
        对象.transform.position = 生成位置;
        对象.transform.rotation = 生成旋转;
        对象.transform.SetParent(获取分类父节点(对象名));

        if (对象.TryGetComponent<I池对象重置>(out var 重置接口))
        {
            重置接口.池对象重置();
        }

        if (池字典[对象名].活跃对象列表 == null)
        {
            池字典[对象名].活跃对象列表 = new List<GameObject>();
        }
        池字典[对象名].活跃对象列表.Add(对象);

        检查容量(对象名);

        对象.SetActive(true);
        return 对象;
    }

    public void 归还对象(string 对象名, GameObject 归还对象)
    {
        if (!池字典.ContainsKey(对象名) || 池字典[对象名].活跃对象列表 == null || !池字典[对象名].活跃对象列表.Contains(归还对象))
        {
            return;
        }

        池字典[对象名].活跃对象列表.Remove(归还对象);

        归还对象.SetActive(false);
        归还对象.transform.SetParent(获取分类父节点(对象名));


        if (!池字典.ContainsKey(对象名))
        {
            池字典.Add(对象名, new 对象池数据());
        }

        池字典[对象名].池队列.Enqueue(归还对象);
    }

    public List<GameObject> 获取活跃对象列表(string 对象名)
    {
        if (池字典.ContainsKey(对象名) && 池字典[对象名].活跃对象列表 != null)
        {
            return 池字典[对象名].活跃对象列表;
        }

        return new List<GameObject>();
    }

    public void 设置最大容量(string 对象名, int 最大容量)
    {
        if (!池字典.ContainsKey(对象名))
        {
            池字典.Add(对象名, new 对象池数据());
        }

        池字典[对象名].最大容量 = 最大容量;
    }

    private void 检查容量(string 对象名)
    {
        if (!池字典.ContainsKey(对象名) || 池字典[对象名].活跃对象列表 == null)
        {
            return;
        }

        int 最大容量 = 池字典[对象名].最大容量;
        int 当前数量 = 池字典[对象名].活跃对象列表.Count;

        if(当前数量 >= 最大容量)
        {
            GameObject 最早激活对象 = 池字典[对象名].活跃对象列表[0];
            归还对象(对象名, 最早激活对象);
        }
    }

    public void 清空指定池(string 对象名)
    {
        if (!池字典.ContainsKey(对象名)) return;

        对象池数据 数据 = 池字典[对象名];

        if (数据.池队列 != null)
        {
            while (数据.池队列.Count > 0)
            {
                Destroy(数据.池队列.Dequeue());
            }
        }

        if (数据.活跃对象列表 != null)
        {
            foreach (var 活跃对象 in 数据.活跃对象列表)
            {
                if (活跃对象 != null) Destroy(活跃对象);
            }
            数据.活跃对象列表.Clear();
        }

        if (数据.分类父节点 != null)
        {
            Destroy(数据.分类父节点.gameObject);
        }

        池字典.Remove(对象名);
    }

    public void 清空所有池()
    {
        List<string> 池名称列表 = new List<string>(池字典.Keys);

        foreach (string 对象名 in 池名称列表)
        {
            清空指定池(对象名);
        }
    }
    
    public void 归还指定池(string 对象名)
    {
        if (!池字典.TryGetValue(对象名, out 对象池数据 数据))
        {
            return;
        }

        if (数据.活跃对象列表 == null ||
            数据.活跃对象列表.Count == 0)
        {
            return;
        }

        List<GameObject> 待归还对象列表 =
            new List<GameObject>(数据.活跃对象列表);

        foreach (GameObject 对象 in 待归还对象列表)
        {
            if (对象 != null)
            {
                归还对象(对象名, 对象);
            }
        }
    }
    
    public void 归还所有对象()
    {
        List<string> 对象名列表 =
            new List<string>(池字典.Keys);

        foreach (string 对象名 in 对象名列表)
        {
            归还指定池(对象名);
        }
    }
}
