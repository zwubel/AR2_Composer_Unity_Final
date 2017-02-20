using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputNormalizedControllerPos : MonoBehaviour {
 
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controllerdevice;
    private Vector3 calibratedLL;
    private Vector3 calibratedUR;
    public Vector3 controllerPos;

    void Start()
    {
        controllerPos = new Vector3();
        trackedObj = GameObject.Find("Controller (right)").GetComponent<SteamVR_TrackedObject>();
    }

    public void setCalibratedPositions(Vector3 lowerLeft, Vector3 upperRight)
    {
        calibratedLL = lowerLeft;
        calibratedUR = upperRight;
    }

    private Vector3 getCalibratedPos(Vector3 position)
    {
        //float newX = Math.Abs()        

        // Linear interpolation of X
        float xMin = calibratedLL.x;
        float xMax = calibratedUR.x;
        float newX = xMin + position.x * (xMax - xMin);        

        // Linear interpolation of Z
        float zMin = calibratedUR.z;
        float zMax = calibratedLL.z;
        float newZ = zMin + position.z * (zMax - zMin);

        return new Vector3(newX, position.y, newZ);
    }

    void Update(){
        controllerdevice = SteamVR_Controller.Input((int)trackedObj.index);
        controllerPos = getCalibratedPos(controllerdevice.transform.pos);
    }
}
