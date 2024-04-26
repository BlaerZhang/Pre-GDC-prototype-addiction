using UnityEngine;

namespace _Scripts.PlayerTools
{
    public class PlayerToolBase : MonoBehaviour, IUnlockable
    {
        [SerializeField] private int unlockPrice;
        [SerializeField] private int unlockMembershipLevel;

        public void UnlockItem(int currentLevel)
        {
            throw new System.NotImplementedException();
        }

        private void ShowUnlockEffect()
        {

        }
    }
}