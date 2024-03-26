using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    [Header("Drag")]
    public float dragSpeed = 1;
    public bool isDragging = false;
    
    [Header("Feedback")] 
    public float hoverScale = 0.95f;

    private SpriteRenderer cardSprite;
    private Vector2 dragOffset = new Vector2(0, 0);
    
    void Start()
    {
        cardSprite = GetComponentInChildren<SpriteRenderer>();
        isDragging = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseEnter()
    {
        if (!isDragging) cardSprite.DOColor(Color.gray, 0.1f);
    }

    private void OnMouseDown()
    {
        isDragging = true;
        BuyCardManager.instance.cardsToBuy.Remove(this);
        BuyCardManager.instance.cardsToBuy.Insert(0,this);
        cardSprite.transform.DOScale(Vector3.one * hoverScale, 0.1f);
        dragOffset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseDrag()
    {
        Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + (Vector3)dragOffset;
        Vector2 cardToTarget = targetPos - (Vector2)transform.position;
        transform.position += (Vector3)cardToTarget.normalized * MathF.Pow(cardToTarget.magnitude,1f) * dragSpeed * Time.deltaTime;
    }

    private void OnMouseUp()
    {
        isDragging = false;
        cardSprite.transform.DOScale(Vector3.one, 0.1f);
    }

    private void OnMouseExit()
    {
        if (!isDragging) cardSprite.DOColor(Color.white, 0.1f);
    }
}
