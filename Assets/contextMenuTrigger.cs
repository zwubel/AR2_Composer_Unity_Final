using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class contextMenuTrigger : MonoBehaviour {
    public GameObject pivot;
    private bool triggering;
    private Collider triggerCollider;
    // Use this for initialization
    void Start() {
        triggering = false;
    }

    void OnTriggerEnter(Collider trigger)
    {
        if (!triggering)
        {
            triggering = true;
            triggerCollider = trigger;
        }
    }

    void OnTriggerExit(Collider trigger)
    {
        triggering = false;
        triggerCollider = null;
    }

    // Update is called once per frame
    void Update() {

        if (triggerCollider != null){
            if (triggerCollider.gameObject.name == "bone3" || triggerCollider.gameObject.name == "bone2" || triggerCollider.gameObject.name == "bone1" && triggering == true)
            {
                
                if (gameObject.transform.parent.name == "CylinderX")
                {
                    gameObject.transform.parent.position = triggerCollider.transform.position;
                    gameObject.transform.parent.localPosition = new Vector3(gameObject.transform.parent.localPosition.x, gameObject.transform.parent.position.y, gameObject.transform.parent.position.z);
                }
                else if (gameObject.transform.parent.name == "CylinderY")
                {
                    gameObject.transform.parent.position = triggerCollider.transform.position;
                    gameObject.transform.parent.localPosition = new Vector3(gameObject.transform.parent.position.x, gameObject.transform.parent.position.y, gameObject.transform.parent.localPosition.z);

                }
                else if (gameObject.transform.parent.name == "Plus")
                {

                    pivot.GetComponent<MarkerScale>().extrudeBuilding();

                }
                else if (gameObject.transform.parent.name == "Minus")
                {
                    pivot.GetComponent<MarkerScale>().deExtrudeBuilding();

                }
            }
        }
    }
}
