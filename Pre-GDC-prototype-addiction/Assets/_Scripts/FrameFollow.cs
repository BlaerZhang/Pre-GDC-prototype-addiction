using UnityEngine;

namespace _Scripts
{
    public class FrameFollow : MonoBehaviour
    {
        public Transform menuHolder;
    
        void Update()
        {
            transform.position = menuHolder.position;
        }
    }
}
