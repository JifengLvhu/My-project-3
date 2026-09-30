using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 史莱姆 : MonoBehaviour, I池对象重置
{
    //组件
    private Collider2D 自身碰撞体;
    private Collider2D 玩家碰撞体;
    private Rigidbody2D 玩家刚体;
    public Rigidbody2D 自身刚体;

    //距离判定
    private float 自身边长;
    private float 玩家边长;
    private float 距离判定值;

    public float 玩家反弹力度 = 10f;

    //移动
    private float 横向输入 = 1f;
    public float 弹跳高度 = 17f;
    public float 空中横向速度 = 7f;
    public float 弹跳CD;
    private float 弹跳CD计时器 = 0f;


    public LayerMask 地面;
    public LayerMask 怪物图层;
    public LayerMask NPCs;
    private LayerMask 可跳跃图层;

    //分裂
    private bool 正在分裂 = false;
    public GameObject 史莱姆预制体;


    private void Awake()
    {
        自身碰撞体 = GetComponentInChildren<Collider2D>();
        玩家碰撞体 = 玩家.Instance.GetComponentInChildren<Collider2D>();
        玩家刚体 = 玩家.Instance.GetComponent<Rigidbody2D>();
        自身刚体 = GetComponent<Rigidbody2D>();

    }

    private void Start()
    {
        自身边长 = 自身碰撞体.bounds.size.y;
        玩家边长 = 玩家碰撞体.bounds.size.y;
        距离判定值 = (自身边长 / 2f) + (玩家边长 / 2f);

        弹跳高度 = 15f;
        玩家反弹力度 = 15f;
        弹跳CD = 1.5f;

        可跳跃图层 = 地面 | 怪物图层 | NPCs;
    }

    private void FixedUpdate()
    {
        if(!正在分裂)
        {
            换向判断();
            弹跳移动();
        }
    }

    private void 换向判断()
    {
        if (前方障碍物检测())
        {
            横向输入 *= -1f;
            return;
        }
    }

    private void 弹跳移动()
    {

        if (射线触地检测())
        {
            if (弹跳CD计时器 > 0f)
            {
                弹跳CD计时器 -= Time.fixedDeltaTime;
            }
            else
            {
                弹跳CD计时器 = 弹跳CD;
                自身刚体.velocity = new Vector2(横向输入 * 空中横向速度, 弹跳高度);
            }
        }
        else
        {
            自身刚体.velocity = new Vector2(横向输入 * 空中横向速度, 自身刚体.velocity.y);
        }
    }

    private bool 射线触地检测()
    {
        Vector2 射线起点 = (Vector2)transform.position - Vector2.up * (自身边长 / 2f + 0.1f);
        RaycastHit2D 射线检测 = Physics2D.Raycast(射线起点, Vector2.down, 0.1f, 可跳跃图层);
        return 射线检测.collider != null;
    }

    private bool 前方障碍物检测()
    {
        Vector2 射线起点 = (Vector2)transform.position + Vector2.right * 横向输入 * (自身边长 / 2f + 0.1f);
        Vector2 射线方向 = Vector2.right * 横向输入;

        RaycastHit2D 射线碰撞 = Physics2D.Raycast(射线起点, 射线方向, 0.2f, 可跳跃图层);

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

    private void OnTriggerEnter2D(Collider2D 其他触发器)
    {
        if (其他触发器.CompareTag("玩家"))
        {
            float y相对距离 = 玩家.Instance.transform.position.y - transform.position.y;
            if (y相对距离 > 距离判定值 - 0.2f)
            {
                玩家刚体.velocity = new Vector2(玩家刚体.velocity.x, Mathf.Abs(玩家刚体.velocity.y) + 2f);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D 其他触发器)
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
            }
        }
    }

    public void 分裂()
    {
        if (正在分裂) return;

        正在分裂 = true;
        StartCoroutine(分裂协程());
    }

    private IEnumerator 分裂协程()
    {

        yield return new WaitUntil(() => 射线触地检测());
        yield return new WaitUntil(() => 对象池.Instance.已就绪);

        自身刚体.velocity = Vector2.zero;
        Vector2 生成位置 = (Vector2)transform.position + Vector2.up * 自身边长;
        GameObject newObj = 对象池.Instance.获取对象("史莱姆", 史莱姆预制体, 生成位置, Quaternion.identity);

        if (newObj.TryGetComponent<史莱姆>(out var 新史莱姆))
        {
            新史莱姆.自身刚体.velocity = new Vector2(横向输入, 1f);
        }

        yield return new WaitForSeconds(0.3f);
        正在分裂 = false;
    }

    public void 池对象重置()
    {
        自身刚体.velocity = Vector2.zero;
        弹跳CD计时器 = 0f;
        正在分裂 = false;
    }
}
