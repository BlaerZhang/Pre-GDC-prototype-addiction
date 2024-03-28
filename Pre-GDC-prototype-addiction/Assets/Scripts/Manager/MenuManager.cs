using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Interaction;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Vector3 = System.Numerics.Vector3;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;
    
    [Header("Spawn Cards")]
    // public List<Transform> cardSpawnPos;
    // public Draggable cardPrefab;

    [Header("Pick Cards")] 
    public Transform cardPurchasePos;

    [Header("Pick Area")] 
    public RectTransform pickArea;
    [Range(0,1)] public float areaActivateThreshold = 0.5f;
    public float areaActivateDistance = 200;
    [Range(0,1)] public float areaStopThreshold = 0.8f;
    public float areaMoveAmount = 1000;

    [Header("Move to Incremental")] 
    public RectTransform incrementalButton;
    

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
        if (card.price <= GameManager.Instance.GetComponent<ResourceManager>().PlayerGold)
        {
            // card.transform.DOMove(cardPurchasePos.position, 0.1f);
            GameManager.Instance.lastPickPrice = card.price;
            GameManager.Instance.lastPickTier = card.tier;
            LoadBuy();
        }
        else
        {
            GameManager.Instance.uiManager.PlayNotEnoughGoldAnimation();
        }
      
    }

    public void AdjustPickArea(Transform posterPos)
    {
        float cardXPosOnViewport = Camera.main.WorldToViewportPoint(posterPos.position).x;

        if (cardXPosOnViewport < areaStopThreshold && cardXPosOnViewport > areaActivateThreshold) 
            pickArea.anchoredPosition = new Vector2(-areaActivateDistance - (cardXPosOnViewport - areaActivateThreshold) * areaMoveAmount, pickArea.anchoredPosition.y);
    }

    public void ActivatePickArea()
    {
        pickArea.DOAnchorPosX(-areaActivateDistance, 0.1f);
    }

    public void DeactivatePickArea()
    {
        pickArea.DOAnchorPosX(0, 0.1f);
    }
    
    public void ActivateIncrementalButton()
    {
        incrementalButton.DOAnchorPosX(125, 0.1f);
    }

    public void DeactivateIncrementalButton()
    {
        incrementalButton.DOAnchorPosX(-200, 0.1f);
    }

    public void LoadIncremental()
    {
        SceneManager.LoadScene("Incremental");
    }

    public void LoadBuy()
    {
        SceneManager.LoadScene("Buy Card");
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }
    
}
