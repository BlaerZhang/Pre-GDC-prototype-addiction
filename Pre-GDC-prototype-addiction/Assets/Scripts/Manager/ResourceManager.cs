using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private int playerGold = 0;
    private int clickerLevel = 1;

    public int PlayerGold
    {
        get { return playerGold; }
        set
        {
            playerGold = value;
            UIManager.instance.UpdateResource($"Gold: {value}");
        }
    }

    public int ClickerLevel
    {
        get { return clickerLevel; }
        set
        {
            clickerLevel = value;
        }
    }
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
