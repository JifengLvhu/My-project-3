using UnityEngine;

public class 钢铁激光显示 : MonoBehaviour,I激光显示,I池对象重置
{
    private SpriteRenderer 贴图;
    private float 原始半径;
    
    private void Awake()
    {
        贴图 = GetComponent<SpriteRenderer>();
        原始半径 = 贴图.sprite.bounds.size.x / 2f;
    }
    
    public void 更新显示(Vector2 起点, Vector2 方向, 通用激光Pattern pattern, Color 显示颜色)
    {
        贴图.color = 显示颜色;
        float 目标缩放 = pattern.外半径 / 原始半径;
        贴图.transform.localScale = new Vector3(目标缩放, 目标缩放, 1f);

        方向 = 方向.normalized;
        float 角度 = Mathf.Atan2(方向.y, 方向.x) * Mathf.Rad2Deg;
        贴图.transform.rotation = Quaternion.Euler(0, 0, 角度);
        贴图.transform.position = 起点;
    }

    public void 池对象重置()
    {
        贴图.color = Color.clear;
        贴图.transform.localScale = Vector3.one;
    }
}
