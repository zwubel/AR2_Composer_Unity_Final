using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class sceneLoad : MonoBehaviour {

   

    public void Close(string sceneClose){
        SceneManager.UnloadSceneAsync(sceneClose);
    }

    public void Open(string sceneOpen){
       SceneManager.LoadScene(sceneOpen, LoadSceneMode.Additive);
    }

    public void OpenARScene(string sceneName) {
        SceneManager.LoadScene("AR2_Composer_Unity_Final", LoadSceneMode.Single);
        SceneManager.LoadScene("CalibrateOrNot", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("Welcome");

    }
}

