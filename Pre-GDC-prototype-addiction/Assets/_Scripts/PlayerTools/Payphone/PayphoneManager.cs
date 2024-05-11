using System;
using System.Collections.Generic;
using _Scripts.PlayerTools.Payphone;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using DG.Tweening;
using Interaction.Clickable;
using ScratchCardGeneration.Utilities;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PayphoneManager : InteractableUIBase
{
    [Title("Message Data")]
    [SerializeField] private ScriptablePayphoneMessages scriptablePayphoneMessages;

    [Title("Text Bubble")]
    [SerializeField] private GameObject textBubbleLayoutGroup;
    [SerializeField] private GameObject textBubblePrefab;
    [SerializeField] private int maxBubbleAmount;

    private List<GameObject> currentDisplayBubbles;
    private int currentBubbleAmount = 0;

    [Title("Text Displaying")]
    [SerializeField] private Volume textDisplayVolume;
    [SerializeField] private float textShowSpeed;
    [SerializeField] private float textFadeModifier;
    [SerializeField] private float textFadeDuration;

    // reset required
    private List<string> lastMessageList;
    private List<string> currentMessageList;
    private int messageIndexCounter = 0;

    private bool inTextDisplayMode = false;
    private bool isTextShowing = false;

    private Tween currentTextDisplayTween;

    // come with the message id that could be used in the dictionary to retrieve the message text
    public static Action<string> onPhoneMessageSent;

    private void OnEnable()
    {
        onPhoneMessageSent += RetrieveMessage;
    }

    private void OnDisable()
    {
        onPhoneMessageSent -= RetrieveMessage;
    }

    protected override void Start()
    {
        base.Start();
        currentDisplayBubbles = new List<GameObject>(maxBubbleAmount);
        textDisplayVolume.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            if (currentMessageList == null) RetrieveMessage("0");
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (currentMessageList == null) RetrieveMessage("1");
        }

        if (Input.GetMouseButtonDown(1) & inTextDisplayMode)
        {
            CheckTextDisplay();
        }
    }

    private void RetrieveMessage(string messageId)
    {
        if (!scriptablePayphoneMessages.PhoneMessageDict.TryGetValue(messageId, out List<string> messageList))
        {
            Debug.LogError("No message found in scriptable payphone message lists, please check your message id");
            return;
        }

        currentMessageList = messageList;
        DisplayMessage();
    }

    private void CheckTextDisplay()
    {
        if (isTextShowing)
        {
            // stop the tween
            currentTextDisplayTween.Pause();
        }
        else DisplayMessage();
    }

    private void ResetStatesAfterMessage()
    {
        // remove blur background
        textDisplayVolume.enabled = false;

        // remove all bubbles
        foreach (var bubble in currentDisplayBubbles)
        {
            RemoveTextBubble(bubble);
        }
        currentDisplayBubbles.Clear();

        // reset states
        messageIndexCounter = 0;
        inTextDisplayMode = false;
        currentBubbleAmount = 0;

        // set last msg list for next use
        lastMessageList = Utils.DeepCopyList(currentMessageList);
        currentMessageList = null;
    }

    /// <summary>
    /// called when click the screen when in the messaging mode
    /// </summary>
    private void DisplayMessage()
    {
        inTextDisplayMode = true;
        // TODO: lock all other interactions: panel block UI interaction, lock sprite
        // blur background
        textDisplayVolume.enabled = true;
        // textDisplayVolume.TryGet(out DepthOfField depthOfField);

        // when all messages are played
        if (messageIndexCounter >= currentMessageList.Count)
        {
            ResetStatesAfterMessage();
            return;
        }

        // TODO: newest message should be at the same position, and older ones needs to be pushed up
        if (currentBubbleAmount == maxBubbleAmount)
        {
            GameObject textBubbleToRemove = currentDisplayBubbles[0];
            currentDisplayBubbles.RemoveAt(0);
            RemoveTextBubble(textBubbleToRemove);
        }
        else
        {
            currentBubbleAmount++;
        }

        GameObject newTextBubble = AddTextBubble();
        var textUI = newTextBubble.GetComponentInChildren<TextMeshProUGUI>();

        // text displays one by one
        textUI.text = "";
        int stringLength = currentMessageList[messageIndexCounter].Length;
        float textPlayDuration = stringLength / textShowSpeed;
        string completeText = currentMessageList[messageIndexCounter];
        currentTextDisplayTween = textUI.DOText(completeText, textPlayDuration)
            .SetEase(Ease.Linear)
            .OnStart(() =>
            {
                isTextShowing = true;
            })
            .OnComplete(() =>
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(textBubbleLayoutGroup.GetComponent<RectTransform>());
                isTextShowing = false;
            })
            .OnPause(() =>
            {
                // replace directly with complete text on kill
                textUI.text = completeText;
                LayoutRebuilder.ForceRebuildLayoutImmediate(textBubbleLayoutGroup.GetComponent<RectTransform>());
                isTextShowing = false;
            }).Play();

        // fade older bubbles
        if (currentDisplayBubbles.Count > 0)
        {
            GameObject lastNewAddedBubble = currentDisplayBubbles[^1];
            lastNewAddedBubble.GetComponent<Image>().DOFade(textFadeModifier, textFadeDuration);
            lastNewAddedBubble.GetComponentInChildren<TextMeshProUGUI>().DOFade(textFadeModifier, textFadeDuration);
        }

        currentDisplayBubbles.Add(newTextBubble);
        messageIndexCounter++;
    }

    private GameObject AddTextBubble()
    {
        GameObject newTextBubble = Instantiate(textBubblePrefab, textBubbleLayoutGroup.transform, false);
        newTextBubble.transform.localScale = Vector3.one;

        newTextBubble.transform.SetAsLastSibling();
        return newTextBubble;
    }

    private void RemoveTextBubble(GameObject textBubbleToRemove)
    {
        // TODO: remove effects
        Destroy(textBubbleToRemove);
    }

    /// <summary>
    /// called when click on the payphone
    /// </summary>
    private void ReplayLastMessageList()
    {
        // if there are messages played before
        if (lastMessageList == null)
        {
            Debug.LogError("No previous message found!");
            return;
        }

        print("Replaying last message");
        currentMessageList = lastMessageList;
        DisplayMessage();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (inTextDisplayMode) return;
        base.OnPointerDown(eventData);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (inTextDisplayMode) return;
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (inTextDisplayMode) return;
        base.OnPointerExit(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (inTextDisplayMode) return;
        base.OnPointerUp(eventData);
    }

    protected override void ClickableEvent() => ReplayLastMessageList();
}