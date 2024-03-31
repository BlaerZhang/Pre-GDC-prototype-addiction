using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subdetector : MonoBehaviour
{
    private DetectScratchArea detectScratchArea;
    
    void Start()
    {
        detectScratchArea = GetComponentInParent<DetectScratchArea>();
    }

    private void OnMouseOver()
    {
        detectScratchArea.isOverScratchArea = true;
    }

    private void OnMouseExit()
    {
        detectScratchArea.isOverScratchArea = false;
    }
}
