using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchModeReady : MonoBehaviour {
    public  bool IsReadyForCopy;
    public DataHandler DH;
    private GameObject greenCube;

    void Start () {
        IsReadyForCopy = false;
        greenCube = gameObject.transform.FindChild("greenCube").gameObject;
    }
	
    //Sets the status of the marker in the matchmode
    public void setReadyState(bool state, GameObject trackedMarker){
        IsReadyForCopy = state;
        if (IsReadyForCopy == true){
            DH.addInstancedMarker(gameObject);
            DH.addTrackedMarker(trackedMarker);
            greenCube.GetComponent<MatchMode>().colorStart = new Color(0, 0.8f, 0);
            greenCube.GetComponent<MatchMode>().colorEnd = new Color(0, 0.5f, 0);            
        }
        if (IsReadyForCopy == false){
            DH.removeInstancedMarker(gameObject);
            DH.removeTrackedMarker(trackedMarker);
            greenCube.GetComponent<MatchMode>().colorStart = new Color(0.8f, 0, 0);
            greenCube.GetComponent<MatchMode>().colorEnd = new Color(0.5f, 0, 0);
        }
    }

    public bool getReadyState(){
        return IsReadyForCopy;
    }

	void Update () {	
	}
}
