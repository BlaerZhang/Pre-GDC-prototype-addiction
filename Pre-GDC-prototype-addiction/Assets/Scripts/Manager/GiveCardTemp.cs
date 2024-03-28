using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GiveCardTemp : MonoBehaviour
{
    public ScratchCardGenerator generator;
    public GameObject hand;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GiveCard()
    {
        GameObject card = GameObject.Find("newScratchCard");
        GameManager.Instance.GetComponent<ResourceManager>().PlayerGold += (int)generator.currentCardPrize;
        Sequence giveAnimation = DOTween.Sequence();
        giveAnimation
            .Append(card.transform.DOMoveY(card.transform.position.y + 1, 0.5f))
            .Append(hand.transform.DOMoveY(card.transform.position.y + 2, 1f))
            .AppendInterval(0.5f)
            .Append(hand.transform.DOMoveY(9, 1f))
            .Insert(2, card.transform.DOMoveY(9, 1f))
            .OnComplete((() => { SceneManager.LoadScene("Buy Card"); }));

        giveAnimation.Play();
    }
}
