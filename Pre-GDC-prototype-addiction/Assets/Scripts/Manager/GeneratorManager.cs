using System.Collections.Generic;
using ScratchCardGeneration.LayoutConstructor;
using UnityEngine;

namespace Manager
{
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
}
