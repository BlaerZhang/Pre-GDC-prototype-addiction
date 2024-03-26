using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private int playerGold = 0;

    public int PlayerGold
    {
        get { return playerGold; }
        set
        {
            playerGold = value;
            UIManager.instance.UpdateResource($"Gold: {value}");
        }
    }
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
