using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 自动归还 : MonoBehaviour
{
    public float 延迟时间 = 2f;

    private Coroutine 归还协程;

    private void OnEnable()
    {
        归还协程 = StartCoroutine(延迟归还());
    }

    private void OnDisable()
    {
        if (归还协程 != null)
        {
            StopCoroutine(归还协程);
        }
    }

    private IEnumerator 延迟归还()
    {
        yield return new WaitForSeconds(延迟时间);

        if(对象池.Instance != null)
        {
            对象池.Instance.归还对象(gameObject.name, gameObject);
        }
    }
}
