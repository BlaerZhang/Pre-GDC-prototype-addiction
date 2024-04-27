using System;
using UnityEngine;

namespace Manager
{
    public class ResourceManager : MonoBehaviour
    {
        private int playerGold = 0;
        private DateTime gameStartTime;
        private DateTime currentTime;
        public static Action<DateTime> OnTimeChanged;

        public int PlayerGold
        {
            get => playerGold;
            set
            {
                playerGold = value;
                GameManager.Instance.uiManager.UpdateResource(value);
                StatsTracker.onValueChanged?.Invoke(nameof(playerGold), playerGold);
            }
        }

        public DateTime CurrentTime
        {
            get => currentTime;
            set
            {
                currentTime = value;
                OnTimeChanged?.Invoke(value);
            }
        }

        public int initialGold = 0;

        public int ClickerLevel { get; set; } = 1;

        void Start()
        {
            PlayerGold = initialGold;
            gameStartTime = DateTime.Now;
            CurrentTime = gameStartTime;
        }

        public void ChangeTime(double minutes)
        {
            CurrentTime = CurrentTime.AddMinutes(minutes);
        }
    }
}
