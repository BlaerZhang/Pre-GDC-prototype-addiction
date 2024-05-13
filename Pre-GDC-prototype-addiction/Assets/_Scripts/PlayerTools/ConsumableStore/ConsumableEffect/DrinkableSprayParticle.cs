using System;
using _Scripts.Manager;
using UnityEngine;

namespace _Scripts.ConsumableStore.ConsumableEffect
{
    public class DrinkableSprayParticle : MonoBehaviour
    {
        public static Action onWaterSprayParticleDisappear;

        public AudioClip drinkingSound;

        private GameObject currentTemptSound;

        private void OnBecameInvisible()
        {
            onWaterSprayParticleDisappear?.Invoke();
        }

        private void OnMouseEnter()
        {
            print("start drinking");
            currentTemptSound = GameManager.Instance.audioManager.PlayLoopSound(drinkingSound);
            GameManager.Instance.cursorManager.SetCursor(CursorManager.CursorType.Drinking);
        }

        private void OnMouseExit()
        {
            print("stop drinking");
            GameManager.Instance.audioManager.StopLoopSound(currentTemptSound);
            GameManager.Instance.cursorManager.SetCursor(CursorManager.CursorType.Idle);
        }
    }
}