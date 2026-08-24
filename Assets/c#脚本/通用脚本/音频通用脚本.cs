using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 音频通用脚本 : MonoBehaviour
{

    public VolumeManager.音量类型 类型;
    public AudioSource 音源;

    private void Start()
    {
        音源 = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (音源 != null && VolumeManager.Instance != null)
        {
            音源.volume = VolumeManager.Instance.获取最终音量(类型);
        }
    }
}
