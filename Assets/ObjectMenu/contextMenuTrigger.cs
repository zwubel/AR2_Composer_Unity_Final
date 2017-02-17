using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class contextMenuTrigger : MonoBehaviour
{
    public GameObject pivot;
    private bool triggering;
    private Collider triggerCollider;
    private bool showContextMenu;
    float lastContactGreenCube;
    float lastContactZ;
    float lastContactX;
    float lastContactY;
    Vector3 oldPosition;
    Vector3 startPosition;

    Vector3 handStartPosition;

    // Use this for initialization
    void Start()
    {
        startPosition = gameObject.transform.parent.transform.localPosition;
        lastContactGreenCube = Time.timeSinceLevelLoad;
        lastContactZ = Time.timeSinceLevelLoad;
        lastContactX = Time.timeSinceLevelLoad;
        lastContactY = Time.timeSinceLevelLoad;

        triggering = false;
        showContextMenu = false;
        if (gameObject.name == "greenCube")
        {
            gameObject.transform.parent.transform.FindChild("X_Handle").gameObject.SetActive(false);
            gameObject.transform.parent.transform.FindChild("Y_Handle").gameObject.SetActive(false);
            gameObject.transform.parent.transform.FindChild("Z_Handle").gameObject.SetActive(false);
            gameObject.transform.parent.transform.FindChild("CanvasTransform").gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider trigger)
    {
       Debug.Log("showContextMenu: " + showContextMenu);
        if (!triggering)
        {
            triggering = true;
            if (trigger.gameObject.name == "bone3" || trigger.gameObject.name == "bone2" || trigger.gameObject.name == "bone1" )
            //if(trigger.gameObject.transform.parent.transform.parent.name== "RigidRoundHand_R" )
            {
            handStartPosition = trigger.transform.position;    
            triggerCollider = trigger;
                

                if (gameObject.transform.name == "greenCube")
                {
                   
                   float actualMilis = Time.timeSinceLevelLoad;

                    Debug.Log("Actual: " + actualMilis + "last: " + lastContactGreenCube);

                    if (actualMilis - lastContactGreenCube >= 1) {
                        if (showContextMenu == false)
                        {
                            showContextMenu = true;
                            gameObject.transform.parent.transform.FindChild("X_Handle").gameObject.SetActive(true);
                            gameObject.transform.parent.transform.FindChild("Y_Handle").gameObject.SetActive(true);
                            gameObject.transform.parent.transform.FindChild("Z_Handle").gameObject.SetActive(true);
                            gameObject.transform.parent.transform.FindChild("CanvasTransform").gameObject.SetActive(true);
                            lastContactGreenCube = Time.timeSinceLevelLoad;
                        }
                        else if (showContextMenu == true)
                        {
                            showContextMenu = false;
                            gameObject.transform.parent.transform.FindChild("X_Handle").gameObject.SetActive(false);
                            gameObject.transform.parent.transform.FindChild("Y_Handle").gameObject.SetActive(false);
                            gameObject.transform.parent.transform.FindChild("Z_Handle").gameObject.SetActive(false);
                            gameObject.transform.parent.transform.FindChild("CanvasTransform").gameObject.SetActive(false);
                            lastContactGreenCube = Time.timeSinceLevelLoad;
                        }

                    }
                }

               else if (gameObject.transform.name == "Plus")
                {
                    float actualMilis = Time.timeSinceLevelLoad;
                    if (actualMilis - lastContactZ >= 0.2f)
                    {
                        pivot.GetComponent<MarkerScale>().extrudeBuilding();
                        lastContactZ = Time.timeSinceLevelLoad;
                    }
                }
                else if (gameObject.transform.name == "Minus")
                {
                    float actualMilis = Time.timeSinceLevelLoad;
                    if (actualMilis - lastContactZ >= 0.2f)
                    {
                        pivot.GetComponent<MarkerScale>().deExtrudeBuilding();
                        lastContactZ = Time.timeSinceLevelLoad;
                    }
                } 
            }
        }
    }

    void OnTriggerExit(Collider trigger){
        triggering = false;
        triggerCollider = null;
        if (gameObject.name == "CylinderX" ||gameObject.name == "CylinderY")
             startPosition = gameObject.transform.parent.transform.localPosition;
        if (gameObject.name == "CylinderX")
            startPosition.x *= 2;
        if(gameObject.name == "CylinderY")
            startPosition.z *= 2;

    }

    // Update is called once per frame
    void Update()
    {
        if (triggerCollider != null && triggering)
        {
            if (triggerCollider.gameObject.name == "bone3" || triggerCollider.gameObject.name == "bone2" || triggerCollider.gameObject.name == "bone1" && triggering == true)
            {
                if (gameObject.transform.name == "CylinderX")
                {
                    Vector3 goPosition = gameObject.transform.parent.transform.InverseTransformVector(triggerCollider.gameObject.transform.position);
                    Vector3 hoPosition = gameObject.transform.parent.transform.InverseTransformVector(handStartPosition);
                    Vector3 localDifference = (hoPosition - goPosition);
                    gameObject.transform.parent.transform.localPosition = new Vector3((startPosition.x - localDifference.y) / 2, startPosition.y, startPosition.z);

                }
                else if (gameObject.transform.name == "CylinderY")
                {

                    Vector3 goPosition = gameObject.transform.parent.transform.InverseTransformVector(triggerCollider.gameObject.transform.position);
                    Vector3 hoPosition = gameObject.transform.parent.transform.InverseTransformVector(handStartPosition);
                    Vector3 localDifference = (hoPosition - goPosition);
                    gameObject.transform.parent.transform.localPosition = new Vector3(startPosition.x, startPosition.y, (startPosition.z - localDifference.y) / 2);
                }
           }
        }
    }
}
