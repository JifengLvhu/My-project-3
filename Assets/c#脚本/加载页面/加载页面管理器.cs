using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class 加载页面管理器 : MonoBehaviour
{
    public CanvasGroup 过渡黑屏;
    private float 过渡速度 = 2f;

    public void 执行加载流程(string 目标场景名)
    {
        StartCoroutine(加载协程(目标场景名));
    }

    private IEnumerator 加载协程(string 目标场景名)
    {
        //Debug.Log("开始加载协程");
        GameManager.Instance.恢复暂停();

        过渡黑屏.blocksRaycasts = true;

        过渡黑屏.alpha = 0f;
        while(过渡黑屏.alpha < 1f)
        {
            过渡黑屏.alpha += Time.deltaTime * 过渡速度;
            过渡黑屏.alpha = Mathf.Min(1f, 过渡黑屏.alpha);

            yield return null;
        }
        过渡黑屏.alpha = 1f;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(目标场景名, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Scene 新场景 = SceneManager.GetSceneByName(目标场景名);
        if (新场景.isLoaded)
        {
            SceneManager.SetActiveScene(新场景);
        }


        string 场景标识 = (目标场景名 == "Main") ? null : 目标场景名;
        yield return GameManager.Instance.加载完毕(场景标识);

        存档管理器.Instance.应用待应用存档数据();

        while (过渡黑屏.alpha > 0f)
        {
            过渡黑屏.alpha -= Time.deltaTime * 过渡速度;
            过渡黑屏.alpha = Mathf.Clamp01(过渡黑屏.alpha);
            yield return null;
        }
        过渡黑屏.alpha = 0f;
        过渡黑屏.blocksRaycasts = false;

        GameManager.Instance.卸载加载场景();
    }
}
