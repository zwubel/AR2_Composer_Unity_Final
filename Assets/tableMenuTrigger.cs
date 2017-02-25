using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tableMenuTrigger : MonoBehaviour
{
    private bool triggering;
    private Collider triggerCollider;
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


    void OnTriggerEnter(Collider trigger){
        if (!triggering){
            if (trigger.gameObject.name == "bone3" || trigger.gameObject.name == "bone2" || trigger.gameObject.name == "bone1"){
                triggering = true;
                triggerCollider = trigger;
                if (gameObject.transform.name == "TableMenuButtons_Cancel"){
                    float actualMilis = Time.timeSinceLevelLoad;
                    if (actualMilis - lastContactCancel >= 2 && actualMilis - lastContactLoad >= 2){
                        showTimeline.showHideTimeLine();
                        lastContactCancel = Time.timeSinceLevelLoad;
                        DH.deleteMarkerDuplicates();
                        DH.SavedCubesCounter = 0;
                    }
                }else if (gameObject.transform.name == "TableMenuButtons_Save"){
                    float actualMilis = Time.timeSinceLevelLoad;
                    if (actualMilis - lastContactSave >= 2f){
                        gameObject.GetComponent<save>().saveScene();
                        if (debug)
                            Debug.Log("init Timeline save");
                        gameObject.GetComponent<Timeline>().initTimeline();
                        lastContactSave = Time.timeSinceLevelLoad;
                    }
                }else if (gameObject.transform.name == "TableMenuButtons_Apply"){
                    float actualMilis = Time.timeSinceLevelLoad;
                    if (actualMilis - lastContactSave >= 2f){
                        if (DH.SavedCubesCounter == DH.instancedMarkers.Count){
                            DH.copyProperties2GameMarker();
                            DH.deleteMarkerDuplicates();
                            DH.SavedCubesCounter = 0;
                            applybutton.SetActive(false);
                        }
                    }
                }else if (gameObject.transform.name.Contains("xml")) {
                    float actualMilis = Time.timeSinceLevelLoad;
                    if (actualMilis - lastContactXML >= 2f) {
                        gameObject.GetComponent<open>().setPath();
                        lastContactXML = Time.timeSinceLevelLoad;
                    }
                }else if (gameObject.transform.name.Contains("TableMenuButtons_one2nine")){
                    float actualMilis = Time.timeSinceLevelLoad;
                    if (actualMilis - lastContactsavedScenes >= 2f){
                        savedScenes.SetActive(true);
                        savedScenes1.SetActive(false);
                        savedScenes2.SetActive(false);
                        savedScenes3.SetActive(false);
                        lastContactsavedScenes = Time.timeSinceLevelLoad;
                    }
                }else if (gameObject.transform.name.Contains("TableMenuButtons_ten2nineteen")){
                    float actualMilis = Time.timeSinceLevelLoad;
                    if (actualMilis - lastContactsavedScenes1 >= 2f){
                        savedScenes.SetActive(false);
                        savedScenes1.SetActive(true);
                        savedScenes2.SetActive(false);
                        savedScenes3.SetActive(false);
                        lastContactsavedScenes1 = Time.timeSinceLevelLoad;
                    }
                }else if (gameObject.transform.name.Contains("TableMenuButtons_twenty2twentynine")){
                    float actualMilis = Time.timeSinceLevelLoad;
                    if (actualMilis - lastContactsavedScenes2 >= 2f){
                        savedScenes.SetActive(false);
                        savedScenes1.SetActive(false);
                        savedScenes2.SetActive(true);
                        savedScenes3.SetActive(false);
                        lastContactsavedScenes2 = Time.timeSinceLevelLoad;
                    }
                }else if (gameObject.transform.name.Contains("TableMenuButtons_thirty2thirtynine")){
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
        triggerCollider = null;
    }

    void Update(){
        if (DH.getSavedCubesCounter() != 0 && DH.getSavedCubesCounter() == DH.instancedMarkers.Count){
            applybutton.SetActive(true);
        }else
            applybutton.SetActive(false);
    }
}
