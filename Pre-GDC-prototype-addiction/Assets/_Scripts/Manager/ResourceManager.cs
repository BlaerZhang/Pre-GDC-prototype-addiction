using System;
using _Scripts.Interaction.InteractableSprite;
using _Scripts.MetaphysicsSystem;
using UnityEngine;

namespace _Scripts.Manager
{
    public class ResourceManager : MonoBehaviour
    {
        private int playerGold = 0;

        private DateTime gameStartTime;
        private DateTime currentTime;
        public static Action<DateTime> OnTimeChanged;
        public int minutesPassed = 0;

        public bool isTimeStopped = false;

        public int PlayerGold
        {
            get => playerGold;
            set
            {
                playerGold = value;
                GameManager.Instance.uiManager.UpdateGold(value);
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

        private void OnEnable()
        {
            ScratchCardPoster.onTryBuyPoster += (poster, isBought) =>
            {
                if (isBought) PlayerGold -= poster.price;
                if (isBought) ChangeTime(3);
            };
        }

        private void OnDisable()
        {
            ScratchCardPoster.onTryBuyPoster -= (poster, isBought) =>
            {
                if (isBought) PlayerGold -= poster.price;
                if (isBought) ChangeTime(3);
            };
        }

        public void ChangeTime(double minutes)
        {
            if (isTimeStopped) return;
            minutesPassed += (int)minutes;
            CurrentTime = CurrentTime.AddMinutes(minutes);
        }
    }
}
