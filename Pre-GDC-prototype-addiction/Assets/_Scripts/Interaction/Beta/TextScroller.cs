using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextScroller : MonoBehaviour
{
    [Title("Basic")] 
    public TextMeshProUGUI scrollingTextTMPPrefab;
    public float scrollSpeed = 1;
    public float gapBetweenTexts = 100;

    [Title("Text")] 
    public string textHeader = "■";
    public List<string> idleStrings;

    private int currentIdleIndex = 0;
    private Image scrollMask;
    private float maskWidth = 2000;
    private TextMeshProUGUI currentScrollingText;
    private bool insertedTextPoped = true;
    private string currentInsertedText;

    public static Action<string> InsertScrollingTextHandler; //use this action to insert scrolling text

    private void OnEnable()
    {
        InsertScrollingTextHandler += InsertScrollingText;
    }

    private void OnDisable()
    {
        InsertScrollingTextHandler -= InsertScrollingText;
    }

    void Start()
    {
        scrollMask = GetComponent<Image>();
        maskWidth = scrollMask.rectTransform.sizeDelta.x;
        insertedTextPoped = true;
        PopScrollingText(idleStrings[0]);
    }
    
    void Update()
    {
        if (Mathf.Abs(currentScrollingText.rectTransform.anchoredPosition.x) < currentScrollingText.preferredWidth + gapBetweenTexts) return;
        switch (insertedTextPoped)
        {
            case true:
                currentIdleIndex = (currentIdleIndex + 1) % idleStrings.Count;
                PopScrollingText(idleStrings[currentIdleIndex]);
                break;
            case false:
                PopScrollingText(currentInsertedText);
                insertedTextPoped = true;
                break;
        }
        
    }

    public TextMeshProUGUI PopScrollingText(string textContent)
    {
         TextMeshProUGUI textInstance = Instantiate(scrollingTextTMPPrefab, this.transform); //instantiate prefab
         textInstance.text = $"{textHeader} {textContent}"; //set text
         textInstance.rectTransform.anchoredPosition = new Vector2(0, 0); //init pos
         currentScrollingText = textInstance; //update status
         float currentTextWidth = currentScrollingText.preferredWidth; //get text width
         textInstance.rectTransform.DOAnchorPosX(-(maskWidth + currentTextWidth),
             (maskWidth + currentTextWidth) / scrollSpeed).SetEase(Ease.Linear).OnComplete(() =>
         {
             Destroy(textInstance.gameObject);
         });
         return textInstance;
    }
    
    public void InsertScrollingText(string textContent)
    {
        insertedTextPoped = false;
        currentInsertedText = textContent;
    }

    // IEnumerator PopIdleText()
    // {
    //     for (int i = 0; i < idleStrings.Count + 1; i = (i + 1) % idleStrings.Count)
    //     {
    //         TextMeshProUGUI currentTMP = PopScrollingText(idleStrings[i]);
    //         yield return new WaitForSeconds(currentTMP.preferredWidth / scrollSpeed + 1);
    //     }
    // }
}
