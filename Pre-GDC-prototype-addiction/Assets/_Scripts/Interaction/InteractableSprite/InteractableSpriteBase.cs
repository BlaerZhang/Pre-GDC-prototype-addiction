using System;
using System.Collections.Generic;
using DG.Tweening;
using Manager;
using ScratchCardGeneration;
using ScratchCardGeneration.PrizeGenerator;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Interaction
{
    public class InteractableSpriteBase : MonoBehaviour
    {
        [Title("Feedback")]
        public bool pickAudio = true;
        public List<AudioClip> pickSounds;
        public bool dropAudio = true;
        public List<AudioClip> dropSounds;
        public bool dropParticle = true;
        public List<ParticleSystem> dropParticles;

        protected virtual void OnMouseEnter()
        {
            //TODO
            GameManager.Instance.cursorManager.SetCursor(CursorManager.CursorType.DragAreaHover);
        }

        protected virtual void OnMouseDown()
        {
            if (pickAudio && pickSounds.Count > 0)
                GameManager.Instance.audioManager.PlaySound(pickSounds[Random.Range(0, pickSounds.Count)]);
        }

        protected virtual void OnMouseDrag()
        {
            GameManager.Instance.cursorManager.SetCursor(CursorManager.CursorType.Dragging);
        }

        protected virtual void OnMouseUp()
        {
            GameManager.Instance.cursorManager.SetCursor(CursorManager.CursorType.DragAreaHover);
            if (dropAudio && dropSounds.Count > 0)
                GameManager.Instance.audioManager.PlaySound(dropSounds[Random.Range(0, dropSounds.Count)]);
        }

        protected virtual void OnMouseExit()
        {
            GameManager.Instance.cursorManager.SetCursor(CursorManager.CursorType.Idle);
        }
    }
}
