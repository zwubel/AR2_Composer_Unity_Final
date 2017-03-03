using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tableMenuTrigger : MonoBehaviour
{
    private bool triggering;
    public showTimeline showTimeline;

    public DataHandler DH;

    float lastContactLoad;
    float lastContactSave;
    float lastContactCancel;
    float lastContactXML;
    float lastContactsavedScenes;
    float lastContactsavedScenes1;
    float lastContactsavedScenes2;
    float lastContactsavedScenes3;

    public GameObject applybutton;
    public GameObject savebutton;
    public GameObject cancelbutton;
    public GameObject savedScenes;
    public GameObject savedScenes1;
    public GameObject savedScenes2;
    public GameObject savedScenes3;

    public bool debug = false;
       
    // Use this for initialization
    void Start(){
        lastContactLoad = Time.timeSinceLevelLoad;
        lastContactSave = Time.timeSinceLevelLoad;
        lastContactCancel = Time.timeSinceLevelLoad;
        lastContactXML = Time.timeSinceLevelLoad;
        triggering = false;
    }

    //This function controlls the behaviour of the table menu when the different buttons are touched
    void OnTriggerEnter(Collider trigger){
        if (!triggering){
            if (trigger.gameObject.name == "bone3" || trigger.gameObject.name == "bone2" || trigger.gameObject.name == "bone1"){
                triggering = true;
                if (gameObject.transform.name == "TableMenuButtons_Cancel"){ //cancel button
                    float actualMilis = Time.timeSinceLevelLoad;
                    if (actualMilis - lastContactCancel >= 2 && actualMilis - lastContactLoad >= 2){
                        DH.deleteAllMarkers();
                        DH.SavedCubesCounter = 0;
                        cancelbutton.SetActive(false);
                        lastContactCancel = Time.timeSinceLevelLoad;
                    }
                }else if (gameObject.transform.name == "TableMenuButtons_Save"){ //save button
                    float actualMilis = Time.timeSinceLevelLoad;
                    if (actualMilis - lastContactSave >= 2f){
                        gameObject.GetComponent<save>().saveScene();
                        if (debug)
                            Debug.Log("init Timeline save");
                        gameObject.GetComponent<Timeline>().initTimeline();
                        lastContactSave = Time.timeSinceLevelLoad;
                    }
                }else if (gameObject.transform.name == "TableMenuButtons_Apply"){ //apply button
                    float actualMilis = Time.timeSinceLevelLoad;
                    if (actualMilis - lastContactSave >= 2f){
                        if (DH.SavedCubesCounter == DH.instancedMarkers.Count){
                            DH.copyProperties2GameMarker();
                            DH.deleteMarkerDuplicates();
                            DH.SavedCubesCounter = 0;
                            applybutton.SetActive(false);
                            savebutton.SetActive(true);
                            cancelbutton.SetActive(false);
                        }
                    }
                }else if (gameObject.transform.name.Contains("xml")) { //saved scenes buttons
                    float actualMilis = Time.timeSinceLevelLoad;
                    if (actualMilis - lastContactXML >= 2f) {
                        DH.deleteAllMarkers();
                        DH.SavedCubesCounter = 0;
                        gameObject.GetComponent<open>().setPath();
                        savebutton.SetActive(false);
                        cancelbutton.SetActive(true);
                        lastContactXML = Time.timeSinceLevelLoad;
                    }
                }else if (gameObject.transform.name.Contains("TableMenuButtons_one2nine")){ //toggel button1
                    float actualMilis = Time.timeSinceLevelLoad;
                    if (actualMilis - lastContactsavedScenes >= 2f){
                        savedScenes.SetActive(true);
                        savedScenes1.SetActive(false);
                        savedScenes2.SetActive(false);
                        savedScenes3.SetActive(false);
                        lastContactsavedScenes = Time.timeSinceLevelLoad;
                    }
                }else if (gameObject.transform.name.Contains("TableMenuButtons_ten2nineteen")){  //toggel button2
                    float actualMilis = Time.timeSinceLevelLoad;
                    if (actualMilis - lastContactsavedScenes1 >= 2f){
                        savedScenes.SetActive(false);
                        savedScenes1.SetActive(true);
                        savedScenes2.SetActive(false);
                        savedScenes3.SetActive(false);
                        lastContactsavedScenes1 = Time.timeSinceLevelLoad;
                    }
                }else if (gameObject.transform.name.Contains("TableMenuButtons_twenty2twentynine")){ //toggel button3
                    float actualMilis = Time.timeSinceLevelLoad;
                    if (actualMilis - lastContactsavedScenes2 >= 2f){
                        savedScenes.SetActive(false);
                        savedScenes1.SetActive(false);
                        savedScenes2.SetActive(true);
                        savedScenes3.SetActive(false);
                        lastContactsavedScenes2 = Time.timeSinceLevelLoad;
                    }
                }else if (gameObject.transform.name.Contains("TableMenuButtons_thirty2thirtynine")){ //toggel button4
                    float actualMilis = Time.timeSinceLevelLoad;
                    if (actualMilis - lastContactsavedScenes3 >= 2f){
                        savedScenes.SetActive(false);
                        savedScenes1.SetActive(false);
                        savedScenes2.SetActive(false);
                        savedScenes3.SetActive(true);
                        lastContactsavedScenes3 = Time.timeSinceLevelLoad;
                    }
                }
            }
        }
    }

    void OnTriggerExit(Collider trigger){
        triggering = false;
    }

    void Update(){
        if (DH.getSavedCubesCounter() != 0 && DH.getSavedCubesCounter() == DH.instancedMarkers.Count){
            applybutton.SetActive(true);
        }else
            applybutton.SetActive(false);
    }
}
