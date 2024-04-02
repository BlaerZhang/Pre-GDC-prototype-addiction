using UnityEngine;

namespace ScratchCardGeneration.LayoutConstructor
{
    public interface ICardLayoutConstructor
    {
        GameObject ConstructCardLayout(float totalPrize, Vector3 generatePos);
    }
}