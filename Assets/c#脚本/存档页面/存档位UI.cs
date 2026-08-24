using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class 存档位UI : MonoBehaviour
{

    public int 档位编号 = -1;

    public Image 缩略图;
    public TMP_Text 章节名文本;
    public TMP_Text 保存时间文本;

    public Button 按钮组件;

    void Awake()
    {
        按钮组件 = GetComponent<Button>();
    }
}
