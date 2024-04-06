using UnityEngine;
using DG.Tweening; // 引入DoTween命名空间

public class MoveCameraOnClick : MonoBehaviour
{
    void Update()
    {
        // 检测鼠标点击
        if (Input.GetMouseButtonDown(0))
        {
            // 将鼠标位置转换为世界坐标
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 发射射线检测是否点击到了Sprite方块
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                // 当点击到该对象时，移动相机
                MoveCamera();
            }
        }
    }

    void MoveCamera()
    {
        // 使用DoTween移动相机
        Camera.main.transform.DOMoveX(Camera.main.transform.position.x - 14, 1f); // 在1秒内左移14单位
    }
}
