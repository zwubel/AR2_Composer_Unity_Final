using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHandler : MonoBehaviour {
    public ArrayList instancedMarkers;
    public ArrayList trackedMarkers;
    public int SavedCubesCounter;
    public bool debug = false;

    public void addInstancedMarker(GameObject cube){
        if (!instancedMarkers.Contains(cube)){
            instancedMarkers.Add(cube);
            if(debug)
                Debug.Log("We added the instanced marker: " + cube.name);
        }
    }

    public void addTrackedMarker(GameObject cube){
        if (!trackedMarkers.Contains(cube)){
            trackedMarkers.Add(cube);
            if (debug)
                Debug.Log("We added tracked: " + cube.name);
        }
    }

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

    public void deleteMarkerDuplicates(){
        for (int i = 0; i < instancedMarkers.Count; i++)
        {
            GameObject temp = (GameObject)instancedMarkers[i];
            if (debug)
                Debug.Log("MarkerDuplicate removed: " + temp.name);
            Destroy(temp);
        }
        trackedMarkers.Clear();
    }


    // Use this for initialization
    void Start () {
        instancedMarkers = new ArrayList();
        trackedMarkers = new ArrayList();
        SavedCubesCounter = 0;

    }

    // Update is called once per frame
    void Update () {
		
	}
}
