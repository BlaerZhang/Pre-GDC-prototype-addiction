using System;
using UnityEngine;

namespace Interaction
{
    public class PrizeRevealing : MonoBehaviour
    {
        public float prize;

        public static Action<float> onPrizeRevealed;

        private void OnMouseDown()
        {
            // if the scratch field is scratched off
            print("rolling number!");

            onPrizeRevealed(prize);
        }
    }
}
