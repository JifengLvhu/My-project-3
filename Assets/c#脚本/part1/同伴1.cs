using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class 同伴1 : MonoBehaviour
{

    // 引用与组件
    Rigidbody2D 物理;
    BoxCollider2D 碰撞体;
    public LayerMask 地面;
    public LayerMask 同伴;
    public LayerMask 怪;

    private 玩家 玩家实例;
    private 工具库 工具库;

    // 状态与开关
    public enum 移动状态类型 { 跟随玩家, 前往目的位置 , 静止, 追逐狱警}

    private 移动状态类型 当前移动状态 = 移动状态类型.跟随玩家;
    private bool 是否触地;
    private bool 是否跟随;

    //编号
    public int 同伴编号;

    //跟随玩家
    private float 开始跟随玩家阈值 = 5f;
    private float 停止跟随玩家阈值 = 3f;
    private float 目标跟随阈值 = 0.2f;
    private float 目标停止阈值 = 0.1f;
    private float 传送距离阈值 = 15f;

    //移动
    private float 移动速度 = 7f;
    private float 默认速度 = 7f;
    private float 跳跃强度 = 12;
    private float 默认跳跃强度;
    private float 横向输入;

    // 尺寸与检测
    private Vector3 目标位置;
    private float 射线长度;
    private float 角色尺寸;

    //自爆
    private 狱警 目标狱警;
    private float 追逐速度;
    private float 追逐跳跃强度;






    void Awake()
    {
        物理 = GetComponent<Rigidbody2D>();
        碰撞体 = transform.Find("碰撞箱").GetComponent<BoxCollider2D>();  

        射线长度计算();
    }

    void Start()
    {
        玩家实例 = 玩家.Instance;

        角色尺寸 = 碰撞体.bounds.size.x;
        开始跟随玩家阈值 = 同伴编号*2f + 2f;
        停止跟随玩家阈值 = 同伴编号*2f ;
        当前移动状态 = 移动状态类型.跟随玩家;

        默认速度 = 7f;
        移动速度 = 默认速度;
        追逐速度 = 8f;

        默认跳跃强度 = 12f;
        跳跃强度 = 默认跳跃强度;
        追逐跳跃强度 = 15f;

    }

    void Update()
    {
        传送判断();
    }

    void FixedUpdate()
    {
        跟随判断();
        左右移动();
        跳跃判断();
    }







    void 射线长度计算()
    {
        射线长度 = 碰撞体.bounds.extents.y * Mathf.Sqrt(2) - 碰撞体.bounds.extents.y / 7;
    }

    void 左右移动()
    {
        if (横向输入 == 0 && 射线触地检测())
        {
            物理.velocity = new Vector2(0f, 物理.velocity.y);
        }
        else
        {
            物理.velocity = new Vector2(横向输入 * 移动速度, 物理.velocity.y);
        }
    }

    void 跟随判断()
    {
        float 距离;
        float 当前跟随阈值;
        float 当前停止阈值;
        Vector3 当前目标位置;

        switch (当前移动状态)
        {
            case 移动状态类型.跟随玩家:
                当前目标位置 = 玩家实例.transform.position;
                当前跟随阈值 = 开始跟随玩家阈值;
                当前停止阈值 = 停止跟随玩家阈值;

                移动速度 = 默认速度;
                跳跃强度 = 默认跳跃强度;
                break;

            case 移动状态类型.前往目的位置:
                当前目标位置 = 目标位置;
                当前跟随阈值 = 目标跟随阈值;
                当前停止阈值 = 目标停止阈值;

                移动速度 = 默认速度;
                跳跃强度 = 默认跳跃强度;
                break;

            case 移动状态类型.追逐狱警:
                当前目标位置 = 目标狱警.transform.position;
                当前跟随阈值 = 目标跟随阈值;
                当前停止阈值 = 目标停止阈值;

                移动速度 = 追逐速度;
                跳跃强度 = 追逐跳跃强度;
                break;

            default:
                当前目标位置 = 玩家实例.transform.position;
                当前跟随阈值 = 开始跟随玩家阈值;
                当前停止阈值 = 停止跟随玩家阈值;

                移动速度 = 默认速度;
                跳跃强度 = 默认跳跃强度;
                break;
        }

        距离 = Mathf.Abs(transform.position.x - 当前目标位置.x);

        if (距离 > 当前跟随阈值)
        {
            是否跟随 = true;
        }
        else if (距离 < 当前停止阈值)
        {
            是否跟随 = false;
        }

        if (是否跟随)
        {
            if (transform.position.x < 当前目标位置.x)
            {
                横向输入 = 1;
            }
            else if (transform.position.x > 当前目标位置.x)
            {
                横向输入 = -1;
            }
        }
        else
        {
            横向输入 = 0;
        }
    }

    bool 左右触地判断()
    {
        Vector2 射线起点 = (Vector2)transform.position;
        Vector2 射线方向;
        if(横向输入 > 0)
        {
            射线方向 = 工具库.角度转向量(-30f);
        }
        else
        {
            射线方向 = 工具库.角度转向量(210f);
        }
        float 射线长度 = 角色尺寸*2;

        RaycastHit2D 射线碰撞 = Physics2D.Raycast(射线起点, 射线方向, 射线长度, 地面);
        //Debug.DrawRay(射线起点, 射线方向 * 射线长度, Color.yellow, 2f); //查看射线

        return 射线碰撞.collider != null;
    }

    bool 前方障碍物检测()
    {

        Vector2 射线起点 = (Vector2)transform.position + Vector2.right * 横向输入 * (角色尺寸 / 2 + 0.2f);
        Vector2 射线方向 = Vector2.right * 横向输入;

        RaycastHit2D 射线碰撞 =  Physics2D.Raycast(射线起点, 射线方向, 角色尺寸, 地面 | 同伴 | 怪);
       
        //Debug.DrawRay(射线起点, 射线方向 * 角色尺寸, Color.green, 2f); //查看射线

        if(射线碰撞.collider != null)
        {

            //GameObject 碰撞对象 = 射线碰撞.collider.gameObject;
            //Debug.Log($"检测到障碍物: {碰撞对象.transform.parent}的{碰撞对象.name}"); //排查为何同伴一直跳跃
            if (射线碰撞.collider.gameObject == gameObject)
            {
                return false; 
            }


            Rigidbody2D 对方刚体 = 射线碰撞.collider.GetComponentInParent<Rigidbody2D>();

            if(对方刚体 != null)
            {
                if(Mathf.Abs(对方刚体.velocity.x) < 0.01f)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    void 跳跃判断()
    {
        if((横向输入 != 0) && 
            (!左右触地判断() || 前方障碍物检测()) && 
            射线触地检测() )
        {
            跳跃();
        }
    }

    void 跳跃()
    {
        物理.velocity = new Vector2(物理.velocity.x, 跳跃强度);
    }

    bool 射线触地检测()
    {
        Vector2 射线起点 = (Vector2)transform.position;
        RaycastHit2D 射线检测 = Physics2D.Raycast(射线起点, Vector2.down, 射线长度, 地面);
        return 射线检测.collider != null;
    }

    void 传送判断()
    {
        if(玩家实例 == null || 当前移动状态 != 移动状态类型.跟随玩家) return;

        float 距离 = Vector2.Distance(transform.position, 玩家实例.transform.position);

        if(距离 > 传送距离阈值)
        {
            物理.velocity = Vector2.zero;

            Vector2 传送位置 = 玩家实例.transform.position;
            传送位置.y += 2f;
            transform.position = 传送位置;
        }
    }

    public void 前往指定位置(Vector3 位置)
    {
        当前移动状态 = 移动状态类型.前往目的位置;
        目标位置 = 位置;
    }

    public void 恢复跟随()
    {
        当前移动状态 = 移动状态类型.跟随玩家;
    }

    public void 执行自爆()
    {
        狱警[] 所有狱警 = FindObjectsByType<狱警>(FindObjectsSortMode.None);
        狱警 最近狱警 = null;
        float 最近距离 = float.MaxValue;

        foreach(var 狱警 in 所有狱警)
        {
            if(狱警.已发现玩家)
            {
                float 距离 = Vector2.Distance(玩家.Instance.transform.position, 狱警.transform.position);
                if (距离 < 最近距离)
                {
                    最近距离 = 距离;
                    最近狱警 = 狱警;
                }
            }
        }
        if(最近狱警 != null)
        {
            Debug.Log("执行自爆");
            当前移动状态 = 移动状态类型.追逐狱警;
            StartCoroutine(自爆协程(最近狱警));
        }
    }

    private IEnumerator 自爆协程(狱警 目标狱警)
    {
        追逐狱警(目标狱警);
        yield return new WaitUntil(() => Vector2.Distance(transform.position, 目标狱警.transform.position) < 1.1f);
        当前移动状态 = 移动状态类型.跟随玩家;
        目标狱警.死亡();
        死亡();
    }

    public void 追逐狱警(狱警 目标)
    {
        当前移动状态 = 移动状态类型.追逐狱警;
        目标狱警 = 目标;
    }

    public void 死亡()
    {
        玩家能力 能力脚本 = 玩家.Instance.GetComponent<玩家能力>();
        Debug.Log("同伴死亡");
        能力脚本.StartCoroutine(能力脚本.复活协程(gameObject));
    }
}


