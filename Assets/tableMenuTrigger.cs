using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tableMenuTrigger: MonoBehaviour
{
    private bool triggering;
    private Collider triggerCollider;
    public showTimeline showTimeline;
    float lastContactLoad;
    float lastContactSave;
    float lastContactCancel;


    // Use this for initialization
    void Start()
    {
        lastContactLoad = Time.timeSinceLevelLoad;
        lastContactSave = Time.timeSinceLevelLoad;
        lastContactCancel = Time.timeSinceLevelLoad;
        triggering = false;
    }

    void OnTriggerEnter(Collider trigger)
    {
        if (!triggering)
        {
            
            if (trigger.gameObject.name == "bone3" || trigger.gameObject.name == "bone2" || trigger.gameObject.name == "bone1")
            {
                triggering = true;
                triggerCollider = trigger;
                if (gameObject.transform.name == "TableMenuButtons_Load")
                {
                    float actualMilis = Time.timeSinceLevelLoad;
                    if (actualMilis - lastContactLoad >= 2 && actualMilis - lastContactCancel >= 2 )
                    {
                        gameObject.GetComponent<Timeline>().initTimeline();
                        showTimeline.showHideTimeLine();
                        lastContactLoad = Time.timeSinceLevelLoad;
                    }
                }
                if (gameObject.transform.name == "TableMenuButtons_Cancel")
                {
                    float actualMilis = Time.timeSinceLevelLoad;
                    if (actualMilis - lastContactCancel >= 2 && actualMilis - lastContactLoad >= 2)
                    {
                        showTimeline.showHideTimeLine();
                        lastContactCancel = Time.timeSinceLevelLoad;
                    }
                }
                
                else if (gameObject.transform.name == "TableMenuButtons_Save")
                {
                    float actualMilis = Time.timeSinceLevelLoad;
                    if (actualMilis - lastContactSave >= 0.2f)
                    {
                        
                        gameObject.GetComponent<save>().saveScene();
                        gameObject.GetComponent<Timeline>().initTimeline();
                        lastContactSave = Time.timeSinceLevelLoad;
                    }
                }
                
                else if (gameObject.transform.name.Contains("xml"))
                {
                    gameObject.GetComponent<open>().setPath();
                }

            }
        }
    }

    void OnTriggerExit(Collider trigger)
    {
        triggering = false;
        triggerCollider = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
