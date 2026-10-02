using System.Collections;
using UnityEngine;

public class test : MonoBehaviour
{
    [Header("弹幕配置")]
    [SerializeField] private ScriptableObject pattern;

    [Header("测试发射")]
    [SerializeField] private float 发射间隔 = 1f;

    private IPattern 当前Pattern => pattern as IPattern;

    private void Start()
    {
        StartCoroutine(测试发射协程());
    }

    private IEnumerator 测试发射协程()
    {
        while (true)
        {
            yield return new WaitForSeconds(发射间隔);

            if (行为调度器.Instance == null || 当前Pattern == null)
            {
                continue;
            }

            行为调度器.Instance.发射(
                transform.position,
                Vector2.left,
                当前Pattern);
        }
    }
}