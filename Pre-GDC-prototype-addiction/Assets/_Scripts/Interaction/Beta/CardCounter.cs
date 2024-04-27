using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Manager;
using UnityEngine;
using UnityEngine.UI;

public class CardCounter : MonoBehaviour
{
    [Header("Basic")] 
    public Image counterIconPrefab;
    public Sprite iconEmpty;
    public Sprite iconHalf;
    public Sprite iconFull;

    [Header("Grid")] 
    public int row = 4;
    public int column = 4;
    private Vector2 startOffset;
    private float gapBetweenIconsHorizontal;
    private float gapBetweenIconsVertical;

    [Header("Feedback")] 
    [Range(0, 1)] public float iconPopDuration = 0.5f;
    [Range(0, 1)] public float iconClearDuration = 0.5f;

    private List<Image> currentIconList;

    private void OnEnable()
    {
        StatsTrackingManager.OnPricePrizeHistoryUpdated += UpdateCardCounter;
    }

    private void OnDisable()
    {
        StatsTrackingManager.OnPricePrizeHistoryUpdated -= UpdateCardCounter;
    }

    void Start()
    {
        currentIconList = new List<Image>();
        
        float bgWidth = GetComponent<Image>().rectTransform.rect.width;
        float bgHeight = GetComponent<Image>().rectTransform.rect.height;
        float iconWidth = counterIconPrefab.rectTransform.rect.width;
        float iconHeight = counterIconPrefab.rectTransform.rect.height;

        gapBetweenIconsHorizontal = iconWidth + (bgWidth - column * iconWidth) / (column + 1);
        gapBetweenIconsVertical = iconHeight + (bgHeight - row * iconHeight) / (row + 1);
        startOffset = new Vector2(gapBetweenIconsHorizontal - iconWidth / 2, -gapBetweenIconsVertical + iconHeight / 2);

    }

    public void UpdateCardCounter(int price, int prize)
    {
        if (currentIconList.Count >= row * column) //check if full
        {
            ClearRow(price, prize);
        }
        else
        {
            AddCounterIcon(price, prize);
        }
    }

    public void AddCounterIcon(int price, int prize)
    {
        Image newIcon = Instantiate(counterIconPrefab, this.transform); //instantiate
        newIcon.transform.localScale = Vector3.zero; //init scale
        newIcon.sprite = prize > price ? iconFull : (prize == 0 ? iconEmpty : iconHalf); //set sprite
        currentIconList.Add(newIcon); //add to list
        
        //place icon
        int iconRow = Mathf.CeilToInt((float)currentIconList.Count / column); //calc row
        print($"row{row}");
        int iconColumn = currentIconList.Count % column == 0 ? column : currentIconList.Count % column; //calc column
        print($"column{column}");
        Vector2 iconPos = new Vector2(startOffset.x + (iconColumn - 1) * gapBetweenIconsHorizontal,
            startOffset.y - (iconRow - 1) * gapBetweenIconsVertical); //calc pos
        print($"pos{iconPos}");
        newIcon.rectTransform.anchoredPosition = iconPos; //set pos
        newIcon.transform.DOScale(1, iconPopDuration).SetEase(Ease.OutElastic);
    }

    public void ClearRow(int price, int prize)
    {
        GameObject tempIconParent = new GameObject(); //instantiate empty
        RectTransform tempRect = tempIconParent.AddComponent<RectTransform>(); //add rect
        tempIconParent.transform.parent = this.transform; //set parent
        foreach (var icon in currentIconList) { icon.transform.parent = tempIconParent.transform; } //set children
        tempRect.DOAnchorPosY(tempRect.anchoredPosition.y + gapBetweenIconsVertical, iconClearDuration).SetEase(Ease.OutElastic).OnComplete(() =>
        {
            foreach (var icon in tempIconParent.GetComponentsInChildren<Image>()) { icon.transform.parent = this.transform; } //detach all icons and set card counter as parent
            Destroy(tempIconParent); //delete temp parent
            //remove first row icon
            for (int i = 0; i < column; i++)
            {
                Image iconToDelete = currentIconList[0];
                currentIconList.RemoveAt(0);
                Destroy(iconToDelete.gameObject);
            }
            AddCounterIcon(price, prize); //add new icon
        });
    }
}
