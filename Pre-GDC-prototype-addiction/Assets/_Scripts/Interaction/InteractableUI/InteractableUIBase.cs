using System.Collections.Generic;
using _Scripts.Manager;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace _Scripts.Interaction.InteractableUI
{
    public abstract class InteractableUIBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Title("Size Modifier")]
        [SerializeField] private bool sizeFeedback = true;
        [SerializeField] private float hoverSizeModifier = 1.1f;
        [SerializeField] private float pressSizeModifier = 0.8f;
        private Vector3 originalLocalScale;

        [Title("Sound")]
        [SerializeField] protected List<AudioClip> hoverSounds = new();
        [SerializeField] protected List<AudioClip> pressSounds = new();
        [SerializeField] protected List<AudioClip> exitSounds = new();

        protected virtual void Start()
        {
            originalLocalScale = transform.localScale;
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

        protected abstract void ClickableEvent();

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
}