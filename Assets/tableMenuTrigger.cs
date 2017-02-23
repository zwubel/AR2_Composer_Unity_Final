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
    private int SavedCubesCounter;
    public GameObject applybutton;

    //TODO

    ArrayList instancedMarkers;
    ArrayList trackedMarkers;

    // Use this for initialization
    void Start()
    {
        SavedCubesCounter = 0;
        lastContactLoad = Time.timeSinceLevelLoad;
        lastContactSave = Time.timeSinceLevelLoad;
        lastContactCancel = Time.timeSinceLevelLoad;
        lastContactXML = Time.timeSinceLevelLoad;
        triggering = false;
        trackedMarkers = new ArrayList();
        instancedMarkers = new ArrayList();
         }

    public void addInstancedMarker(GameObject cube)
    {
       
        if (!instancedMarkers.Contains(cube))
        {

            instancedMarkers.Add(cube);
             Debug.Log("We added the instanced marker: " + cube.name);
            
        }
      
    }

    public void addTrackedMarker(GameObject cube)
    {
       
        if (!trackedMarkers.Contains(cube))
        {

            trackedMarkers.Add(cube);
            Debug.Log("We added tracked: " + cube.name);
        }

    }

    public void removeInstancedMarker(GameObject cube)
    {

        Object O = (Object)cube;
        object o = (object)O;
        if (instancedMarkers.Contains(o))
        {
           
            instancedMarkers.Remove(cube);
             Debug.Log("We removed instance: " + cube.name);
        }

    }

    public void removeTrackedMarker(GameObject cube)
    {

        Object O = (Object)cube;
        object o = (object)O;
        if (trackedMarkers.Contains(o))
       {
            Debug.Log("We remove the tracked marker: " + cube.name);
            instancedMarkers.Remove(cube);
          
        }

    }

    public void increaseSavedCubesCounter()
    {
        SavedCubesCounter++;

    }
    public int getSavedCubesCounter()
    {
        return SavedCubesCounter;

    }

    public int getNumberOfActiveMarker()
    {
        return instancedMarkers.Count;

    }

    public bool isInsideActiveMarker(GameObject gameobject)
    {
        return instancedMarkers.Contains(gameobject);

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
                        Debug.Log("init load");
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
                        Debug.Log("init Timeline save");
                        gameObject.GetComponent<Timeline>().initTimeline();
                        lastContactSave = Time.timeSinceLevelLoad;
                    }
                }
                else if (gameObject.transform.name == "TableMenuButtons_Apply")
                {
                    float actualMilis = Time.timeSinceLevelLoad;
                    if (actualMilis - lastContactSave >= 0.2f)
                    {
                        if (SavedCubesCounter == instancedMarkers.Count)
                        {
                        deleteMarkerDuplicates(instancedMarkers); 
                        }

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
           GameObject temp = (GameObject)instancedMarkers[i];
           instancedMarkers.RemoveAt(i);
            Debug.Log("NarkerDuplicate removed: " + temp.name);
           Destroy(temp);
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
        if (GameObject.Find("TableMenuButtons_Save").gameObject.GetComponent<tableMenuTrigger>().getSavedCubesCounter()== GameObject.Find("TableMenuButtons_Save").gameObject.GetComponent<tableMenuTrigger>().instancedMarkers.Count && applybutton.activeSelf == false)
        {
            Debug.Log("SavedCubesCounter " + GameObject.Find("TableMenuButtons_Save").gameObject.GetComponent<tableMenuTrigger>().getSavedCubesCounter() + "instancedMarkers " + GameObject.Find("TableMenuButtons_Save").gameObject.GetComponent<tableMenuTrigger>().instancedMarkers.Count);
            //start copy properties
            applybutton.SetActive(true);
      
        }

    }
}
