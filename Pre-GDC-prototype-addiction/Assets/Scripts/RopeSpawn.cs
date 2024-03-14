using System;
using System.Collections;
using System.Collections.Generic;
using ScratchCardAsset;
using UnityEngine;

public class RopeSpawn : MonoBehaviour
{

    public GameObject ropePrefab;
    private GameObject currentRope = null;
    private float currentErasePercentage = 0;

    public EraseProgress eraseProgress;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            currentErasePercentage = eraseProgress.GetProgress() * 100f;
            currentRope = Instantiate(ropePrefab, Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
        }
        
        if (Input.GetMouseButton(0))
        {
            if (!currentRope) return;
            currentRope.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentRope.GetComponent<Rope>().segmentLength = Mathf.RoundToInt(1 + 100f * eraseProgress.GetProgress() - currentErasePercentage);
            print(Mathf.RoundToInt(1 + 100f * eraseProgress.GetProgress() - currentErasePercentage));
        }

        if (Input.GetMouseButtonUp(0))
        {
            currentRope.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            Invoke("Destroy", 5f);
        }
    }

    private void Destroy()
    {
        
    }
}
