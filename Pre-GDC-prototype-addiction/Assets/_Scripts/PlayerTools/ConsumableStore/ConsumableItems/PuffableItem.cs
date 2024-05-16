using _Scripts.PlayerTools.ConsumableStore.ConsumableEffect;
using _Scripts.PlayerTools.ConsumableStore.ConsumableItems;
using DG.Tweening;

namespace _Scripts.ConsumableStore
{
    public class PuffableItem : ConsumableItemBase
    {
        private void OnEnable()
        {
            PuffableEffect.onStopSmoking += RemoveItem;
        }

        private void OnDisable()
        {
            PuffableEffect.onStopSmoking -= RemoveItem;
        }

        protected override void ClickableEvent()
        {
            if (isDisabled) return;

            transform.DOScale(1, 0.3f).SetEase(Ease.OutElastic);
            UseItem();
        }
    }
}