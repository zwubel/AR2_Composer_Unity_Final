using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class sceneStart : MonoBehaviour {

    private Scene mainScene;

    void Start()
    {
        SceneManager.LoadScene("AR2_Composer_Unity_Final", LoadSceneMode.Additive);
    }

   

    public void OpenARScene(string sceneName)
    {
        mainScene = SceneManager.GetSceneByName("AR2_Composer_Unity_Final");
        SceneManager.SetActiveScene(mainScene);
        SceneManager.LoadScene("CalibrateOrNot", LoadSceneMode.Additive);
        GameObject IPField = GameObject.Find("IPField");
        string IPPlaceholderText = IPField.transform.FindChild("IPPlaceholder").GetComponent<Text>().text;
        string IPtext = IPField.transform.FindChild("IPText").GetComponent<Text>().text;
        sendIP(IPtext, IPPlaceholderText);
        SceneManager.UnloadSceneAsync("Welcome");

    }

    public void sendIP(string text, string placeholder)
    {
        if (text.Equals(""))
            FindObjectOfType<readInNetworkData>().setHostIP(placeholder);
        else
            FindObjectOfType<readInNetworkData>().setHostIP(text);
    }
}
