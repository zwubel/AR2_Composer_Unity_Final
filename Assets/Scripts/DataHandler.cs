using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script handles the data for saving and opening scenes with the table menu
public class DataHandler : MonoBehaviour {
    public ArrayList instancedMarkers;
    public ArrayList trackedMarkers;
    public int SavedCubesCounter;
    public bool debug = false;
    private ArrayList additionalMarkers;

    //Destroys all gameobjects of the markers
    public void deleteAllMarkers(){
        foreach (Object GO in additionalMarkers)
            Destroy((GameObject)GO);
    }

    //Adds an marker object
    public void addAdditionalMarker(GameObject marker)
    {
        additionalMarkers.Add(marker);
    }

    //Adds Instanciated markers 
    public void addInstancedMarker(GameObject cube){
        if (!instancedMarkers.Contains(cube)){
            instancedMarkers.Add(cube);
            if(debug)
                Debug.Log("We added the instanced marker: " + cube.name);
        }
    }

    //Adds TCP controlled markers 
    public void addTrackedMarker(GameObject cube){
        if (!trackedMarkers.Contains(cube)){
            trackedMarkers.Add(cube);
            if (debug)
                Debug.Log("We added tracked: " + cube.name);
        }
    }

    //Removes an instanciated marker
    public void removeInstancedMarker(GameObject cube)
    {
        Object O = (Object)cube;
        object o = (object)O;
        if (instancedMarkers.Contains(o)){
            instancedMarkers.Remove(cube);
            if (debug)
                Debug.Log("We removed instance: " + cube.name);
        }
    }

    //Removes an TCP controlled marker
    public void removeTrackedMarker(GameObject cube)
    {
        Object O = (Object)cube;
        object o = (object)O;
        if (trackedMarkers.Contains(o))
        {
            if (debug)
                Debug.Log("We remove the tracked marker: " + cube.name);
            instancedMarkers.Remove(cube);
        }
    }

    //Counter handler
    public void increaseSavedCubesCounter()
    {
        SavedCubesCounter++;
    }

    //Counter getter
    public int getSavedCubesCounter()
    {
        return SavedCubesCounter;
    }
    
    //Returns number of active instanciated markers
    public int getNumberOfActiveMarker()
    {
        return instancedMarkers.Count;
    }

    //Returns true if gameobject is attached to an instanciated marker
    public bool isInsideActiveMarker(GameObject gameobject)
    {
        return instancedMarkers.Contains(gameobject);
    }

    //Copies marker properties from instanciated markers (the marker which are loaded by opening a scene and wich contains all the saved 
    //marker informations) to the tcp controlled markers
    public void copyProperties2GameMarker(){
        for (int i = 0; i < instancedMarkers.Count; i++){
            GameObject instance = (GameObject)instancedMarkers[i];
            GameObject tracked = (GameObject)trackedMarkers[i];
            instance.transform.localRotation = Quaternion.FromToRotation(instance.transform.localRotation.eulerAngles, tracked.transform.localRotation.eulerAngles);
            tracked.transform.localRotation = instance.transform.localRotation;
            tracked.transform.FindChild("X_Handle").transform.localPosition = instance.transform.FindChild("X_Handle").transform.localPosition;
            tracked.transform.FindChild("Y_Handle").transform.localPosition = instance.transform.FindChild("Y_Handle").transform.localPosition;
            tracked.transform.FindChild("Pivot").transform.localScale = instance.transform.FindChild("Pivot").transform.localScale;
        }
    }    

    //Checkout dublicated markers
    public void deleteMarkerDuplicates(){
        for (int i = 0; i < instancedMarkers.Count; i++)
        {
            GameObject temp = (GameObject)instancedMarkers[i];
            if (debug)
                Debug.Log("MarkerDuplicate removed: " + temp.name);
            Destroy(temp);
        }
        instancedMarkers.Clear();
    }

    // Use this for initialization
    void Start () {
        instancedMarkers = new ArrayList();
        trackedMarkers = new ArrayList();
        SavedCubesCounter = 0;
        additionalMarkers = new ArrayList();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
