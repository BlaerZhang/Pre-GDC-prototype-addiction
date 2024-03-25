using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TMPro;

public class IncrementalManager : MonoBehaviour
{
    public int clickerLevel = 1;

    public List<int> productivityPerLevel;

    public List<int> upgradePricePerLevel;

    private ResourceManager resourceManager;

    [Header("Feedback")]
    public bool feedback;
    public GameObject textFeedback;
    public List<AudioClip> soundFeedbacks;
    
    void Start()
    {
        //TODO: Put ResourceManager Under GameManager
        resourceManager = GetComponent<ResourceManager>();
        UIManager.instance.UpdateUpgradePrice($"${upgradePricePerLevel[clickerLevel - 1]}");
        
    }


    void Update()
    {
        
    }

    public void OnClick(RectTransform buttonTransform)
    {
        resourceManager.PlayerGold += productivityPerLevel[clickerLevel - 1];
        if (feedback)
        {
            PlayFeedbackAnimation(buttonTransform);
            if (soundFeedbacks.Count > 0) AudioManager.instance.PlaySound(soundFeedbacks[Random.Range(0, soundFeedbacks.Count)]);
        }
    }

    public void OnUpgrade()
    {
        if (resourceManager.PlayerGold < upgradePricePerLevel[clickerLevel - 1]) 
        {
            //TODO: Play not enough feedback
            return;
        } 
        resourceManager.PlayerGold -= upgradePricePerLevel[clickerLevel - 1];
        clickerLevel++;
        UIManager.instance.UpdateUpgradePrice($"${upgradePricePerLevel[clickerLevel - 1]}");
    }
    
    private void PlayFeedbackAnimation(RectTransform buttonTransform)
    {
        Vector3 targetPos = buttonTransform.anchoredPosition + new Vector2(Random.Range(-50f, 50f), 200);

        GameObject feedback = Instantiate(textFeedback, buttonTransform);
        TextMeshProUGUI feedbackText = feedback.GetComponentInChildren<TextMeshProUGUI>();
        Sequence feedbackSequence = DOTween.Sequence();

        feedbackText.text = $"${productivityPerLevel[clickerLevel - 1]}";
        
        feedbackSequence
            .Append(feedbackText.rectTransform.DOScale(Vector3.zero, 0))
            .Append(feedbackText.rectTransform.DOScale(Vector3.one, 0.5f))
            .Insert(0, feedbackText.rectTransform.DOAnchorPos(targetPos, 2f))
            .Insert(1, feedbackText.DOFade(0, 1f))
            .OnComplete((() => { Destroy(feedback); }));
        feedbackSequence.Play();
    }
}
