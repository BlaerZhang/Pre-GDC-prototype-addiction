using System;
using System.Collections.Generic;
using _Scripts.ConsumableStore.ConsumableEffect;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.PlayerTools.ConsumableStore.ConsumableEffect
{
    public class ChewableEffect : MonoBehaviour, IConsumableEffect
    {
        [SerializeField] private GameObject overlayCanvasObject;
        [SerializeField] private GameObject chewableResiduePrefabs;
        [SerializeField] private float screenBorderOffset;

        private void Start()
        {
            if (!chewableResiduePrefabs.TryGetComponent(out ChewableResidue chewableResidue))
                chewableResiduePrefabs.AddComponent<ChewableResidue>();
        }

        public void Trigger()
        {
            print("chewable effect triggered");
            GameObject residue = Instantiate(chewableResiduePrefabs, overlayCanvasObject.transform);
            RectTransform residueRectTransform = residue.GetComponent<RectTransform>();
            residueRectTransform.SetAsFirstSibling();

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            float randomX = Random.Range(-screenWidth / 2 + screenBorderOffset, screenWidth / 2 - screenBorderOffset);
            float randomY = Random.Range(-screenHeight / 2 + screenBorderOffset, screenHeight / 2 - screenBorderOffset);

            residueRectTransform.anchoredPosition = new Vector2(randomX, randomY);
            residueRectTransform.eulerAngles = new Vector3(0, 0, Random.Range(0, 360));
        }
    }
}