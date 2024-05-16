using System.Collections;
using System.Collections.Generic;
using _Scripts.Manager;
using _Scripts.PlayerTools;
using _Scripts.PlayerTools.ConsumableStore;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public class FreeCash : PlayerToolBase, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Title("Incremental")] 
    [SerializeField] private IncrementalManager incrementalManager;
    [SerializeField] private bool unlock = false;
    [SerializeField] private int numberOfCardsPlayedToUnlock = 10;
    
    [Title("Size Modifier")]
    [SerializeField] private bool sizeFeedback = true; 
    [SerializeField] private float hoverSizeModifier = 1.1f; 
    [SerializeField] private float pressSizeModifier = 0.8f; 
    private Vector3 originalLocalScale;
    
    [Title("Sound")] 
    [SerializeField] protected List<AudioClip> hoverSounds = new(); 
    [SerializeField] protected List<AudioClip> pressSounds = new(); 
    [SerializeField] protected List<AudioClip> exitSounds = new();

    protected override void OnEnable()
    {
        ConsumableItemIcon.onTryBuyItem += o => ExpandFromEdge();
        base.OnEnable();
    }
    
    protected override void OnDisable()
    {
        ConsumableItemIcon.onTryBuyItem -= o => ExpandFromEdge();
        base.OnDisable();
    }

    protected override void Start()
    {
        base.Start();
        GetComponent<RectTransform>().DOAnchorPosX(-GetComponent<RectTransform>().rect.width, 0);
        originalLocalScale = transform.localScale;
    }

    protected override void ExpandFromEdge()
    {
        if (GameManager.Instance.resourceManager.PlayerGold < 100 || GameManager.Instance.statsTrackingManager.pricePrizeHistory.Count >= 10) unlock = true;
        if (unlock) base.ExpandFromEdge();
    }
    
    public virtual void OnPointerEnter(PointerEventData eventData) 
    { 
        if (sizeFeedback) ScaleUpClickable(hoverSizeModifier); 
        PlaySound(hoverSounds);
    }
    
    
    public virtual void OnPointerExit(PointerEventData eventData) 
    { 
        if (sizeFeedback) ScaleDownClickable(hoverSizeModifier); 
        PlaySound(exitSounds);
        
    }

    public virtual void OnPointerDown(PointerEventData eventData) 
    { 
        if (sizeFeedback) ScaleUpClickable(pressSizeModifier); 
        PlaySound(pressSounds); 
        ClickableEvent();
    }
    
    public virtual void OnPointerUp(PointerEventData eventData) 
    { 
        if (sizeFeedback) ScaleDownClickable(pressSizeModifier);
        
    }
    protected void ClickableEvent()
    {
        incrementalManager.SwitchToIncremental();
    }
    
    
    private void PlaySound(List<AudioClip> audioClips)
    {
        if (audioClips.Count > 0) 
            GameManager.Instance.audioManager.PlaySound(audioClips[Random.Range(0, audioClips.Count)]);
    }
    
    private void ScaleUpClickable(float modifier)
    {
        transform.DOScale(originalLocalScale * modifier, 0.3f).SetEase(Ease.OutElastic);
    }
    
    private void ScaleDownClickable(float modifier)
    {
        transform.DOScale(originalLocalScale, 0.3f).SetEase(Ease.OutElastic);
    }
}
