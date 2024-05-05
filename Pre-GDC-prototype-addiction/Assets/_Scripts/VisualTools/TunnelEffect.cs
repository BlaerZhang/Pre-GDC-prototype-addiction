using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace _Scripts.VisualTools
{
    public class TunnelEffect : MonoBehaviour
    {
        public SpriteRenderer[] sprites; // 存储参与动画的Sprite对象
        public float shrinkDuration = 3.0f; // 每个Sprite缩小到消失的持续时间
        private float delayBetweenSprites; // Sprite之间动画的延迟
        public float minScale = 0.2f;
        public float maxScale = 2f;
        private int baseSortingOrder = 100; // 基础排序序号

        void Start()
        {
            delayBetweenSprites = shrinkDuration / sprites.Length;
            // 设置Sprite的初始比例和排序
            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i].transform.localScale = Vector3.one * (maxScale - i * 0.1f);
                sprites[i].GetComponent<SpriteRenderer>().sortingOrder = baseSortingOrder - i;
                AnimateSprite(sprites[i], delayBetweenSprites * i);
            }
        }

        void AnimateSprite(SpriteRenderer spriteRenderer, float delay)
        {
            // 使用更平滑的缓动函数
            spriteRenderer.transform.DOScale(minScale, shrinkDuration)
                .SetDelay(delay)
                .SetEase(Ease.InOutSine) // 更自然的缓和效果
                .OnComplete(() => {
                    spriteRenderer.transform.localScale = Vector3.one * maxScale; // 立即重置为最大尺寸
                    // 立即更新排序，将这个Sprite置于最底层
                    spriteRenderer.sortingOrder = FindLowestSortingOrder() - 1;
                    AnimateSprite(spriteRenderer, 0); // 无延迟地重新开始动画
                });
        }

        private int FindLowestSortingOrder()
        {
            int lowest = int.MaxValue;
            foreach (var spr in sprites)
            {
                int order = spr.GetComponent<SpriteRenderer>().sortingOrder;
                if (order < lowest)
                {
                    lowest = order;
                }
            }
            return lowest;
        }
    }
}