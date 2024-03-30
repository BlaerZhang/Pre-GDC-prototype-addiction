using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Interaction;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Vector3 = System.Numerics.Vector3;

public class BuyCardManager : MonoBehaviour
{
    public static BuyCardManager instance;
    
    [Header("Spawn Cards")]
    public List<Draggable> cardsToBuy = new List<Draggable>();
    public List<Transform> cardSpawnPos;
    public Transform cardPurchasePos;
    public Draggable cardPrefab;

    [Header("Buy Cards")] 
    public int price;

    [Header("Hand")] 
    public Animator handAnimator;
    
    [Header("Buy Area")] 
    public RectTransform buyArea;
    [Range(0,1)] public float areaActivateThreshold = 0.5f;
    public float areaActivateDistance = 200;
    [Range(0,1)] public float areaStopThreshold = 0.2f;
    public float areaMoveAmount = 1000;
    
    [Header("Move to Scratch-off")] 
    public RectTransform scratchOffButton;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        GameManager.Instance.switchSceneManager.onSceneChange += OnSceneChange;
    }

    private void OnDisable()
    {
        GameManager.Instance.switchSceneManager.onSceneChange -= OnSceneChange;
    }

    private void OnSceneChange(string sceneLoaded)
    {
        if (sceneLoaded == "Buy Card")
        {
            SpawnCardsToBuy();
            ActivateScratchOffButton();
        }
    }

    public void sortCardsOrder()
    {
        if (cardsToBuy.Count <= 0) return;
        for (int i = 0; i < cardsToBuy.Count; i++)
        {
            cardsToBuy.ToArray()[i].transform.DOMoveZ(0.1f * i, 0);
        }
    }

    public void SpawnCardsToBuy()
    {
        price = GameManager.Instance.lastPickPrice;
        GameManager.Instance.uiManager.UpdateBuyPrice(price);
        
        for (int i = 0; i < 5; i++)
        {
            Draggable cardInstance = Instantiate(cardPrefab, cardPurchasePos.position, Quaternion.identity);
            cardsToBuy.Add(cardInstance);
        }

        StartCoroutine(DealCards());
    }

    public void BuyCard(Draggable card)
    {
        if (price <= GameManager.Instance.GetComponent<ResourceManager>().PlayerGold)
        {
            GameManager.Instance.GetComponent<ResourceManager>().PlayerGold -= price;
            
            cardsToBuy.Remove(card);
            card.transform.DOMove(cardPurchasePos.position, 0.1f);
            StartCoroutine(CollectCards(true));
        }
        else
        {
            GameManager.Instance.uiManager.PlayNotEnoughGoldAnimation();
        }
    }

    IEnumerator DealCards()
    {
        handAnimator.SetTrigger("Deal");
        yield return new WaitForSeconds(0.18f);
        for (int i = 0; i < 5; i++)
        {
            cardsToBuy[i].transform.position = cardSpawnPos[i].position;
            yield return new WaitForSeconds(0.11f);
        }
    }

    public void CollectCard(bool isPurchased)
    {
        StartCoroutine(CollectCards(isPurchased));
    }

    IEnumerator CollectCards(bool isPurchased)
    {
        //cards move to slots
        for (int i = 0; i < cardsToBuy.Count; i++) { cardsToBuy[i].transform.DOMove(cardSpawnPos[i].position, 0.1f); }
        
        //play hand animation
        handAnimator.SetTrigger("Collect");
        
        yield return new WaitForSeconds(0.4f);

        int numberOfCardsToCollect = cardsToBuy.Count;
        
        //destroy cards with interval
        for (int i = 0; i < numberOfCardsToCollect; i++)
        {
            GameObject cardToCollect = cardsToBuy[0].gameObject;
            cardsToBuy.Remove(cardsToBuy[0]);
            Destroy(cardToCollect);
            yield return new WaitForSeconds(0.13f);
        }

        if (isPurchased) GameManager.Instance.switchSceneManager.ChangeScene("Interaction");
    }

    public void AdjustBuyArea(Transform cardPos)
    {
        float cardYPosOnViewport = Camera.main.WorldToViewportPoint(cardPos.position).y;
        
        if(cardYPosOnViewport < areaActivateThreshold && cardYPosOnViewport > areaStopThreshold)
            buyArea.anchoredPosition = new Vector2(buyArea.anchoredPosition.x, areaActivateDistance + (areaActivateThreshold - cardYPosOnViewport) * areaMoveAmount);
    }

    public void ActivateBuyArea()
    {
        buyArea.DOAnchorPosY(areaActivateDistance, 0.1f);
    }
    
    public void DeactivateBuyArea()
    {
        buyArea.DOAnchorPosY(0, 0.1f);
    }
    
    public void ActivateScratchOffButton()
    {
        scratchOffButton.gameObject.SetActive(true);
        scratchOffButton.DOAnchorPosX(125, 0.1f);
    }

    public void DeactivateScratchOffButton()
    {
        scratchOffButton.DOAnchorPosX(-200, 0.1f).OnComplete((() => { scratchOffButton.gameObject.SetActive(false); }));
    }
}
