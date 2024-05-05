using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Interaction
{
    public class ScrollView : MonoBehaviour
    {
        [HideInInspector] public bool isScrollLocked = false;

        [Title("Scroll View Settings")]
        [SerializeField] protected Transform scrollViewHolder;

        [SerializeField] protected float initialYPosition = -2.25f;
        [SerializeField] protected Vector2 scrollLimits; // 滚动限制
        [SerializeField] protected float smoothTime = 0.3f; // 平滑时间

        private float currentVelocity = 0f; // 当前速度，用于平滑减速

        void Start()
        {
            ResetScrollViewToInitialPosition();
        }

        // private void OnDisable()
        // {
        //     RecordLastYPosition();
        // }

        void Update()
        {
            if (isScrollLocked) return;
            Scroll();
        }

        private void Scroll()
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            float targetVelocity = scrollInput * 300f; // 目标速度基于输入和一个自定义系数

            // 平滑过渡当前速度到目标速度
            currentVelocity = Mathf.Lerp(currentVelocity, targetVelocity, Time.deltaTime / smoothTime);

            // 更新位置
            scrollViewHolder.position += Vector3.up * currentVelocity * Time.deltaTime;
            scrollViewHolder.position = new Vector3(scrollViewHolder.position.x,
                Mathf.Clamp(scrollViewHolder.position.y, scrollLimits.x, scrollLimits.y),
                scrollViewHolder.position.z);

            // 当速度非常小的时候，直接设置为0，避免无限趋近
            if (Mathf.Abs(currentVelocity) < 0.001f) currentVelocity = 0;
        }

        protected void ResetScrollViewToInitialPosition()
        {
            // 设置初始位置
            scrollViewHolder.position = new Vector3(scrollViewHolder.position.x, initialYPosition, scrollViewHolder.position.z);
        }

        // void RecordLastYPosition()
        // {
        //     // 记录当前Y位置
        //     initialYPosition = scrollViewHolder.position.y;
        // }
    }
}