using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Interaction.Clickable;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleDoor : InteractableUIBase
{
    [Title("Doors")]
    [SerializeField] private RectTransform leftDoor;
    [SerializeField] private RectTransform rightDoor;
    [SerializeField] private Image doorMask;
    [SerializeField] private RectTransform uiScale;

    private bool isOpened;

    protected override void ClickableEvent() => OpenDoor();

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (isOpened) return;
        base.OnPointerEnter(eventData);
        leftDoor.DOScaleX(0.9f, 0.25f);
        rightDoor.DOScaleX(0.9f, 0.25f);
    }
    
    public override void OnPointerExit(PointerEventData eventData)
    {
        if (isOpened) return;
        base.OnPointerExit(eventData);
        leftDoor.DOScaleX(1, 0.25f);
        rightDoor.DOScaleX(1, 0.25f);
    }

    private void OpenDoor()
    {
        if (isOpened) return;
        isOpened = true;
        leftDoor.DOScaleX(0f, 1f);
        rightDoor.DOScaleX(0f, 1f);
        uiScale.DOScale(Vector3.one * 20, 1f).OnComplete(() =>
        {
            doorMask.DOFade(1, 0.25f).OnComplete(() => SceneManager.LoadScene("Game Scene"));
        });

    }
}
