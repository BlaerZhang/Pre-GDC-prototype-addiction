using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIReflection : MonoBehaviour
{
    public float reflectionTimeInterval = 5f;
    public float reflectionDuration = 1f;
    public float reflectionAngle = -15f;
    public Sprite reflectionSprite;
    [Range(0, 1)] public float reflectionTransparency = 0.5f;

    private Image image;
    private RectMask2D uiMask;
    private RectTransform reflectionParent;
    
    void Start()
    {
        reflectionParent = transform.Find("Reflection Parent").GetComponent<RectTransform>();
        image = GetComponent<Image>();
        reflectionParent.anchoredPosition = new Vector3(image.rectTransform.rect.width * 2, 0, 0);
        InvokeRepeating("Reflect", Random.Range(0f, 5f), reflectionTimeInterval);
    }

    [Button("Initialize")]
    void Init()
    {
        image = GetComponent<Image>();
        uiMask = this.AddComponent<RectMask2D>();

        reflectionParent = new GameObject("Reflection Parent").AddComponent<RectTransform>();
        reflectionParent.transform.parent = this.transform;
        reflectionParent.transform.localPosition = Vector3.zero;
        reflectionParent.localScale = Vector3.one;
        
        CreateReflection(-75f);
        CreateReflection(150);
        CreateReflection(75f);
    }
    
    void CreateReflection(float XPos)
    {
        GameObject square = new GameObject("Square");
        square.transform.parent = reflectionParent.transform;
        square.transform.SetAsLastSibling();

        RectTransform squareRect = square.AddComponent<RectTransform>();
        squareRect.anchoredPosition = Vector2.zero;
        squareRect.sizeDelta = new Vector2(Random.Range(25f, 75f), 1000f);
        
        Image reflectionImage = square.AddComponent<Image>();
        reflectionImage.sprite = reflectionSprite;
        reflectionImage.color = new Color(1, 1, 1, reflectionTransparency);
        squareRect.rotation = Quaternion.Euler(new Vector3(0, 0, reflectionAngle));
        squareRect.anchoredPosition = new Vector3(XPos, 0, 0);
    }

    void Reflect()
    {
        reflectionParent.anchoredPosition = new Vector3(image.rectTransform.rect.width * 2, 0, 0);
        reflectionParent.DOAnchorPosX(-image.rectTransform.rect.width * 2, reflectionDuration).SetEase(Ease.Linear);
    }
}
