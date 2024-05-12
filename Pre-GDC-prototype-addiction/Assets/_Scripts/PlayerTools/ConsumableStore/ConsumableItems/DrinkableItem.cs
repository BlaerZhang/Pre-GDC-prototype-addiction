using _Scripts.ConsumableStore.ConsumableEffect;
using _Scripts.PlayerTools.ConsumableStore.ConsumableItems;
using DG.Tweening;
using UnityEngine;

namespace _Scripts.ConsumableStore
{
    public class DrinkableItem : ConsumableItemBase
    {
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

            // TODO: remove drinkable effect
        }

        private void ReduceLiquidInBottle()
        {

            // on complete,
        }
    }
}