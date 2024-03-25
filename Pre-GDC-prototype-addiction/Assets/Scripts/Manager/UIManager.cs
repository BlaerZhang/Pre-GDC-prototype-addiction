using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Game")]
    public TextMeshProUGUI playerResource;
    
    [Header("Incremental")]
    public TextMeshProUGUI upgradePrice;
    
    public static UIManager instance;
    
    private void Awake()
    {
        instance = this;
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateResource(string resource)
    {
        playerResource.text = resource;
    }

    public void UpdateUpgradePrice(string price)
    {
        upgradePrice.text = price;
    }
    
}
