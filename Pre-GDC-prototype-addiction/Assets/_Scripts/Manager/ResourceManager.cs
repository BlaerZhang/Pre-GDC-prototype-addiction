using UnityEngine;

namespace Manager
{
    public class ResourceManager : MonoBehaviour
    {
        private int playerGold = 0;
        private int clickerLevel = 1;

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

        public int initialGold = 0;

        public int ClickerLevel
        {
            get => clickerLevel;
            set => clickerLevel = value;
        }
    
        void Start()
        {
            PlayerGold = initialGold;
        }
    }
}
