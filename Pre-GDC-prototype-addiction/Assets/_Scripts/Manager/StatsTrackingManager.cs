using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Manager
{
    public class StatsTrackingManager : MonoBehaviour
    {
        public static Action<int,int> OnPricePrizeHistoryUpdated;
        public List<Vector2> pricePrizeHistory;

        void Start()
        {
            pricePrizeHistory = new List<Vector2>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void UpdatePricePrizeHistory(int price, int prize)
        {
            pricePrizeHistory.Add(new Vector2(price, prize));
            OnPricePrizeHistoryUpdated?.Invoke(price, prize);
        }
    }
}
