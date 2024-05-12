using UnityEngine;

namespace _Scripts
{
    public class CursorBoundary : MonoBehaviour
    {
        public SpriteRenderer targetSpriteRenderer; // 要限制的目标Sprite
        private Bounds spriteBounds;

        void Start()
        {
            // 获取Sprite的边界
            spriteBounds = targetSpriteRenderer.bounds;
        }

        void Update()
        {
            // 获取鼠标的屏幕坐标并转换为世界坐标
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0; // 2D环境下，忽略Z轴

            // 限制鼠标位置在Sprite的边界范围内
            mouseWorldPosition.x = Mathf.Clamp(mouseWorldPosition.x, spriteBounds.min.x, spriteBounds.max.x);
            mouseWorldPosition.y = Mathf.Clamp(mouseWorldPosition.y, spriteBounds.min.y, spriteBounds.max.y);

            // 在这里进行其他逻辑，例如使用鼠标位置来控制其他对象
            Debug.Log("Clamped Mouse World Position: " + mouseWorldPosition);
        }
    }
}
