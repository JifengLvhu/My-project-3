using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 对象池数据
{
    public Transform 分类父节点;
    public Queue<GameObject> 池队列 = new Queue<GameObject>();
    public int 最大容量 = 50;
    public List<GameObject> 活跃对象列表 = new List<GameObject>();
}