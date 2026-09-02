using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part2下水道管理器 : MonoBehaviour, I章节管理器
{
    public object 获取章节存档数据()
    {
        存档管理器.Part2存档数据 Part2存档 = new 存档管理器.Part2存档数据();

        水池[] 所有水池 = FindObjectsOfType<水池>();

        foreach (水池 水池实例 in 所有水池)
        {
            存档管理器.水池存档单元 单元 = new 存档管理器.水池存档单元();
            单元.水池ID = 水池实例.水池ID;
            单元.当前水位高度 = 水池实例.当前水位;
            Part2存档.水池列表.Add(单元);
        }

        Debug.Log($"Part2管理器：收集了 {Part2存档.水池列表.Count} 个水池的数据");
        return Part2存档;
    }

    public void 应用章节存档数据(object 数据)
    {
        存档管理器.Part2存档数据 Part2存档 = 数据 as 存档管理器.Part2存档数据;
        if (Part2存档 == null) return;

        水池[] 所有水池 = FindObjectsOfType<水池>();
        Dictionary<int,水池> 水池字典 = new Dictionary<int,水池>();

        Debug.Log($"场景中找到 {所有水池.Length} 个水池");

        foreach (水池 水池实例 in 所有水池)
        {
            水池字典.Add(水池实例.水池ID, 水池实例);
            Debug.Log($"注册水池 ID: {水池实例.水池ID}");
        }

        foreach (存档管理器.水池存档单元 单元 in Part2存档.水池列表)
        {
            if (水池字典.TryGetValue(单元.水池ID, out 水池 目标水池))
            {
                目标水池.设置水位(单元.当前水位高度);
            }
        }
        Debug.Log($"Part2管理器：已恢复 {Part2存档.水池列表.Count} 个水池的水位");

    }
}
