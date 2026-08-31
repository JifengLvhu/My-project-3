using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 狱警 : 基础小怪
{
    public float 追逐速度 = 6f;
    public float 检测距离;
    public float 脱离距离;
    public LayerMask 视野遮挡图层;
    public float 跳跃强度;
    private float 触地射线长度;
    public bool 眩晕状态 = false;
    private float 眩晕计时器 = 0f;


    public bool 已发现玩家 = false;
    private Transform 玩家位置;

    protected override void Start()
    {
        base.Start();
        玩家位置 = 玩家.Instance.transform;
        触地射线长度 = 自身碰撞体.bounds.extents.y * Mathf.Sqrt(2) - 自身碰撞体.bounds.extents.y / 7;
        最大生命值 = 9999;
        当前生命值 = 最大生命值;
        检测距离 = 5f;
        脱离距离 = 20f;
        跳跃强度 = 15f;
    }

    private void Update()
    {
        检测玩家();
    }

    private void FixedUpdate()
    {
        if (眩晕状态)
        {
            自身刚体.velocity = new Vector2(0, 自身刚体.velocity.y);
        }
        else
        {
            if (已发现玩家)
            {
                追逐玩家();
            }
            else
            {
                换向判断();
                左右移动();
            }
        }
        

    }

    protected override void OnTriggerStay2D(Collider2D 其他触发器)
    {
        if(!眩晕状态)
        {
            base.OnTriggerStay2D(其他触发器);
        }
    }

    private void 检测玩家()
    {
        float 距离 = Vector2.Distance(玩家位置.position, transform.position);

        if (距离 <= 检测距离)
        {
            Vector2 射线方向 = (玩家位置.position - transform.position).normalized;
            RaycastHit2D 射线检测 = Physics2D.Raycast(transform.position, 射线方向, 距离, 视野遮挡图层);

            if (射线检测.collider == null || 射线检测.collider.tag == "玩家")
            {
                已发现玩家 = true;
                return;
            }
        }
        else if(距离 > 脱离距离)
        {
            已发现玩家 = false;
        }
    }

    private void 追逐玩家()
    {
        if (transform.position.x - 玩家位置.position.x < -0.3f)
        {
            横向输入 = 1f;
        }
        else if (transform.position.x - 玩家位置.position.x > 0.3f)
        {
            横向输入 = -1f;
        }
        else
        {
            横向输入 = 0f;
        }

            自身刚体.velocity = new Vector2(横向输入 * 追逐速度, 自身刚体.velocity.y);

        if (横向输入 != 0 && (前方障碍物检测() || !前方地面检测()))
        {
            跳跃();
        }
    }

    private bool 触地检测()
    {
        Vector2 射线起点 = (Vector2)transform.position;
        RaycastHit2D 射线检测 = Physics2D.Raycast(射线起点, Vector2.down, 触地射线长度, 地面);
        return 射线检测.collider != null;
    }

    private void 跳跃()
    {
        if(触地检测())
        {
            自身刚体.velocity = new Vector2(自身刚体.velocity.x, 跳跃强度);
        }
    }

    public override void 收到伤害(int 伤害值)
    {
        base.收到伤害(伤害值);
        眩晕(3f);
    }

    private void 眩晕(float 持续时间)
    {
        Debug.Log($"眩晕{持续时间}s");
        眩晕状态 = true;
        眩晕计时器 = 持续时间;
        StartCoroutine(眩晕协程());
    }

    private IEnumerator 眩晕协程()
    {
        while (眩晕计时器 > 0)
        {
            眩晕计时器 -= Time.deltaTime;
            yield return null;
        }
        眩晕状态 = false;
    }

    public override void 死亡()
    {
        Debug.Log("狱警死亡");
        眩晕(10f);
    }
}

