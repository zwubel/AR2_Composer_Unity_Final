using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CoverController : MonoBehaviour {
private bool isTriggered;

	// Use this for initialization
	void Start () {
        isTriggered = false;
	}

    public bool isTriggeredMarker()
    {
        return isTriggered;

    }

    void OnTriggerEnter(Collider trigger) {
        //Debug.Log("CoverControllerEnter");
        isTriggered = true;
    }

    void OnTriggerExit(Collider trigger) {
       // Debug.Log("CoverControllerExit");
        isTriggered = false;
    }
	
	// Update is called once per frame
	void Update () {
        //if (isTriggered) {
        //    gameObject.transform.parent.gameObject.SetActive(true);
        //}
	}
}
