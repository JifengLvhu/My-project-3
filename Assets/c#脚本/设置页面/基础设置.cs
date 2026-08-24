using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class 基础设置 : MonoBehaviour
{

    public Slider 主音量滑条;
    public Slider BGM滑条;
    public Slider SFX滑条;

    public TMP_Dropdown 分辨率下拉框;
    public TMP_Dropdown 画面模式下拉框;
    public TMP_Dropdown 帧率下拉框;

    private Resolution[] 可用分辨率;

    private void Start()
    {
        初始化分辨率列表();

        主音量滑条.onValueChanged.AddListener(应用主音量);
        BGM滑条.onValueChanged.AddListener(应用BGM音量);
        SFX滑条.onValueChanged.AddListener(应用SFX音量);

        分辨率下拉框.onValueChanged.AddListener(应用视频设置);
        画面模式下拉框.onValueChanged.AddListener(应用视频设置); //0：全屏  1：窗口
        帧率下拉框.onValueChanged.AddListener(应用帧率);//0：无限制  1：60  2：30
        
        加载设置();
 
    }


    private void 应用主音量(float value)
    {
        VolumeManager.Instance.设置音量(VolumeManager.音量类型.主音量, value);
        AudioListener.volume = value;

        保存设置();
    }

    private void 应用BGM音量(float value)
    {
        VolumeManager.Instance.设置音量(VolumeManager.音量类型.BGM, value);

        保存设置();
    }

    private void 应用SFX音量(float value)
    {
        VolumeManager.Instance.设置音量(VolumeManager.音量类型.SFX, value);

        保存设置();
    }

    private void 应用帧率(int 选项索引) //0：无限制  1：60  2：30
    {
        switch (选项索引)
        {
            case 0:
                Application.targetFrameRate = -1;
                break;

            case 1:
                Application.targetFrameRate = 60;
                break;

            case 2:
                Application.targetFrameRate = 30;
                break;
        }

        保存设置();
    }

    private void 初始化分辨率列表()
    {
#if UNITY_EDITOR //模拟真实环境
        可用分辨率 = new Resolution[]
        {
            new Resolution {width = 1920, height = 1080, refreshRateRatio = new RefreshRate{numerator = 60, denominator = 1}},
            new Resolution {width = 1920, height = 1080, refreshRateRatio = new RefreshRate{numerator = 144, denominator = 1}},
            new Resolution {width = 1920, height = 1080, refreshRateRatio = new RefreshRate{numerator = 165, denominator = 1}},
            new Resolution {width = 1280, height = 720, refreshRateRatio = new RefreshRate{numerator = 60, denominator = 1}},
            new Resolution {width = 1280, height = 720, refreshRateRatio = new RefreshRate{numerator = 144, denominator = 1}},
            new Resolution {width = 1280, height = 720, refreshRateRatio = new RefreshRate{numerator = 165, denominator = 1}},
         };
#else
        可用分辨率 = Screen.resolutions;
#endif
        分辨率下拉框.ClearOptions();

        var 分辨率选项 = new List<string>();
        var 已有分辨率 = new HashSet<string>();
        int 当前分辨率索引 = 0;
        int 有效索引 = 0;

        for (int i = 0; i < 可用分辨率.Length; i++)
        {

            if (Math.Abs(可用分辨率[i].width * 9f - 可用分辨率[i].height * 16f) > 0.05f) continue;

            string 选项 = 可用分辨率[i].width + " x " + 可用分辨率[i].height;

            if (已有分辨率.Contains(选项)) continue;

            分辨率选项.Add(选项);
            已有分辨率.Add(选项);

            if (可用分辨率[i].width == Screen.currentResolution.width &&
                可用分辨率[i].height == Screen.currentResolution.height)
            {
                当前分辨率索引 = 有效索引;
            }

            有效索引++;
        }

        分辨率下拉框.AddOptions(分辨率选项);
        分辨率下拉框.value = 当前分辨率索引;
        分辨率下拉框.RefreshShownValue();
    }

    public void 保存设置()
    {
        PlayerPrefs.SetFloat("主音量", 主音量滑条.value);
        PlayerPrefs.SetFloat("BGM音量", BGM滑条.value);
        PlayerPrefs.SetFloat("SFX音量", SFX滑条.value);
        PlayerPrefs.SetInt("分辨率", 分辨率下拉框.value);
        PlayerPrefs.SetInt("画面模式", 画面模式下拉框.value);
        PlayerPrefs.SetInt("帧率", 帧率下拉框.value);

        PlayerPrefs.Save();
    }

    private void 加载设置()
    {
        主音量滑条.value = VolumeManager.Instance.主音量;
        BGM滑条.value = VolumeManager.Instance.BGM;
        SFX滑条.value = VolumeManager.Instance.SFX;
        分辨率下拉框.value = PlayerPrefs.GetInt("分辨率", 分辨率下拉框.value);
        画面模式下拉框.value = PlayerPrefs.GetInt("画面模式", 画面模式下拉框.value);
        帧率下拉框.value = PlayerPrefs.GetInt("帧率", 帧率下拉框.value);
    }

    private void 应用视频设置(int 选项索引)
    {
        string 选项文本 = 分辨率下拉框.options[分辨率下拉框.value].text;
        string[] 尺寸 = 选项文本.Split(new[] {" x "},System.StringSplitOptions.None);
        int 宽 = int.Parse(尺寸[0]);
        int 高 = int.Parse(尺寸[1]);

        FullScreenMode 目标模式 = FullScreenMode.Windowed;
        switch (画面模式下拉框.value)
        {
            case 0: 
                目标模式 = FullScreenMode.FullScreenWindow;
                break;
            case 1: 
                目标模式 = FullScreenMode.Windowed;
                break;
        }

        Screen.SetResolution(宽, 高, 目标模式);

        保存设置();
    }

}
