using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class 岩浆池 : MonoBehaviour
{
    [Header("身份与尺寸")]
    public int 岩浆池ID;
    public float 岩浆池宽度 = 26f;

    [Header("岩浆池数据")]
    public float 最高岩浆位;
    public float 最低岩浆位;

    public float 当前岩浆位;
    public float 浮动速度;
    private bool 岩浆上涨 = true;
    private bool 停止浮动 = false;

    private Transform 岩浆池图片;
    private SpriteRenderer 岩浆池图片Sprite;
    private Transform 岩浆池判定框;
    private BoxCollider2D 岩浆池判定框Collider;

    private float 原始高度;
    
    void Awake()
    {
        岩浆池图片 = transform.Find("岩浆池图片");
        岩浆池判定框 = transform.Find("岩浆池判定框");

        if (岩浆池图片 != null)
        {
            岩浆池图片Sprite = 岩浆池图片.GetComponent<SpriteRenderer>();

            原始高度 = 岩浆池图片Sprite.bounds.size.y;
            float 原始宽度 = 岩浆池图片Sprite.bounds.size.x;
            float ScaleX = 岩浆池宽度 / 原始宽度;

            岩浆池图片.localScale = new Vector3(ScaleX, 岩浆池图片.localScale.y, 1f);
        }

        if (岩浆池判定框 != null)
        {
            岩浆池判定框Collider = 岩浆池判定框.GetComponent<BoxCollider2D>();
        }
    }

    void Start()
    {
        设置岩浆位(当前岩浆位);
    }
    
    void Update()
    {
        岩浆浮动();
    }

    void OnTriggerEnter2D(Collider2D 其他触发器)
    {
        if(其他触发器.CompareTag("玩家"))
        {
            玩家.Instance.受到伤害(1);
            玩家.Instance.回到安全位置();
        }
    }

    private void 岩浆浮动()
    {
        if(停止浮动) return;
        
        if(岩浆上涨 && 当前岩浆位 < 最高岩浆位)
        {
            当前岩浆位 += 浮动速度 * Time.deltaTime;
            设置岩浆位(当前岩浆位);
        }
        else if(!岩浆上涨 && 当前岩浆位 > 最低岩浆位)
        {
            当前岩浆位 -= 浮动速度 * Time.deltaTime;
            设置岩浆位(当前岩浆位);
        }
        
        if(当前岩浆位 >= 最高岩浆位)
        {
            岩浆上涨 = false;
            StartCoroutine(等待协程(2.5f));
        }
        else if(当前岩浆位 <= 最低岩浆位)
        {
            岩浆上涨 = true;
            StartCoroutine(等待协程(2.5f));
        }
    }
    
    public void 设置岩浆位(float H)
    {
        当前岩浆位 = Mathf.Clamp(H, 最低岩浆位, 最高岩浆位);

        if (岩浆池图片 != null)
        {
            float ScaleY = 当前岩浆位 / 原始高度;
            岩浆池图片.localScale = new Vector3(岩浆池图片.localScale.x, ScaleY, 1f);
            岩浆池图片.localPosition = new Vector3(0f,当前岩浆位 / 2f, 0f);
        }
        if(岩浆池判定框Collider != null)
        {
            岩浆池判定框Collider.size = new Vector2(岩浆池宽度, 当前岩浆位);
            岩浆池判定框Collider.offset = new Vector2(0f, 当前岩浆位 / 2f);
            
            岩浆池判定框Collider.enabled = 当前岩浆位 > 0.001f;
        }
    }

    public void 喷涌(float 上涨程度)
    {
        最高岩浆位 += 上涨程度;
        最低岩浆位 += 上涨程度;

        岩浆上涨 = true;
        StartCoroutine(喷涌协程(上涨程度));
    }

    private IEnumerator 等待协程(float 等待时间)
    {
        停止浮动 = true;
        yield return new WaitForSeconds(等待时间);
        停止浮动 = false;
    }
    
    private IEnumerator 喷涌协程(float 上涨程度)
    {
        停止浮动 = true;
        float H = 当前岩浆位;
        while(H < 当前岩浆位+上涨程度)
        {
            H += 浮动速度 * Time.deltaTime;
            设置岩浆位(H);
            yield return null;
        }
        停止浮动 = false;
    }
}
