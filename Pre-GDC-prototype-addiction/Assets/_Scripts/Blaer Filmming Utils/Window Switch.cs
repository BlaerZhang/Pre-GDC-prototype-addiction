using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

public class WindowSwitch : MonoBehaviour
{
    private Animator windowAnimator;
    void Start()
    {
        windowAnimator = GetComponent<Animator>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            windowAnimator.SetTrigger("OnWindowChange");
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.resourceManager.PlayerGold += Random.Range(1, 50) * 100;
        }
    }
}
