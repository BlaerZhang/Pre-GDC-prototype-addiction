using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public TextMeshPro timeText;
    public 
    
    void Start()
    {
        
    }
    
    void Update()
    {
        timeText.text = System.DateTime.Now.ToLongTimeString();
    }
}
