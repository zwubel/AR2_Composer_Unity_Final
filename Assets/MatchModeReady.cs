using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchModeReady : MonoBehaviour {
    public  bool IsReadyForCopy;
	// Use this for initialization
	void Start () {
        IsReadyForCopy = false;
    }
	
    public void setReadyState(bool state)
    {
        IsReadyForCopy = state;
        if(IsReadyForCopy == true){
            GameObject.Find("TableMenuButtons_Apply").GetComponent<tableMenuTrigger>().addActiveCube(gameObject);
        }
        if (IsReadyForCopy == false)
        {
            GameObject.Find("TableMenuButtons_Apply").GetComponent<tableMenuTrigger>().removeActiveCube(gameObject);
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
