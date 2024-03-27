using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Interaction;
using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = System.Numerics.Vector3;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    
    [Header("Spawn Cards")]
    // public List<Transform> cardSpawnPos;
    // public Draggable cardPrefab;

    [Header("Pick Cards")] 
    public RectTransform pickArea;
    public Transform cardPurchasePos;

    private void Awake()
    {
        instance = this;
    }
    
    void Start()
    {
        SpawnCardsToBuy();
    }
    
    void Update()
    {
        
    }

    public void SpawnCardsToBuy()
    {
        
    }

    public void PickCard(MenuDraggable card)
    {
        card.transform.DOMove(cardPurchasePos.position, 0.1f); 
    }
    
}
