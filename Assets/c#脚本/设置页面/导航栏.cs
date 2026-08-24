using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class 导航栏 : MonoBehaviour
{
    public Button 基础设置;
    public Button 按键设置;
    public Button 关闭设置;

    public GameObject 基础设置面板;
    public GameObject 按键设置面板;

    public void Start()
    {

        ShowPanel(基础设置面板);

        基础设置.onClick.AddListener(() => ShowPanel(基础设置面板));
        按键设置.onClick.AddListener(() => ShowPanel(按键设置面板));
        关闭设置.onClick.AddListener(() => SceneManager.UnloadSceneAsync("Settings"));
    }

    public void ShowPanel(GameObject panelToShow)
    {
        基础设置面板.SetActive(false);
        按键设置面板.SetActive(false);

        panelToShow.SetActive(true);
    }
}
