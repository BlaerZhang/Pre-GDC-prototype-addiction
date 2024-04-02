using Interaction;
using ScratchCardGeneration.LayoutConstructor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public UIManager uiManager;

        public SwitchSceneManager switchSceneManager;

        public ScratchCardGenerator scratchCardGenerator;

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
            switchSceneManager = GetComponent<SwitchSceneManager>();
            scratchCardGenerator = GetComponent<ScratchCardGenerator>();
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
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
