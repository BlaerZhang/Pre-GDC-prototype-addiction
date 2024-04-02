using UnityEngine;

namespace Interaction
{
    public class Subdetector : MonoBehaviour
    {
        private DetectScratchArea detectScratchArea;
    
        void Start()
        {
            detectScratchArea = GetComponentInParent<DetectScratchArea>();
        }

        private void OnMouseOver()
        {
            detectScratchArea.isOverScratchArea = true;
        }

        private void OnMouseExit()
        {
            detectScratchArea.isOverScratchArea = false;
        }
    }
}
