using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class part3城堡管理器 : MonoBehaviour, I章节管理器
{
    public object 获取章节存档数据()
    {
        Debug.Log("Part3管理器：无章节专属数据，返回null");
        return null;
    }

    public void 应用章节存档数据(object 数据)
    {
        Debug.Log("Part3管理器：收到数据，无需应用");
    }
}
