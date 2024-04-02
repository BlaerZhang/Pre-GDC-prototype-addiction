using UnityEngine;

namespace ScratchCardGeneration.LayoutConstructor
{
    public interface ICardLayoutConstructor
    {
        GameObject ConstructCardLayout(float totalPrize);
    }
}