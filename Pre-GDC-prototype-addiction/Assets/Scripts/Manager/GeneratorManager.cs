using System.Collections;
using System.Collections.Generic;
using Interaction;
using UnityEngine;

public class GeneratorManager : MonoBehaviour
{
    public List<ScratchCardGenerator> generators;

    void Start()
    {
        Instantiate(generators[(int)GameManager.Instance.lastPickTier]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
