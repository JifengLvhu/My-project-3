using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 自动归还 : MonoBehaviour
{
    public string 预制体名称;
    public float 延迟时间 = 2f;

    private Coroutine 归还协程;

    private void OnEnable()
    {
        归还协程 = StartCoroutine(延迟归还());
        //Debug.Log($"Auto启用，开始倒计时，最大时长：{延迟时间}", gameObject);
    }

    private void OnDisable()
    {
        if (归还协程 != null)
        {
            StopCoroutine(归还协程);
            //Debug.Log("中止归还协程", gameObject);
        }
    }

    private IEnumerator 延迟归还()
    {
        yield return new WaitForSeconds(延迟时间);

        if(对象池.Instance != null)
        {
            //Debug.Log("Auto超时，归还子弹", gameObject);
            对象池.Instance.归还对象(预制体名称, gameObject);
        }
    }
}
