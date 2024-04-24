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

        [HideInInspector] public UIManager uiManager;

        [HideInInspector] public SwitchSceneManager switchSceneManager;

        [HideInInspector] public ScratchCardGenerator scratchCardGenerator;

        [HideInInspector] public AudioManager audioManager;

        [HideInInspector] public ResourceManager resourceManager;

        [HideInInspector] public CardPoolManager cardPoolManager;
        
        [HideInInspector] public FaceEventManager faceEventManager;

        [HideInInspector] public StatsTrackingManager statsTrackingManager;

        [HideInInspector] public bool incrementalLock = true;

        [HideInInspector] public int lastPickPrice = 1;

        [HideInInspector] public ScratchCardTier lastPickTier = ScratchCardTier.Level1;

        public float lastMenuYPos = 0;
    
        [HideInInspector] public float totalCostBeforeWinning = 0;

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
            faceEventManager = GetComponentInChildren<FaceEventManager>();
            statsTrackingManager = GetComponentInChildren<StatsTrackingManager>();
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
