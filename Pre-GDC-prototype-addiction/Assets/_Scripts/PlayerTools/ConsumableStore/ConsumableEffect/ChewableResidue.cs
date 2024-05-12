using Interaction.Clickable;
using UnityEngine;

namespace _Scripts.ConsumableStore.ConsumableEffect
{
    public class ChewableResidue : InteractableUIBase
    {
        [SerializeField] private int initialClickRequiredToRemove = 3;
        private int currentClickRequired;

        protected override void Start()
        {
            base.Start();
            currentClickRequired = initialClickRequiredToRemove;
        }

        protected override void ClickableEvent()
        {
            TryRemove();
        }

        private void TryRemove()
        {
            print("Trying to remove chewable residue");
            currentClickRequired--;

            // TODO: click on residue effect

            if (currentClickRequired == 0)
            {
                Remove();
            }
        }

        private void Remove()
        {
            print("Chewable residue removed");
            // TODO: remove residue effect
            Destroy(gameObject);
        }
    }
}