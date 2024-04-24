using System;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Interaction.Clickable
{
    public abstract class ClickableBase : MonoBehaviour
    {
        [Header("Size Modifier")]
        [SerializeField] private float hoverSizeModifier = 1.1f;
        [SerializeField] private float pressSizeModifier = 0.8f;

        [Header("Sound")]
        [SerializeField] private List<AudioClip> hoverSounds = new();
        [SerializeField] private List<AudioClip> pressSounds = new();
        [SerializeField] private List<AudioClip> exitSounds = new();

        protected void OnEnable()
        {
            var col = gameObject.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
        }

        protected virtual void OnMouseEnter()
        {
            transform.localScale *= hoverSizeModifier;
            PlaySound(hoverSounds);
        }

        protected virtual void OnMouseDown()
        {
            transform.localScale *= pressSizeModifier;
            PlaySound(pressSounds);
            ClickableEvent();
        }

        protected virtual void OnMouseUp()
        {
            transform.localScale /= pressSizeModifier;
        }

        protected virtual void OnMouseExit()
        {
            transform.localScale /= hoverSizeModifier;
            PlaySound(exitSounds);
        }

        protected abstract void ClickableEvent();

        private void PlaySound(List<AudioClip> audioClips)
        {
            if (audioClips.Count > 0)
                GameManager.Instance.audioManager.PlaySound(audioClips[Random.Range(0, audioClips.Count)]);
        }
    }
}