using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 传送门 : MonoBehaviour
{
    public string 目标场景名;
    public string 目标出生点id;

    void OnTriggerEnter2D(Collider2D 其他触发器)
    {
        if (其他触发器.CompareTag("玩家"))
        {
            GameManager.Instance.StartCoroutine(GameManager.Instance.传送玩家(目标场景名, $"出生点{目标出生点id}"));
        }
    }
}
