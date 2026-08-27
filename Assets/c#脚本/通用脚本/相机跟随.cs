using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 相机跟随 : MonoBehaviour
{
    public static 相机跟随 Instance { get; private set; }

    private Transform 玩家位置;
    public float 相机跟随速度;
    public Vector3 偏移;
    public float 像素单位 = 100f;

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
    }

    void Start()
    {
        玩家位置 = 玩家.Instance.transform;

        偏移 = new Vector3(0, 1, -10);
        相机跟随速度 = 5f;
    }

    void FixedUpdate() 
    {
        跟随玩家();
    }

    void Lateupdate()
    {
        相机防抖();
    }

    void 跟随玩家()
    {
        if (玩家位置 != null)
        {
            Vector3 目标位置 = 玩家位置.position + 偏移;
            transform.position = Vector3.Lerp(transform.position, 目标位置, Time.deltaTime * 相机跟随速度);
        }
        else
        {
            Debug.Log("没有找到玩家位置");
        }    
    }

    void 相机防抖()
    {
        if (Vector3.Distance(transform.position, 玩家位置.position + 偏移) > 0.01f)
        {
            Vector3 新位置 = transform.position;
            新位置.x = Mathf.Round(新位置.x * 像素单位) / 像素单位;
            新位置.y = Mathf.Round(新位置.y * 像素单位) / 像素单位;
            新位置.z = -10;
            transform.position = 新位置;
        }
    }
}
