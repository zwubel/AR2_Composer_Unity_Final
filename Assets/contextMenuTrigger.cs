using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class contextMenuTrigger : MonoBehaviour
{
    public GameObject pivot;
    private bool triggering;
    private Collider triggerCollider;
    // Use this for initialization
    void Start()
    {
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
    void Update()
    {

        if (triggerCollider != null)
        {
            if (triggerCollider.gameObject.name == "bone3" || triggerCollider.gameObject.name == "bone2" || triggerCollider.gameObject.name == "bone1" && triggering == true)
            {
                Vector3 oldPosition = gameObject.transform.parent.transform.position;
                gameObject.transform.parent.transform.position = triggerCollider.transform.position;


                Debug.Log("triggering " + gameObject.name);

                if (gameObject.name == "CylinderX")
                {
                    
                    gameObject.transform.parent.transform.localPosition = new Vector3(gameObject.transform.parent.transform.localPosition.x + 0.825f, oldPosition.y, oldPosition.z);
                }
                else if (gameObject.transform.name == "CylinderY")
                {
                    gameObject.transform.parent.transform.localPosition = new Vector3(oldPosition.x, oldPosition.y, gameObject.transform.parent.localPosition.z+0.825f);

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
