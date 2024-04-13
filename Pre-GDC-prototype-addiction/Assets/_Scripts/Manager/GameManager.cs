using Interaction;
using ScratchCardGeneration.LayoutConstructor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [HideInInspector] public UIManager uiManager;

        [HideInInspector] public SwitchSceneManager switchSceneManager;

        [HideInInspector] public ScratchCardGenerator scratchCardGenerator;

        [HideInInspector] public AudioManager audioManager;
        
        [HideInInspector] public CardPoolManager cardPoolManager;

        [HideInInspector] public bool incrementalLock = true;

        [HideInInspector] public int lastPickPrice = 1;

        [HideInInspector] public ScratchCardTier lastPickTier = ScratchCardTier.Level1;

        [HideInInspector] public float lastMenuYPos = 0;
    
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
            switchSceneManager = GetComponent<SwitchSceneManager>();
            scratchCardGenerator = GetComponent<ScratchCardGenerator>();
            audioManager = GetComponent<AudioManager>();
            cardPoolManager = GetComponent<CardPoolManager>();
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
