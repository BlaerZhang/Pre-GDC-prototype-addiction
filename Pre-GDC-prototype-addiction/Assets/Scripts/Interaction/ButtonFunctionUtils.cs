using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonFunctionUtils : MonoBehaviour
{
    public void ChangeScene(string toScene)
    {
        GameManager.Instance.switchSceneManager.ChangeScene(toScene);
    }
    
}
