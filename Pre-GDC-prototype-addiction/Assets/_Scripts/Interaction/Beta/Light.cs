using _Scripts.Interaction.InteractableUI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Interaction.Beta
{
    public class Light : InteractableUIBase
    {
        [Title("Light")]
        public RawImage mask;
        public Color lightOffColor;
        private bool isLightOn = true;
    
        protected override void ClickableEvent()
        {
            isLightOn = !isLightOn;
            mask.enabled = !isLightOn;
            mask.color = isLightOn ? Color.clear : lightOffColor;
        }
    }
}
