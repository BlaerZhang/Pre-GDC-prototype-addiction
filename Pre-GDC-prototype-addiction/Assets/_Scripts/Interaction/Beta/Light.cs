using System.Collections;
using System.Collections.Generic;
using Interaction.Clickable;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

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
