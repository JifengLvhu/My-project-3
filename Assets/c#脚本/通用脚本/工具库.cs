using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class 工具库 : MonoBehaviour
{
    public static GameObject 查找对象(Scene 场景, string 对象名)
    {
        Debug.Log("查找对象：" + 对象名);
        if (!场景.IsValid()) return null;

        foreach(GameObject obj in 场景.GetRootGameObjects())
        {
            Transform t = obj.transform.Find(对象名);
            if(t != null)
            {
                Debug.Log("找到对象：" + t.gameObject.name);
                return t.gameObject;
            }
        }
        return null;
    }

    public static float 向量转角度( Vector2 原点位置, Vector2 目标位置 )
    {
        Vector3 向量 = 目标位置 - 原点位置;
        float 角度 = Mathf.Atan2( 向量.y, 向量.x ) * Mathf.Rad2Deg;

        while(角度 < 0 || 角度 > 360)
        {
            if (角度 < 0)
            {
                角度 += 360;
            }
            else if (角度 > 360)
            {
                角度 -= 360;
            }
        }

        return 角度;
    }

    public static Vector2 角度转向量( float 角度 )
    {
        float 弧度 = 角度 * Mathf.Deg2Rad;
        return new Vector2( Mathf.Cos( 弧度 ), Mathf.Sin( 弧度 ) );
    }

    public static bool 鼠标上滚()
    {
        return Input.GetAxis( "Mouse ScrollWheel" ) > 0f;
    }

    public static bool 鼠标下滚()
    {
        return Input.GetAxis( "Mouse ScrollWheel" ) < 0f;
    }

    public static bool 鼠标点击(int 键位)
    {
        return Input.GetMouseButtonDown(键位);
    }

    public static void 旋转(Transform 旋转对象 ,float 角度)
    {
        旋转对象.rotation = Quaternion.Euler(0, 0, 角度);
    }

    public static GameObject 生成对象(GameObject 对象, Vector3 坐标)
    {
        return Instantiate(对象, 坐标, Quaternion.identity);
    }
}
