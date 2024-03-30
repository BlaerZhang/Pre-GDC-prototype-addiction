using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

public class SwitchSceneManager : SerializedMonoBehaviour
{
    [HideInInspector] public Action<string> onSceneChange;
    public Dictionary<string, CinemachineVirtualCamera> sceneName_vCamDict;
    public bool isChangingScene = false;
    private string currentActiveScene;
    
    // Start is called before the first frame update
    void Start()
    {
        isChangingScene = false;
        currentActiveScene = SceneManager.GetActiveScene().name;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeScene("Incremental");
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeScene("Menu");
        }
    }

    public void ChangeScene(string toScene)
    {
        //set bool lock
        if (isChangingScene) return;
        isChangingScene = true;
        
        //adjust cam priority
        foreach (var scene_vCam in sceneName_vCamDict)
        {
            scene_vCam.Value.Priority = 0;
        }
        sceneName_vCamDict[toScene].Priority = 1;
        
        //additive load scene
        SceneManager.LoadScene(toScene, LoadSceneMode.Additive);
        
        //unload scene
        StartCoroutine(UnloadScene(currentActiveScene, toScene));
    }

    IEnumerator UnloadScene(string fromScene, string toScene)
    {
        yield return new WaitForSeconds(Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.BlendTime);
        SceneManager.UnloadSceneAsync(fromScene);
        
        //update active scene
        currentActiveScene = toScene;
        
        //reset bool lock
        isChangingScene = false;
        
        //invoke action
        onSceneChange?.Invoke(toScene);
    }
}