using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 水池 : MonoBehaviour
{
    [Header("身份与尺寸")]
    public int 水池ID;
    public float 水池宽度 = 5f;

    [Header("水池数据")]
    public float 最大水位;
    public float 最小水位;

    public float 当前水位;

    private Transform 水池图片;
    private SpriteRenderer 水池图片Sprite;
    private Transform 水池判定框;
    private BoxCollider2D 水池判定框Collider;

    private float 原始高度;

    void Awake()
    {
        水池图片 = transform.Find("水池图片");
        水池判定框 = transform.Find("水池判定框");

        if (水池图片 != null)
        {
            水池图片Sprite = 水池图片.GetComponent<SpriteRenderer>();

            float 原始宽度 = 水池图片Sprite.bounds.size.x;
            原始高度 = 水池图片Sprite.bounds.size.y;
            float scaleX = 水池宽度 / 原始宽度;

            水池图片.localScale = new Vector3(scaleX, 水池图片.localScale.y, 1f);
        }
        if(水池判定框 != null)
        { 
            水池判定框Collider = 水池判定框.GetComponent<BoxCollider2D>(); 
            水池判定框Collider.size = new Vector2(水池宽度, 水池判定框Collider.size.y);
        }


    }

    void Start()
    {
        设置水位(当前水位);
    }

    public void 设置水位(float H)
    {
        当前水位 = Mathf.Clamp(H, 最小水位, 最大水位);

        if(水池图片 != null)
        {
            float scaleY = 当前水位 / 原始高度;

            水池图片.localScale = new Vector3(水池图片.localScale.x, scaleY, 1f);
            水池图片.localPosition = new Vector3(0f, 当前水位 / 2f, 0f);
        }

        if (水池判定框Collider != null)
        {
            水池判定框Collider.size = new Vector2(水池宽度, 当前水位);
            水池判定框Collider.offset = new Vector2(0f, 当前水位 / 2f);

            水池判定框Collider.enabled = 当前水位 > 0.001f;
        }
    }

}
