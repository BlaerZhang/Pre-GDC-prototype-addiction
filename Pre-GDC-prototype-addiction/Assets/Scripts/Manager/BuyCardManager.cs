using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    public RectTransform buyArea;

    [Header("Hand")] 
    public Animator handAnimator;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnCardsToBuy();
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
        for (int i = 0; i < 5; i++)
        {
            Draggable cardInstance = Instantiate(cardPrefab, cardPurchasePos.position, Quaternion.identity);
            cardsToBuy.Add(cardInstance);
        }

        StartCoroutine(DealCards());
    }

    public void BuyCard(Draggable card)
    {
        cardsToBuy.Remove(card);
        card.transform.DOMove(cardPurchasePos.position, 0.1f);
        for (int i = 0; i < cardsToBuy.Count; i++) { cardsToBuy[i].transform.DOMove(cardSpawnPos[i].position, 0.1f); }
        StartCoroutine(CollectCards());
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
    
}
