using _Scripts.Manager;
using UnityEngine;

namespace _Scripts.Interaction
{
    public class ButtonFunctionUtils : MonoBehaviour
    {
        public void ChangeScene(string toScene)
        {
            GameManager.Instance.switchSceneManager.ChangeScene(toScene);
        }

        public void ClearFaceEvent()
        {
            GameManager.Instance.faceEventManager.ClearDuration();
        }
    }
}
