using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

public class BuyCardManager : MonoBehaviour
{
    public static BuyCardManager instance;
    
    [Header("Spawn Cards")]
    public List<Draggable> cardsToBuy = new List<Draggable>();
    public List<Transform> cardSpawnPos;
    public Draggable cardPrefab;

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
        sortCardsOrder();
    }

    public void sortCardsOrder()
    {
        for (int i = 0; i < 5; i++) { cardsToBuy.ToArray()[i].transform.DOMoveZ(0.1f * i, 0); }
    }

    public void SpawnCardsToBuy()
    {
        for (int i = 0; i < 5; i++)
        {
            Draggable cardInstance = Instantiate(cardPrefab);
            cardsToBuy.Add(cardInstance);
            cardsToBuy[i].transform.position = cardSpawnPos[i].position;
        }
        //TODO: Spawn Animation
    }

    public void BuyCard()
    {
        
    }
}
