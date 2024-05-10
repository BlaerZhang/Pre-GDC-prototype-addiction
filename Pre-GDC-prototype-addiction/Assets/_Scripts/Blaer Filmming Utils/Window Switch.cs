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

        if (Input.GetKeyDown(KeyCode.T))
        {
            GameManager.Instance.resourceManager.CurrentTime = GameManager.Instance.resourceManager.CurrentTime.AddMinutes(15);
        }
        
        // GameManager.Instance.resourceManager.CurrentTime = GameManager.Instance.resourceManager.CurrentTime.AddMinutes(1);
        // GameManager.Instance.resourceManager.CurrentTime = GameManager.Instance.resourceManager.CurrentTime.AddHours(1);
        
        if (Input.GetKeyDown(KeyCode.D))
        {
            GameManager.Instance.resourceManager.CurrentTime = GameManager.Instance.resourceManager.CurrentTime.AddMinutes(15);
            GameManager.Instance.resourceManager.PlayerGold += Random.Range(1, 50) * 100;
            windowAnimator.SetTrigger("OnWindowChange");
            StatsTrackingManager.OnPricePrizeHistoryUpdated.Invoke(3, Random.Range(0, 6));
        }
    }
}
