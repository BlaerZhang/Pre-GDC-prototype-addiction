using System;
using UnityEngine;
using System.Collections.Generic;
using _Scripts.ConsumableStore.ConsumableEffect;
using DG.Tweening;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;

// TODO: drop after certain layers ?
public class CigaretteButtGenerator : MonoBehaviour
{

    [SerializeField] private List<GameObject> cigarettePrefabs;
    [SerializeField] private RectTransform ashTrayTransform;
    [SerializeField] private RectTransform cigaretteItemOnTableTransform;

    [Title("Position And Rotation Settings")]
    [SerializeField] private float generationPositionXOffset;

    [SerializeField] private float minRotationAngle;
    [SerializeField] private float maxRotationAngle;

    [Title("Layer Settings")]
    [SerializeField] private int maxLayer = 4;
    [SerializeField] private int maxCigarettesPerLayer = 5;
    [SerializeField] private float layerHeight;
    
    private int currentLayer = 0;
    private int currentLayerCigarettesCount = 0;

    private readonly List<GameObject> currentCigarettes = new();

    private List<int> availableSlots = new();
    private List<int> occupiedSlots = new();

    private float ashTrayWidth;
    private float ashTrayHeight;

    private float slotWidth;
    private float slotDegree;

    [Title("Animation Settings")]
    [SerializeField] private float animationStartDistance = 10f;
    [SerializeField] private float animationDuration = 1f;


    private void Start()
    {
        ashTrayWidth = ashTrayTransform.rect.width - generationPositionXOffset * 2;
        ashTrayHeight = ashTrayTransform.rect.height;

        slotWidth = ashTrayWidth / maxCigarettesPerLayer;
        slotDegree = (maxRotationAngle - minRotationAngle) / maxCigarettesPerLayer;
        
        InitializeSlotAvailability();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D)) AddCigarettes();
        if (Input.GetKeyDown(KeyCode.A)) RemoveCigarettes();
        
        if (Input.GetKey(KeyCode.W)) AddCigarettes();
        if (Input.GetKey(KeyCode.S)) RemoveCigarettes();
    }

    private void OnEnable()
    {
        PuffableEffect.onStopSmoking += AddCigarettes;
    }

    private void OnDisable()
    {
        PuffableEffect.onStopSmoking -= AddCigarettes;
    }

    private void InitializeSlotAvailability()
    {
        occupiedSlots?.Clear();
        availableSlots?.Clear();
        for (int i = 0; i < maxCigarettesPerLayer; i++)
        {
            availableSlots?.Add(i);
        }
    }

    void SetCigarettePositionAndRotation(GameObject cigarette)
    {
        // get an available random slot
        int randSlotIndex = availableSlots[Random.Range(0, availableSlots.Count)];
        occupiedSlots.Add(randSlotIndex);
        availableSlots.Remove(randSlotIndex);

        Vector2 finalPosition = GetCigarettePosition(randSlotIndex);
        float angle = GetCigaretteRotation(randSlotIndex);

        float startPositionXLength = Mathf.Sin(Mathf.Abs(angle) * Mathf.Deg2Rad) * animationStartDistance;
        if (angle > 0) startPositionXLength = -startPositionXLength;

        Vector2 startPosition = new Vector2(startPositionXLength, animationStartDistance) + finalPosition;

        // cigarette.transform.localPosition = startPosition;
        // cigarette.transform.localRotation = Quaternion.Euler(0, 0, angle);

        cigarette.transform.position = cigaretteItemOnTableTransform.position;
        cigarette.transform.rotation = cigaretteItemOnTableTransform.rotation;
        
        Sequence putCigaretteInAshtraySequence = DOTween.Sequence();
        putCigaretteInAshtraySequence
            .Append(cigarette.transform.DOLocalMove(startPosition, animationDuration / 2))
            .Insert(0, cigarette.transform.DORotate(new Vector3(0, 0, angle), animationDuration / 2))
            .OnComplete(() =>
            {
                cigarette.transform.SetParent(ashTrayTransform);
                cigarette.transform.SetAsFirstSibling();
                cigarette.transform.DOLocalMove(finalPosition, animationDuration / 2).SetEase(Ease.InExpo);
            })
            .Play();
    }

    Vector2 GetCigarettePosition(int randSlotIndex)
    {
        // calculate the slot bound
        float leftXBound = -ashTrayWidth / 2 + randSlotIndex * slotWidth;
        float rightXBound = leftXBound + slotWidth;

        // get a random position within the slot
        Vector3 localPosition = new Vector3(Random.Range(leftXBound, rightXBound),
            Random.Range(-ashTrayHeight / 2, ashTrayHeight / 2) + currentLayer * layerHeight,
            0);

        return localPosition;
    }

    float GetCigaretteRotation(int randSlotIndex)
    {
        float minAngle = minRotationAngle + slotDegree * randSlotIndex;
        float maxAngle = minAngle + slotDegree;
        float angle = Random.Range(minAngle, maxAngle);

        return angle;
    }

    void AddCigarettes()
    {
        if (currentLayerCigarettesCount >= maxCigarettesPerLayer)
        {
            if (currentLayer < maxLayer)
            {
                currentLayer++;
                currentLayerCigarettesCount = 0;
                InitializeSlotAvailability();
            }
            else
            {
                return;
            }
        }
        
        GameObject cig = Instantiate(cigarettePrefabs[Random.Range(0, cigarettePrefabs.Count)], this.transform);
        cig.transform.SetAsLastSibling();
        SetCigarettePositionAndRotation(cig);
        currentLayerCigarettesCount++;
        currentCigarettes.Add(cig);
    }

    // TODO: remove automatically / or remove when the player click on the pile
    void RemoveCigarettes()
    {
        if (currentLayerCigarettesCount <= 0)
        {
            if (currentLayer > 0)
            {
                currentLayer--;
                currentLayerCigarettesCount = maxCigarettesPerLayer;
            }
            else
            {
                return;
            }
        }
        
        if (occupiedSlots.Count > 0)
        {
            availableSlots.Add(occupiedSlots[^1]);
            occupiedSlots.Remove(occupiedSlots[^1]);
        }
        
        GameObject cig = currentCigarettes[^1];
        currentCigarettes.RemoveAt(currentCigarettes.Count - 1);
        currentLayerCigarettesCount--;
        Destroy(cig);
    }
}