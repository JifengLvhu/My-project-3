using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class part6太空管理器 : MonoBehaviour
{
    public GameObject 基础子弹预制体;
    public GameObject 基础激光预制体;
    public GameObject 钢铁激光预制体;
    
    public Vector2 旧全局重力;
    void Start()
    {
        对象池.Instance.预加载对象("基础子弹", 基础子弹预制体, 1200);
        对象池.Instance.设置最大容量("基础子弹", 1200);
        
        对象池.Instance.预加载对象("直线激光", 基础激光预制体, 40);
        对象池.Instance.设置最大容量("直线激光", 40);
        
        对象池.Instance.预加载对象("钢铁激光", 钢铁激光预制体, 40);
        对象池.Instance.设置最大容量("钢铁激光", 40);
        
        旧全局重力 = Physics2D.gravity;
        Physics2D.gravity = Vector2.zero;
    }
    
    void OnDisable()
    {
        对象池.Instance.清空指定池("基础子弹");
        对象池.Instance.清空指定池("直线激光");
        对象池.Instance.清空指定池("钢铁激光");
        
        Physics2D.gravity = 旧全局重力;
    }
    
    public object 获取章节存档数据()
    {
        Debug.Log("Part6管理器：无章节专属数据，返回null");
        return null;
    }

    public void 应用章节存档数据(object 数据)
    {
        Debug.Log("Part6管理器：收到数据，无需应用");
    }
}
