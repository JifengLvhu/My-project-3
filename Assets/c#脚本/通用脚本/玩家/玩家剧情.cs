using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 玩家剧情 : MonoBehaviour
{
    public List<string> 已触发剧情列表 = new List<string>();

    public bool 是否已触发(string 剧情名称)
    {
        return 已触发剧情列表.Contains(剧情名称);
    }

    public void 触发剧情(string 剧情名称)
    {
        if (!是否已触发(剧情名称))
        {
            已触发剧情列表.Add(剧情名称);
        }
        // 剧情触发函数预留
    }

    public List<string> 获取已触发剧情列表()
    {
        return new List<string>(已触发剧情列表);
    }

    public void 应用存档数据(List<string> 存档列表)
    {
        已触发剧情列表 = new List<string>(存档列表);
    }
}
