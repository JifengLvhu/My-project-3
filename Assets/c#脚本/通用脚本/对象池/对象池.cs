using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I池对象重置
{
    void 池对象重置();
}

public class 对象池 : MonoBehaviour
{
    public static 对象池 Instance {  get; private set; }

    private Transform 池父节点;
    private Dictionary<string, 对象池数据> 池字典 = new Dictionary<string, 对象池数据>();

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

    public void 预加载对象(string 对象名, GameObject 预制体, int 预加载数量)
    {
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


    public GameObject 获取对象(string 对象名, GameObject 预制体, Vector3 生成位置, Quaternion 生成旋转)
    {
        if(池字典.ContainsKey(对象名) && 池字典[对象名].池队列.Count > 0)
        {
            GameObject 闲置对象 = 池字典[对象名].池队列.Dequeue();
            闲置对象.transform.position = 生成位置;
            闲置对象.transform.rotation = 生成旋转;
            闲置对象.transform.SetParent(获取分类父节点(对象名));

            if (闲置对象.TryGetComponent<I池对象重置>(out var 重置接口))
            {
                重置接口.池对象重置();
            }

            if (池字典[对象名].活跃对象列表 == null)
            {
                池字典[对象名].活跃对象列表 = new List<GameObject>();
            }
            池字典[对象名].活跃对象列表.Add(闲置对象);

            检查容量(对象名);

            闲置对象.SetActive(true);
            return 闲置对象;
        }

        if (预制体 != null)
        {
            return Instantiate(预制体, 生成位置, 生成旋转, 获取分类父节点(对象名));
        }

        return null;
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

}
