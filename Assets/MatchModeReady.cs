using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchModeReady : MonoBehaviour {
    public  bool IsReadyForCopy;
	// Use this for initialization
	void Start () {
        IsReadyForCopy = false;
    }
	
    public void setReadyState(bool state, GameObject trackedMarker)
    {
        IsReadyForCopy = state;
        if(IsReadyForCopy == true){
            GameObject.Find("TableMenuButtons_Save").GetComponent<tableMenuTrigger>().addInstancedMarker(gameObject);
            GameObject.Find("TableMenuButtons_Save").GetComponent<tableMenuTrigger>().addTrackedMarker(trackedMarker);

        }
        if (IsReadyForCopy == false)
        {
            //Debug.Log("isReadyFor Copy false");
            GameObject.Find("TableMenuButtons_Save").GetComponent<tableMenuTrigger>().removeInstancedMarker(gameObject);
            GameObject.Find("TableMenuButtons_Save").GetComponent<tableMenuTrigger>().removeTrackedMarker(trackedMarker);
        }
    }

    public bool getReadyState()
    {
        return IsReadyForCopy;

    }

	// Update is called once per frame
	void Update () {
		
	}
}
