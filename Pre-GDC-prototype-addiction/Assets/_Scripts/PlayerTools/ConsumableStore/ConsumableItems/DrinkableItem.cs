using _Scripts.ConsumableStore.ConsumableEffect;
using _Scripts.Manager;
using _Scripts.PlayerTools.ConsumableStore.ConsumableItems;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.ConsumableStore
{
    public class DrinkableItem : ConsumableItemBase
    {
        public GameObject targetThrowPoint;
        public float throwDuration;
        public AudioClip throwSound;

        private void OnEnable()
        {
            DrinkableSprayParticle.onWaterSprayParticleDisappear += RemoveItem;
        }

        private void OnDisable()
        {
            DrinkableSprayParticle.onWaterSprayParticleDisappear -= RemoveItem;
        }

        protected override void ClickableEvent()
        {
            if (isConsuming) return;

            transform.DOScale(1, 0.3f).SetEase(Ease.OutElastic);
            UseItem();

            ReduceLiquidInBottle();
        }

        protected override void RemoveItem()
        {
            base.RemoveItem();

            GameObject drinkableObject = new GameObject("drinkableToThrow");
            var drinkableObjectImage = drinkableObject.AddComponent<Image>();
            drinkableObjectImage.sprite = GetComponent<Image>().sprite;
            drinkableObjectImage.SetNativeSize();

            GameObject drinkableToThrow = Instantiate(drinkableObject, transform.parent);
            drinkableToThrow.transform.position = transform.position;

            Vector3 centerPos = (transform.position + targetThrowPoint.transform.position) / 2;
            float distance = Vector3.Distance(transform.position, targetThrowPoint.transform.position);
            centerPos.y += (distance / 2);

            GameManager.Instance.audioManager.PlaySound(throwSound);

            drinkableToThrow.transform
                .DOPath(new[] { transform.position, centerPos, targetThrowPoint.transform.position }, throwDuration,
                    PathType.CatmullRom)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    Destroy(drinkableObject);
                });

            drinkableToThrow.transform.DORotate(new Vector3(0, 0, -1800), throwDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear);
        }

        private void ReduceLiquidInBottle()
        {
            // TODO:
        }
    }
}