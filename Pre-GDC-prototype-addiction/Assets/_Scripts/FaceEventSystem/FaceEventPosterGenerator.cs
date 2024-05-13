using System;
using System.Collections.Generic;
using _Scripts.Interaction.InteractableSprite;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.FaceEventSystem
{
    public class FaceEventPosterGenerator : SerializedMonoBehaviour
    {
        [SerializeField] private GameObject backgroundCanvasObject;
        [SerializeField] private float screenBorderOffset;
        [SerializeField] private Dictionary<FaceEventType, GameObject> faceEventPosterPrefabDict = new();

        private void OnEnable()
        {
            SelectableScratchCard.onFaceEventHappened += GenerateFaceEventPoster;
        }

        private void OnDisable()
        {
            SelectableScratchCard.onFaceEventHappened -= GenerateFaceEventPoster;
        }

        private void GenerateFaceEventPoster(FaceEventType faceEventType)
        {
            faceEventPosterPrefabDict.TryGetValue(faceEventType, out GameObject currentEventPosterPrefab);

            GameObject eventPoster = Instantiate(currentEventPosterPrefab, backgroundCanvasObject.transform);
            RectTransform eventPosterRectTransform = eventPoster.GetComponent<RectTransform>();

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            float randomX = Random.Range(-screenWidth / 2 + screenBorderOffset, screenWidth / 2 - screenBorderOffset);
            float randomY = Random.Range(-screenHeight / 2 + screenBorderOffset, screenHeight / 2 - screenBorderOffset);

            eventPosterRectTransform.anchoredPosition = new Vector2(randomX, randomY);
            eventPosterRectTransform.eulerAngles = new Vector3(0, 0, Random.Range(0, 360));
        }
    }
}