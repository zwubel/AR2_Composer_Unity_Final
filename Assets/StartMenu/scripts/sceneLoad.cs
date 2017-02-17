using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class sceneLoad : MonoBehaviour {

    private Scene mainScene;

    void Start()
    {
        //if (SceneManager.GetActiveScene().name == "Welcome")
        //{
        //    SceneManager.LoadScene("AR2_Composer_Unity_Final", LoadSceneMode.Additive);

        //}

    }

    public void Close(string sceneClose){
        SceneManager.UnloadSceneAsync(sceneClose);
    }

    public void Open(string sceneOpen){
       SceneManager.LoadScene(sceneOpen, LoadSceneMode.Additive);
    }

    public void sendIP()
    {

            GameObject IPField = GameObject.Find("IPField");
            string IPPlaceholderText = IPField.transform.FindChild("IPPlaceholder").GetComponent<Text>().text;
            string IPtext = IPField.transform.FindChild("IPText").GetComponent<Text>().text;
            if (IPtext.Equals(""))
                FindObjectOfType<readInNetworkData>().setHostIP(IPPlaceholderText);
            else
                FindObjectOfType<readInNetworkData>().setHostIP(IPtext);
       
    
        }

    public void OpenARScene(string sceneName){
        //mainScene = SceneManager.GetSceneByName("AR2_Composer_Unity_Final");
        //SceneManager.SetActiveScene(mainScene);
        SceneManager.LoadScene("AR2_Composer_Unity_Final", LoadSceneMode.Single);
        SceneManager.LoadScene("CalibrateOrNot", LoadSceneMode.Additive);
       // sendIP();
        SceneManager.UnloadSceneAsync("Welcome");

    }
}

