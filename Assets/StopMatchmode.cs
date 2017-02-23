using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopMatchmode : MonoBehaviour {

    public bool colliding;

    public GameObject CubeA;
    public GameObject CubeB;
    public GameObject CubeC;
    public GameObject CubeD;

    private int myID;

    private GameObject collidingTrackedMarker;
    
    // Use this for initialization
    void Start () {
        colliding = false;
        myID = 0;
       
	}
	
    private void findMyID(GameObject marker)
    {
        string s = gameObject.transform.parent.transform.parent.name.Substring(6);
        //Debug.Log(s);
        myID = int.Parse(gameObject.transform.parent.transform.parent.name.Substring(6));

    }

    void OnTriggerEnter(Collider collider)
    {
        colliding = true;
        collidingTrackedMarker = collider.gameObject.transform.parent.transform.parent.gameObject;
    }

    void OnTriggerExit(Collider collider)
    {
        Debug.Log("onTriggerExit");
        colliding = false;
        collidingTrackedMarker = null;
    }


    // Update is called once per frame
    void Update () {
        if (CubeA.GetComponent<StopMatchmode>().colliding == true && CubeB.GetComponent<StopMatchmode>().colliding == true && CubeC.GetComponent<StopMatchmode>().colliding == true && CubeD.GetComponent<StopMatchmode>().colliding == true)
        {
            findMyID(gameObject);
            if (myID > 100)
                gameObject.transform.parent.transform.parent.GetComponent<MatchModeReady>().setReadyState(true, collidingTrackedMarker);
        }
        else 
        {
            findMyID(gameObject);
            if (myID > 100)
            {
               // Debug.Log("run remove");
                gameObject.transform.parent.transform.parent.GetComponent<MatchModeReady>().setReadyState(false, collidingTrackedMarker);
                
            }
        }
    }
}
