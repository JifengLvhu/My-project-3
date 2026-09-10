using System.Collections;
using UnityEngine;
using System.Linq;
using Unity.Collections;
using UnityEngine.SceneManagement;


public class 玩家 : MonoBehaviour
{
    public static 玩家 Instance{ get; private set; }

    public enum 水域状态
    {
        空气中,//中心在水位以上
        半淹没,//中心在水位以下，头部在水位以上
        全淹没//头部在水位以下
    }


    // 组件引用
    Rigidbody2D 刚体;
    BoxCollider2D 碰撞体;
    public 玩家能力 能力脚本;
    public 玩家剧情 剧情脚本;

    // 图层设置
    public LayerMask 地面;
    public LayerMask 同伴;
    public LayerMask 怪;
    private LayerMask 可跳跃图层;

    //生命值
    public int 最大生命值 = 4;
    public int 当前生命值 = 4;
    

    // 尺寸与检测
    private float 射线长度;
    private bool 是否触地;

    // 移动控制
    private float 横向输入;
    private float 移动速度 = 7f;
    private bool 跳跃输入;
    private float 跳跃强度 = 12f;

    //水中参数
    public 水域状态 当前水域状态 = 水域状态.空气中;
    private float 水中重力缩放 = 9f / 25f;
    private float 水中触地跳跃强度 = 9f;
    private float 蹬水跳跃强度 = 4f;

    //氧气
    public float 最大氧气量 = 20f;
    public float 当前氧气量 = 20f;
    private float 氧气消耗速度 = 1f;
    private float 氧气恢复速度 = 10f;

    //无敌帧
    public bool 无敌状态 = false;
    private float 无敌帧时间 = 3f;
    public float 无敌帧计时器 = 0f;

    //其他
    public bool 正在受击退 = false;
    public bool 正在下砸 = false;
    public float 下砸速度;
    public Vector3 安全位置;

    //暂停
    public bool 游戏已被暂停 = false;
    private bool 角色已被冻结 = false;
    private float 原本的重力缩放;
    private Coroutine 安全位置记录协程引用;

    private bool 下砸协程已启用 = false;
    private bool 安全位置记录协程已启用 = false;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        刚体 = GetComponent<Rigidbody2D>();
        碰撞体 = transform.Find("碰撞箱").GetComponent<BoxCollider2D>();        
        能力脚本 = GetComponent<玩家能力>();
        剧情脚本 = GetComponent<玩家剧情>();
    }
    
    void Start()
    {
        可跳跃图层 = 地面 | 同伴 | 怪;

        射线长度计算();
        下砸速度 = 10f;

        if(存档管理器.Instance != null && 存档管理器.Instance.待应用数据 != null)
        {
            应用存档数据(存档管理器.Instance.待应用数据);
            存档管理器.Instance.清空存档数据缓存();
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void Update()
    {
        if(!游戏已被暂停)
        {
            if (Input.GetKey(按键设置.获取按键("向左移动"))) 横向输入 = -1;
            else if (Input.GetKey(按键设置.获取按键("向右移动"))) 横向输入 = 1;
            else 横向输入 = 0;

            if (Input.GetKeyDown(按键设置.获取按键("跳跃"))) 跳跃输入 = true;

            if (能力脚本 != null) 能力脚本.处理能力输入();

            if (Input.GetKeyDown(KeyCode.R))
            {
                存档管理器.Instance.触发快捷读档();
            }

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene("Pause", LoadSceneMode.Additive);
            }

            if(正在下砸)
            {
                if (射线触地检测() && !下砸协程已启用)
                {
                    下砸协程已启用 = true;
                    StartCoroutine(结束下砸协程());
                }
            }

            更新水域状态();
            氧气更新();
            无敌帧更新();
            更新安全位置();
        }
    }

    void FixedUpdate()
    {
        if (角色已被冻结)
        {
            刚体.velocity = Vector2.zero;
            刚体.gravityScale = 0;
            return;
        }

        if (!游戏已被暂停)
        {
            if (!正在受击退)
            {
                左右移动();
                跳跃逻辑();
            }
            if (正在受击退 && 射线触地检测() && 刚体.velocity.y <= 0f)
            {
                Debug.Log("击退结束");
                正在受击退 = false;
            }
            重力更新();
        }
    }

    void OnSceneLoaded(Scene 场景, LoadSceneMode 加载模式)
    {
        if(能力脚本 != null)
        {
            能力脚本.刷新同伴引用();
        }
    }

    public void 收到击退(Vector2 击退方向, float 力度)
    {
        Debug.Log($"收到击退{击退方向}{力度}");
        if(!无敌状态)
        {
            Debug.Log("击退生效");
            刚体.velocity = 击退方向 * 力度;
            正在受击退 = true;
        }
    }

    public void 下砸()
    {
        if (正在下砸) return;

        正在下砸 = true;
        刚体.velocity = new Vector2(刚体.velocity.x, -下砸速度);
    }
    
    private IEnumerator 结束下砸协程()
    {
        yield return new WaitForSeconds(0.25f);
        正在下砸 = false;
        下砸协程已启用 = false;
    }


    public void 解冻主角()
    {
        if (!角色已被冻结) return;

        角色已被冻结 = false;
        刚体.gravityScale = 原本的重力缩放;
    }

    private void 射线长度计算()
    {
        射线长度 = 碰撞体.bounds.extents.y * Mathf.Sqrt(2) - 碰撞体.bounds.extents.y / 7;
    }

    private void 左右移动()
    {
        刚体.velocity = new Vector2(横向输入 * 移动速度, 刚体.velocity.y);
    }

    private void 跳跃逻辑()
    {
        if (跳跃输入)
        {
            if(当前水域状态 == 水域状态.空气中)
            {
                if (射线触地检测())
                {
                    //Debug.Log("普通触地跳跃");
                    刚体.velocity = new Vector2(刚体.velocity.x, 跳跃强度);
                }
            }
            else
            {
                if (射线触地检测())
                {
                    //Debug.Log("水中触地跳跃");
                    刚体.velocity = new Vector2(刚体.velocity.x, 水中触地跳跃强度);
                }
                else
                {
                    //Debug.Log("水中腾空跳跃");
                    刚体.velocity = new Vector2(刚体.velocity.x, 蹬水跳跃强度);
                }
            }

            跳跃输入 = false;
        }
    }

    public bool 射线触地检测()
    {
        Vector2 射线起点 = (Vector2)transform.position;
        RaycastHit2D 射线检测 = Physics2D.Raycast(射线起点, Vector2.down, 射线长度, 可跳跃图层);
        
        Debug.DrawLine(射线起点, 射线起点 + Vector2.down * 射线长度, 射线检测.collider != null ? Color.green : Color.red, 0.1f);
        
        return 射线检测.collider != null;
    }

    private void 更新水域状态()
    {
        Vector2 角色中心 = transform.position;
        Vector2 探测尺寸 = 碰撞体.bounds.size;

        Collider2D 水池触发器 = Physics2D.OverlapBox(角色中心, 探测尺寸, 0f, LayerMask.GetMask("水池"));

        if(水池触发器 == null)
        {
            当前水域状态 = 水域状态.空气中;
            return;
        }

        水池 当前水池 = 水池触发器.GetComponentInParent<水池>();
        if(当前水池 == null)
        {
            当前水域状态 = 水域状态.空气中;
            return;
        }

        float 水面高度 = 当前水池.transform.position.y + 当前水池.当前水位;

        Bounds 角色碰撞箱 = 碰撞体.bounds;

        if(角色碰撞箱.max.y < 水面高度)
        {
            当前水域状态 = 水域状态.全淹没;
            return;
        }
        else if(角色碰撞箱.max.y > 水面高度 && 角色中心.y < 水面高度)
        {
            当前水域状态 = 水域状态.半淹没;
            return;
        }
        else
        {
            当前水域状态 = 水域状态.空气中;
            return;
        }
    }

    private void 更新安全位置()
    {
        if(Mathf.Abs(刚体.velocity.y) < 0.05f && 射线触地检测())
        {
            if(!安全位置记录协程已启用)
            {
                安全位置记录协程引用 = StartCoroutine(安全位置记录协程());
            }
        }
        else
        {
            if(安全位置记录协程引用 != null)
            {
                StopCoroutine(安全位置记录协程引用);
                安全位置记录协程已启用 = false;
                安全位置记录协程引用 = null;
            }
        }
    }

    private IEnumerator 安全位置记录协程()
    {
        安全位置记录协程已启用 = true;
        yield return new WaitForSeconds(0.5f);
        安全位置 = transform.position;
        安全位置记录协程已启用 = false;
    }

    private void 重力更新()
    {

        if(当前水域状态 != 水域状态.空气中)
        {
            刚体.gravityScale = 水中重力缩放;
        }
        else
        {
            刚体.gravityScale = 1f;
        }

    }

    private void 氧气更新()
    {
        if(当前水域状态 == 水域状态.全淹没)
        {
            if(当前氧气量 > 0)
            {
                当前氧气量 -= 氧气消耗速度 * Time.deltaTime;
                当前氧气量 = Mathf.Max(0, 当前氧气量);
            }

            if(当前氧气量 <= 0 && !无敌状态)
            {
                受到伤害(1);
            }
        }
        else
        {
            if(当前氧气量 < 最大氧气量)
            {
                当前氧气量 += 氧气恢复速度 * Time.deltaTime;
                当前氧气量 = Mathf.Min(最大氧气量, 当前氧气量);
            }
        }
    }



    //受伤与无敌帧


    public void 冻结主角()
    {
        if (角色已被冻结) return;

        角色已被冻结 = true;
        原本的重力缩放 = 刚体.gravityScale;
    }

    private void 无敌帧更新()
    {
        if(无敌状态)
        {
            无敌帧计时器 += Time.deltaTime;
            if(无敌帧计时器 >= 无敌帧时间)
            {
                无敌状态 = false;
                无敌帧计时器 = 0f;
            }
        }
    }

    public void 受到伤害(int 伤害值)
    {
        if(无敌状态) return;

        当前生命值 -= 伤害值;

        if(当前生命值 <= 0)
        {
            死亡();
        }

        无敌状态 = true;
    }

    public void 回到安全位置()
    {
        transform.position = 安全位置;
        刚体.velocity = Vector2.zero;
    }

    private void 死亡()
    {
        Debug.Log("玩家死亡");
        存档管理器.Instance.StartCoroutine(存档管理器.Instance.死亡后快捷读档协程());
    }




    //存档功能
    

    public 存档管理器.基础存档数据 获取基础存档数据()
    {
        Debug.Log("获取基础存档数据");

        存档管理器.基础存档数据 数据 = new 存档管理器.基础存档数据();
        数据.当前生命值 = 当前生命值;
        数据.当前氧气量 = 当前氧气量;
        数据.玩家位置 = transform.position;

        return 数据;
    }

    public 存档管理器.总存档数据 获取最新存档数据(string 章节名, object 章节数据)
    {
        Debug.Log("获取最新存档数据");

        当前生命值 = 最大生命值;
        当前氧气量 = 最大氧气量;

        存档管理器.总存档数据 存档数据 = new 存档管理器.总存档数据();

        存档数据.当前章节 = 章节名;
        存档数据.基础数据 = 获取基础存档数据();
        存档数据.能力数据 = new 存档管理器.玩家能力存档数据 { 已解锁能力列表 = 能力脚本.获取已解锁能力列表() };
        存档数据.剧情数据 = new 存档管理器.剧情存档数据 { 已触发剧情列表 = 剧情脚本.获取已触发剧情列表() };


        if (章节名 == "Part2" && 章节数据 is 存档管理器.Part2存档数据)
        {
            存档数据.Part2数据 = (存档管理器.Part2存档数据)章节数据;
        }

        return 存档数据;
    }

    public void 应用存档数据(存档管理器.总存档数据 存档数据)
    {
        if (!验证存档数据(存档数据))
        {
            return;
        }


        当前生命值 = 存档数据.基础数据.当前生命值;
        当前氧气量 = 存档数据.基础数据.当前氧气量;
        transform.position = 存档数据.基础数据.玩家位置;
        横向输入 = 0;
        跳跃输入 = false;

        能力脚本.应用存档数据(存档数据.能力数据.已解锁能力列表);
        剧情脚本.应用存档数据(存档数据.剧情数据.已触发剧情列表);

        I章节管理器[] 所有章节管理器 = FindObjectsOfType<MonoBehaviour>().OfType<I章节管理器>().ToArray();

        if(所有章节管理器.Length != 0)
        {
            I章节管理器 当前章节管理器 = 所有章节管理器[0];

            if(存档数据.Part2数据 != null)
            {
                当前章节管理器.应用章节存档数据(存档数据.Part2数据);
                Debug.Log("已调用管理器恢复 Part2 数据");
            }
        }

        //Debug.Log("读取存档成功");

    }

    private bool 验证存档数据(存档管理器.总存档数据 存档数据)
    {
        if(存档数据 == null)
        {
            Debug.Log("存档数据为空");
            return false;
        }

        if(存档数据.基础数据 == null)
        {
            Debug.Log("基础数据为空");
            return false;
        }

        if(存档数据.基础数据.当前生命值 <=0 || 存档数据.基础数据.当前生命值 > 最大生命值)
        {
            Debug.Log("当前生命值不合法");
            return false;
        }

        if(存档数据.基础数据.当前氧气量 <=0 || 存档数据.基础数据.当前氧气量 > 最大氧气量)
        {
            Debug.Log("当前氧气量不合法");
            return false;
        }

        if(float.IsNaN(存档数据.基础数据.玩家位置.x) || float.IsNaN(存档数据.基础数据.玩家位置.y))
        {
            Debug.Log("玩家位置不合法");
            return false;
        }

        return true;
    }
}
