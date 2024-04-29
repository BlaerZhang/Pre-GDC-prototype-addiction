using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

/// <summary>
/// TODO:
/// 3. 符合按下去之后的形态 (按下去之后会使周围的烟有位置上的改变， 也会根据周围环境
/// 4. 不能浮在空中
/// </summary>
public class CigaretteButtGenerator : MonoBehaviour
{
    public List<GameObject> cigarettePrefabs;
    public RectTransform ashTrayTransform;
    public float generationPositionXOffset;

    public float minRotationAngle;
    public float maxRotationAngle;
    
    public int maxLayer = 4;
    public int maxCigarettesPerLayer = 5;
    public float layerHeight;
    
    private int currentLayer = 0;
    private int currentLayerCigarettesCount = 0;

    private readonly List<GameObject> currentCigarettes = new();

    private List<int> availableSlots = new();
    private List<int> occupiedSlots = new();

    private float ashTrayWidth;
    private float ashTrayHeight;

    private float slotWidth;
    private float slotDegree;

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

    private void OnValidate()
    {
        slotWidth = ashTrayWidth / maxCigarettesPerLayer;
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
    
    // TODO: 1. randomly fill slots
    void SetCigarettePositionAndRotation(GameObject cigarette)
    {
        // get an available random slot 
        int randSlotIndex = availableSlots[Random.Range(0, availableSlots.Count)];
        occupiedSlots.Add(randSlotIndex);
        availableSlots.Remove(randSlotIndex);
        
        // calculate the slot bound
        float leftXBound = -ashTrayWidth / 2 + randSlotIndex * slotWidth;
        float rightXBound = leftXBound + slotWidth;
        
        // get a random position within the slot
        Vector3 localPosition = new Vector3(Random.Range(leftXBound, rightXBound),
            Random.Range(-ashTrayHeight / 2, ashTrayHeight / 2) + currentLayer * layerHeight,
            0);
        cigarette.transform.localPosition = localPosition;
        
        // TODO: set the rotation according to the current slot
        float minAngle = minRotationAngle + slotDegree * randSlotIndex;
        float maxAngle = minAngle + slotDegree;
        float angle = Random.Range(minAngle, maxAngle); 
        print("min " + minAngle);print("max " + maxAngle);
        print("angle " + angle);
        cigarette.transform.localRotation = Quaternion.Euler(0, 0, angle);
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
        
        GameObject cig = Instantiate(cigarettePrefabs[Random.Range(0, cigarettePrefabs.Count)], ashTrayTransform);
        cig.transform.SetAsFirstSibling();
        SetCigarettePositionAndRotation(cig);
        currentLayerCigarettesCount++;
        currentCigarettes.Add(cig);
    }

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