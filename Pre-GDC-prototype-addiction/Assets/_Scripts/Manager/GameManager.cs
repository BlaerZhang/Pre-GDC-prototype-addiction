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
        [HideInInspector] public ResourceManager resourceManager;

        public bool incrementalLock = true;
        public int lastPickPrice = 1;
        public MenuDraggable.Tier lastPickTier = MenuDraggable.Tier.Level1;
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
