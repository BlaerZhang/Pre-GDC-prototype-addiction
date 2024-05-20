using _Scripts.PlayerTools.Payphone;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Scripts.Interaction.PosterPicking
{
    public class ScrollView : MonoBehaviour
    {
        [HideInInspector] public bool isScrollLocked = false;
        protected bool PayphoneState = false;

        [Title("Scroll View Settings")]
        [SerializeField] public Transform scrollViewHolder;

        [SerializeField] protected float initialYPosition = -2.25f;
        [SerializeField] public Vector2 scrollLimits; // 滚动限制
        [SerializeField] protected float smoothTime = 1f; // 平滑时间
        [SerializeField] [PropertyRange(500, 2000)] protected float scrollSpeedSensitivity = 300f;

        private float currentVelocity = 0f; // 当前速度，用于平滑减速

        protected virtual void OnEnable()
        {
            PayphoneManager.onPhoneStateChanged += isInMessage => { PayphoneState = isInMessage; };
        }
        
        protected virtual void OnDisable()
        {
            PayphoneManager.onPhoneStateChanged -= isInMessage => { PayphoneState = isInMessage; };
        }

        protected virtual void Start()
        {
            ResetScrollViewToInitialPosition();
        }

        // private void OnDisable()
        // {
        //     RecordLastYPosition();
        // }

        void Update()
        {
            if (!isScrollLocked && !PayphoneState) Scroll();
        }

        private void Scroll()
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            float targetVelocity = scrollInput * scrollSpeedSensitivity; // 目标速度基于输入和一个自定义系数

            // 平滑过渡当前速度到目标速度
            currentVelocity = Mathf.Lerp(currentVelocity, targetVelocity, Time.fixedDeltaTime / smoothTime);

            // 更新位置
            scrollViewHolder.position += Vector3.up * currentVelocity * Time.fixedDeltaTime;
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