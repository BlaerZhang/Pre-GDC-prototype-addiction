using System;
using _Scripts.Manager;
using DG.Tweening;
using TMPro;
using UnityEngine;
using ResourceManager = _Scripts.Manager.ResourceManager;

namespace _Scripts.Interaction.Beta
{
    public class Clock : MonoBehaviour
    {
        public TextMeshProUGUI hourText;
        public TextMeshProUGUI colun;
        public TextMeshProUGUI minuteText;
        private DateTime displayTime;
        private int displayHour;
        private int displayMinute;

        private void OnEnable()
        {
            ResourceManager.OnTimeChanged += UpdateClock;
        }

        private void OnDisable()
        {
            ResourceManager.OnTimeChanged -= UpdateClock;
        }

        private void Start()
        {
            displayTime = GameManager.Instance.resourceManager.CurrentTime;
            displayHour = displayTime.Hour;
            displayMinute = displayTime.Minute;
        }

        private void UpdateClock(DateTime newTime)
        {
            //update time
            displayTime = newTime;
        
            //minute animation
            DOVirtual.Int(displayMinute, newTime.Minute, 1, value =>
            {
                minuteText.text = value.ToString("D2");
            }).SetEase(Ease.Linear).OnComplete(() => { displayMinute = newTime.Minute; });

            //hour animation
            DOVirtual.Int(displayHour, newTime.Hour, 1, value =>
            {
                hourText.text = value.ToString("D2");
            }).SetEase(Ease.Linear).OnComplete(() => { displayHour = newTime.Hour; });
        
            //Colun Feedback
            colun.DOColor(Color.red, 0.5f).SetEase(Ease.Flash, 4, 0);
        }
    }
}
