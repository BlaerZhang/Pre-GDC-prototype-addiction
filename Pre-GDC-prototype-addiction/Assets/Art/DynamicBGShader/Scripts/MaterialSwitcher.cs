using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTNGames
{
    public class MaterialSwitcher : MonoBehaviour
    {
        public Material[] materials;
        
        //Change skybox material
        public void NextMaterial()
        {
            int index = System.Array.IndexOf(materials, RenderSettings.skybox);
            index = (index + 1) % materials.Length;
            RenderSettings.skybox = materials[index];
        }

        //Change skybox material
        public void PreviousMaterial()
        {
            int index = System.Array.IndexOf(materials, RenderSettings.skybox);
            index = ((index - 1) + materials.Length) % materials.Length;
            RenderSettings.skybox = materials[index];
        }

    }
}


