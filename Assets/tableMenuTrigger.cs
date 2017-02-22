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
    float lastContactXML;

    //TODO

    ArrayList activeCubes;

    // Use this for initialization
    void Start()
    {
        lastContactLoad = Time.timeSinceLevelLoad;
        lastContactSave = Time.timeSinceLevelLoad;
        lastContactCancel = Time.timeSinceLevelLoad;
        lastContactXML = Time.timeSinceLevelLoad;
        triggering = false;

        activeCubes = new ArrayList();
    }

    public void addActiveCube( GameObject cube)
    {
        activeCubes.Add(cube);
        Debug.Log("We added: " + cube.name);
      
    }

    public void removeActiveCube(GameObject cube)
    {

        //TODO
    //    activeCubes.

    }

    public void increaseSavedCubesCounter()
    {
        //TODO

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
                        // TODO: Button Duplikate prüfen
                        gameObject.GetComponent<save>().saveScene();
                        gameObject.GetComponent<Timeline>().initTimeline();
                        lastContactSave = Time.timeSinceLevelLoad;
                    }
                }
                else if (gameObject.transform.name == "TableMenuButtons_Apply")
                {
                    float actualMilis = Time.timeSinceLevelLoad;
                    if (actualMilis - lastContactSave >= 0.2f)
                    {
                        deleteMarkerDuplicates(activeCubes); 

                    }
                }

                else if (gameObject.transform.name.Contains("xml")){
                    float actualMilis = Time.timeSinceLevelLoad;
                    if (actualMilis - lastContactXML >= 2f){
                        gameObject.GetComponent<open>().setPath();
                        lastContactXML = Time.timeSinceLevelLoad;
                    }
                }

            }
        }
    }



    public void deleteMarkerDuplicates(ArrayList arrayList)
    {
        for(int i=0; i< arrayList.Count; i++)
        {
            //TODO
            //GameObject.Destroy
            //arrayList[i]. Destroy
               



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
