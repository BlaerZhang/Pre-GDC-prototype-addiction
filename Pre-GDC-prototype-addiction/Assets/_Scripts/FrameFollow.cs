using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameFollow : MonoBehaviour
{
    public Transform menuHolder;
    
    void Update()
    {
        transform.position = menuHolder.position;
    }
}
