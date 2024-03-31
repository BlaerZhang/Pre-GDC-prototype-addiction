using System.Collections;
using System.Collections.Generic;
using Interaction;
using UnityEngine;

public class GeneratorManager : MonoBehaviour
{
    public List<ScratchCardGenerator> generators;

    void Start()
    {
        // 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateCard()
    {
        Instantiate(generators[(int)GameManager.Instance.lastPickTier], transform);
    }
}
