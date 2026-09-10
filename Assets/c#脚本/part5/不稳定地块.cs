using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 不稳定地块 : MonoBehaviour
{
    private Rigidbody2D 玩家刚体;

    public bool 已被砸碎 = false;
    public float 玩家下砸反弹高度 = 17f;
    public float 上涨程度;

    void Start()
    {
        玩家刚体 = 玩家.Instance.GetComponent<Rigidbody2D>();
    }

    void OnTriggerEnter2D(Collider2D 其他触发器)
    {
        if(已被砸碎) return;
        if (其他触发器.CompareTag("玩家"))
        {
            if (玩家.Instance.正在下砸)
            {
                已被砸碎 = true;
                岩浆池[] 所有岩浆池 = FindObjectsByType<岩浆池>(FindObjectsSortMode.None);
                foreach (var 岩浆池实例 in 所有岩浆池)
                {
                    岩浆池实例.喷涌(上涨程度);
                }
                玩家刚体.velocity = new Vector2(玩家刚体.velocity.x, 玩家下砸反弹高度);
            }
        }
    }
}
