using Manager;
using UnityEngine;

namespace Interaction
{
    public class ButtonFunctionUtils : MonoBehaviour
    {
        public void ChangeScene(string toScene)
        {
            GameManager.Instance.switchSceneManager.ChangeScene(toScene);
        }
    
    }
}
