using System;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.EventSystems;

namespace Interaction.Clickable
{
    public abstract class ClickableUIBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Size Modifier")]
        [SerializeField] private float hoverSizeModifier = 1.1f;
        [SerializeField] private float pressSizeModifier = 0.8f;

        [Header("Sound")]
        [SerializeField] private List<AudioClip> hoverSounds = new();
        [SerializeField] private List<AudioClip> pressSounds = new();
        [SerializeField] private List<AudioClip> exitSounds = new();

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            ScaleUpClickable(hoverSizeModifier);
            PlaySound(hoverSounds);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            ScaleDownClickable(hoverSizeModifier);
            PlaySound(exitSounds);
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            ScaleUpClickable(pressSizeModifier);
            PlaySound(pressSounds);
            ClickableEvent();
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            ScaleDownClickable(pressSizeModifier);
        }

        protected abstract void ClickableEvent();

        private void PlaySound(List<AudioClip> audioClips)
        {
            if (audioClips.Count > 0)
                GameManager.Instance.audioManager.PlaySound(audioClips[Random.Range(0, audioClips.Count)]);
        }

        private void ScaleUpClickable(float modifier)
        {
            transform.localScale *= modifier;
        }

        private void ScaleDownClickable(float modifier)
        {
            transform.localScale /= modifier;
        }
    }
}