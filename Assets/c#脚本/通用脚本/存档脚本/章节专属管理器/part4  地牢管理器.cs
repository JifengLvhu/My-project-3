using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class part4地牢管理器 : MonoBehaviour, I章节管理器
{
    public GameObject 史莱姆预制体;

    void Start()
    {
        对象池.Instance.预加载对象("史莱姆", 史莱姆预制体, 30);
        对象池.Instance.设置最大容量("史莱姆", 30);
    }

    void OnDisable()
    {
        对象池.Instance.清空指定池("史莱姆");
    }

    public object 获取章节存档数据()
    {
        Debug.Log("Part4管理器：无章节专属数据，返回null");
        return null;
    }

    public void 应用章节存档数据(object 数据)
    {
        Debug.Log("Part4管理器：收到数据，无需应用");
    }

}
