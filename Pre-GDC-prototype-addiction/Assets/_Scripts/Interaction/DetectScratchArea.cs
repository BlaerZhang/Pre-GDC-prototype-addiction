using UnityEngine;

namespace Interaction
{
    public class DetectScratchArea : MonoBehaviour
    {
        public bool isOverCard = false;
        public bool isOverScratchArea = false;

        private Subdetector subdetector;

        void Start()
        {
            subdetector = GetComponentInChildren<Subdetector>();
        }

        private void OnMouseOver()
        {
            isOverCard = true;
        }

        private void OnMouseExit()
        {
            isOverCard = false;
        }
    }
}
