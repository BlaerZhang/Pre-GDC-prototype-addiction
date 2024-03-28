using System;
using System.Collections;
using System.Collections.Generic;
using Interaction;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public UIManager uiManager;

    public int lastPickPrice = 1;

    public MenuDraggable.Tier lastPickTier = MenuDraggable.Tier.Level1;
    
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
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RestartGame();

        if (Input.GetKeyDown(KeyCode.UpArrow)) SceneManager.LoadScene("Menu");
        if (Input.GetKeyDown(KeyCode.LeftArrow)) SceneManager.LoadScene("Incremental");
        if (Input.GetKeyDown(KeyCode.RightArrow)) SceneManager.LoadScene("Buy Card");
        if (Input.GetKeyDown(KeyCode.DownArrow)) SceneManager.LoadScene("?");
    }

    private void RestartGame()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
