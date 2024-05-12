using System.Collections;
using System.Collections.Generic;
using Abu;
using DG.Tweening;
using UnityEngine;

public class HighlightManager : MonoBehaviour
{
    private static TutorialFadeImage _tutorialFadeImage;
    public static float targetFadeImageSmoothness = 0.01f;
  
    void Start()
    {
        _tutorialFadeImage = GetComponent<TutorialFadeImage>();
        _tutorialFadeImage.Smoothness = 1;
        _tutorialFadeImage.enabled = false;
    }
    

    public static void SmoothEnableHighlight(float transitionDuration)
    {
        DOTween.To(() => _tutorialFadeImage.Smoothness, x => _tutorialFadeImage.Smoothness = x,
                targetFadeImageSmoothness, transitionDuration)
            .OnStart(() => _tutorialFadeImage.enabled = true);
    }
    
    public static void SmoothDisableHighlight(float transitionDuration)
    {
        DOTween.To(() => _tutorialFadeImage.Smoothness, x => _tutorialFadeImage.Smoothness = x,
                1, transitionDuration)
            .OnComplete(() => _tutorialFadeImage.enabled = false);
    }
}
