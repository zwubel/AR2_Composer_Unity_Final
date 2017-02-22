using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopMatchmode : MonoBehaviour {

    public bool colliding;

    public GameObject CubeA;
    public GameObject CubeB;
    public GameObject CubeC;
    public GameObject CubeD;
    // Use this for initialization
    void Start () {
        colliding = false;
	}
	
    void OnTriggerEnter(Collider collider)
    {
        colliding = true;
    }

    void onTriggerExit()
    {
        colliding = false;
    }


    // Update is called once per frame
    void Update () {
		if(CubeA.GetComponent<StopMatchmode>().colliding == true && CubeB.GetComponent<StopMatchmode>().colliding == true && CubeC.GetComponent<StopMatchmode>().colliding == true && CubeD.GetComponent<StopMatchmode>().colliding == true)
        {
            Debug.Log("STOPPING MATCHMODE");
            gameObject.transform.parent.transform.parent.transform.GetComponent<MatchModeReady>().setReadyState(true);
        }
        else
            gameObject.transform.parent.transform.parent.transform.GetComponent<MatchModeReady>().setReadyState(false);

    }
}
