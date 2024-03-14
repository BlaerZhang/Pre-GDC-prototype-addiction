using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;
using ScratchCardAsset;

public class ObiRopeSpawn : MonoBehaviour
{
    public ObiSolver solver;
    public GameObject obiRopePrefab;
    public Transform scratcher;
    
    public int particlePoolSize = 100;

    private ObiRope rope;
    private ObiRopeBlueprint blueprint;
    private ObiRopeExtrudedRenderer ropeRenderer;

    private ObiRopeCursor cursor;
    
    public EraseProgress eraseProgress;
    private float currentErasePercentage = 0;
    private GameObject currentRope = null;
    
    void Awake()
    {
        
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            currentErasePercentage = eraseProgress.GetProgress();
            CreateRope();
        }
        
        if (Input.GetMouseButton(0))
        {
            if (!currentRope) return;
            currentRope.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            cursor.ChangeLength((eraseProgress.GetProgress() - currentErasePercentage) * 30f);
            // print(Mathf.RoundToInt(1 + 100f * eraseProgress.GetProgress() - currentErasePercentage));
        }

        if (Input.GetMouseButtonUp(0))
        {
            currentRope.GetComponent<ObiParticleAttachment>().enabled = false;
            Invoke("Destroy", 5f);
        }   
    }

    void Destroy()
    {
        
    }

    void CreateRope()
    {
        // Create both the rope and the solver:	
        GameObject ropeInstance = Instantiate(obiRopePrefab, (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
        ropeInstance.transform.parent = solver.transform;
        currentRope = ropeInstance;
        
        rope = ropeInstance.GetComponent<ObiRope>();
        ropeRenderer = ropeInstance.GetComponent<ObiRopeExtrudedRenderer>();
        
        cursor = rope.gameObject.GetComponent<ObiRopeCursor>();
    }
}
