using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 基础小怪 : MonoBehaviour
{
    protected Collider2D 自身碰撞体;
    protected Collider2D 玩家碰撞体;
    protected Rigidbody2D 玩家刚体;
    protected Rigidbody2D 自身刚体;

    protected float 自身边长;
    protected float 玩家边长;
    protected float 距离判定值;


    public int 最大生命值 = 1;
    public int 当前生命值 = 1;

    public int 接触伤害值 = 1;
    public float 玩家反弹力度 = 10f;

    //移动
    public float 移动速度 = 5f;
    protected float 横向输入 = 1f;
    public LayerMask 地面;
    public LayerMask 怪物图层;
    public LayerMask NPCs;


    protected void Awake()
    {
        自身碰撞体 = GetComponentInChildren<Collider2D>();
        玩家碰撞体 = 玩家.Instance.GetComponentInChildren<Collider2D>();
        玩家刚体 = 玩家.Instance.GetComponent<Rigidbody2D>();
        自身刚体 = GetComponent<Rigidbody2D>();

    }

    protected virtual void Start()
    {
        当前生命值 = 最大生命值;
        自身边长 = 自身碰撞体.bounds.size.y;
        玩家边长 = 玩家碰撞体.bounds.size.y;
        距离判定值 = (自身边长 / 2f) + (玩家边长 / 2f);

        移动速度 = 工具库.生成物体随机数(gameObject, 4f, 5.5f);
    }

    private void FixedUpdate()
    {
        换向判断();
        左右移动();
    }

    protected void 左右移动()
    {
        自身刚体.velocity = new Vector2(横向输入 * 移动速度, 自身刚体.velocity.y);
    }

    protected void 换向判断()
    {
        if (前方障碍物检测())
        {
            横向输入 *= -1f;
            return;
        }

        if (!前方地面检测())
        {
            横向输入 *= -1f;
        }
    }

    protected bool 前方障碍物检测()
    {
        Vector2 射线起点 = (Vector2)transform.position + Vector2.right * 横向输入 * (自身边长 / 2f + 0.1f);
        Vector2 射线方向 = Vector2.right * 横向输入;

        RaycastHit2D 射线碰撞 = Physics2D.Raycast(射线起点, 射线方向, 0.2f, 地面 | 怪物图层 | NPCs);

        if (射线碰撞.collider != null)
        {
            if (射线碰撞.collider.gameObject == gameObject || 射线碰撞.collider.transform.parent == transform)
            {
                return false;
            }
            return true;
        }
        return false;
    }

    protected bool 前方地面检测()
    {
        Vector2 射线起点 = (Vector2)transform.position + Vector2.right * 横向输入 * (自身边长 / 2f + 0.1f);
        Vector2 射线方向 = Vector2.down;

        RaycastHit2D 射线碰撞 = Physics2D.Raycast(射线起点, 射线方向, 自身边长 / 2f + 0.2f, 地面);

        return 射线碰撞.collider != null;
    }

    public virtual void 收到伤害(int 伤害值)
    {
        当前生命值 -= 伤害值;

        if (当前生命值 <= 0)
        {
            死亡();
        }
    }

    public virtual void 死亡()
    {
        Debug.Log("基础小怪死亡");
        //死亡功能
    }

    protected virtual void OnTriggerEnter2D(Collider2D 其他触发器)
    {
        if (其他触发器.CompareTag("玩家"))
        {
            float y相对距离 = 玩家.Instance.transform.position.y - transform.position.y;

            if (y相对距离 > 距离判定值 - 0.2f)
            {
                收到伤害(1);
                玩家刚体.velocity = new Vector2(玩家刚体.velocity.x, 玩家反弹力度);
            }
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D 其他触发器)
    {
        if (其他触发器.CompareTag("玩家"))
        {
            float y相对距离 = 玩家.Instance.transform.position.y - transform.position.y;

            if (!(y相对距离 > 距离判定值 - 0.2f))
            {
                float x相对距离 = 玩家.Instance.transform.position.x - transform.position.x;
                Vector2 击退方向 = Vector2.zero;

                //Debug.Log($"x相对距离:{x相对距离} y相对距离:{y相对距离} 距离判定值:{距离判定值}");
                if (y相对距离 < -距离判定值)
                {
                    击退方向 = Vector2.down;
                }
                else if (x相对距离 > 0)
                {
                    击退方向 = new Vector2(1f, 1f).normalized;
                }
                else
                {
                    击退方向 = new Vector2(-1f, 1f).normalized;
                }

                玩家.Instance.收到击退(击退方向, 玩家反弹力度);
                玩家.Instance.受到伤害(接触伤害值);
            }
        }
    }
}