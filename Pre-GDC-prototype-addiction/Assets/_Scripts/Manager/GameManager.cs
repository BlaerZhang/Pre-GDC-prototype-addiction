using Interaction;
using ScratchCardGeneration.LayoutConstructor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public UIManager uiManager;

        public SwitchSceneManager switchSceneManager;

        public ScratchCardGenerator scratchCardGenerator;

        public AudioManager audioManager;

        public ResourceManager resourceManager;

        public CardPoolManager cardPoolManager;

        public bool incrementalLock = true;

        public int lastPickPrice = 1;

        public ScratchCardTier lastPickTier = ScratchCardTier.Level1;

        public float lastMenuYPos = 0;
    
        public float totalCostBeforeWinning = 0;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            uiManager = GetComponentInChildren<UIManager>();
            switchSceneManager = GetComponentInChildren<SwitchSceneManager>();
            scratchCardGenerator = GetComponentInChildren<ScratchCardGenerator>();
            audioManager = GetComponentInChildren<AudioManager>();
            resourceManager = GetComponentInChildren<ResourceManager>();
            cardPoolManager = GetComponentInChildren<CardPoolManager>();
        }

        void Update()
        {
            RestartGame();
        }

        private void RestartGame()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}
