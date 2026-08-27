using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeManager : MonoBehaviour
{
    public static VolumeManager Instance { get; private set; }
    public enum 音量类型 { 主音量, BGM, SFX }

    [Range(0f, 1f)]
    public float 主音量 = 1f;

    [Range(0f, 1f)]
    public float BGM = 1f;

    [Range(0f, 1f)]
    public float SFX = 1f;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            加载设置();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void 设置音量(音量类型 类型, float 音量)
    {
        音量 = Mathf.Clamp01(音量);

        switch (类型)
        {
            case 音量类型.主音量:
                主音量 = 音量;
                break;

            case 音量类型.BGM:
                BGM = 音量;
                break;

            default:
                SFX = 音量;
                break;
        }
    }

    public float 获取最终音量(音量类型 类型)
    {
        switch (类型)
        {
            case 音量类型.主音量:
                return 主音量;

            case 音量类型.BGM:
                return 主音量 * BGM;

            default:
                return 主音量 * SFX;  
        }
    }

    private void 加载设置()
    {
        主音量 = PlayerPrefs.GetFloat("主音量", 1f);
        BGM = PlayerPrefs.GetFloat("BGM音量", 1f);
        SFX = PlayerPrefs.GetFloat("SFX音量", 1f);
    }
}
