using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Interaction;
using TMPro;
using UnityEngine;
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
    
    //temp
    public TextMeshProUGUI buyCardPrice;
    
    public void UpdateBuyPrice(int price)
    { 
        buyCardPrice.text = $"${price}";
    }

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Invoke("SpawnCardsToBuy", 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
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
        UpdateBuyPrice(price);
        
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
            for (int i = 0; i < cardsToBuy.Count; i++) { cardsToBuy[i].transform.DOMove(cardSpawnPos[i].position, 0.1f); }
            StartCoroutine(CollectCards());
        }
        else
        {
            UIManager.instance.PlayNotEnoughGoldAnimation();
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

    IEnumerator CollectCards()
    {
        handAnimator.SetTrigger("Collect");
        yield return new WaitForSeconds(0.4f);
        for (int i = 0; i < 4; i++)
        {
            GameObject cardToCollect = cardsToBuy[0].gameObject;
            cardsToBuy.Remove(cardsToBuy[0]);
            Destroy(cardToCollect);
            yield return new WaitForSeconds(0.13f);
        }
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
        scratchOffButton.DOAnchorPosX(125, 0.1f);
    }

    public void DeactivateScratchOffButton()
    {
        scratchOffButton.DOAnchorPosX(-200, 0.1f);
    }
    
}
