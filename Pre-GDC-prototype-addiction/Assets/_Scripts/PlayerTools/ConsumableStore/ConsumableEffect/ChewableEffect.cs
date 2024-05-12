using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.ConsumableStore.ConsumableEffect
{
    public class ChewableEffect : MonoBehaviour, IConsumableEffect
    {
        public static Action onDrugEffectStop;

        [SerializeField] private GameObject overlayCanvasObject;
        [SerializeField] private List<GameObject> chewableResiduePrefabs;
        [SerializeField] private float effectDuration = 5f;

        public void Trigger()
        {

        }
    }
}