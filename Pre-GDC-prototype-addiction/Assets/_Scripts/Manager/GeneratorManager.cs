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
            // 
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void GenerateCard()
        {
            Instantiate(generators[(int)GameManager.Instance.lastPickTier - 1], transform);
        }
    }
}
