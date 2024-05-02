using System;
using UnityEngine;

namespace Interaction
{
    public class ScrollView : MonoBehaviour
    {
        public float lastMenuYPosition = -2.25f;
        public Vector2 scrollLimits; // 滚动限制
        public float smoothTime = 0.3f; // 平滑时间

        private float currentVelocity = 0f; // 当前速度，用于平滑减速

        // Start is called before the first frame update
        void Start()
        {
            // 设置初始位置
            transform.position = new Vector3(transform.position.x, lastMenuYPosition, transform.position.z);
        }

        private void OnDisable()
        {
            RecordLastYPos();
        }

        // Update is called once per frame
        void Update()
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            float targetVelocity = scrollInput * 300f; // 目标速度基于输入和一个自定义系数

            // 平滑过渡当前速度到目标速度
            currentVelocity = Mathf.Lerp(currentVelocity, targetVelocity, Time.deltaTime / smoothTime);

            // 更新位置
            transform.position += Vector3.up * currentVelocity * Time.deltaTime;
            transform.position = new Vector3(transform.position.x,
                Mathf.Clamp(transform.position.y, scrollLimits.x, scrollLimits.y),
                transform.position.z);

            // 当速度非常小的时候，直接设置为0，避免无限趋近
            if (Mathf.Abs(currentVelocity) < 0.001f) currentVelocity = 0;
        }

        void RecordLastYPos()
        {
            // 记录当前Y位置
            lastMenuYPosition = transform.position.y;
        }
    }
}