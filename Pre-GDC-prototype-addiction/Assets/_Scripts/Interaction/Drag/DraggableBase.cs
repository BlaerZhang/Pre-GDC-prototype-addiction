using System;
using System.Collections.Generic;
using DG.Tweening;
using Manager;
using ScratchCardGeneration;
using ScratchCardGeneration.PrizeGenerator;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Interaction
{
    public class DraggableBase : MonoBehaviour
    {
        [Header("Feedback")] 
        public bool pickAudio = true;
        public List<AudioClip> pickSounds;
        public bool dropAudio = true;
        public List<AudioClip> dropSounds;
        public bool dropParticle = true;
        public List<ParticleSystem> dropParticles;

        protected virtual void OnMouseEnter()
        {
            //TODO
        }

        protected virtual void OnMouseDown()
        {
            if (pickAudio && pickSounds.Count > 0)
                GameManager.Instance.audioManager.PlaySound(pickSounds[Random.Range(0, pickSounds.Count)]);
        }

        protected virtual void OnMouseDrag()
        {
            
        }
        protected virtual void OnMouseUp()
        {
            if (dropAudio && dropSounds.Count > 0)
                GameManager.Instance.audioManager.PlaySound(dropSounds[Random.Range(0, dropSounds.Count)]);
        }

        protected virtual void OnMouseExit()
        {
            
        }
    }
}
