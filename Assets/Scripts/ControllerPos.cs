﻿using UnityEngine;

public class ControllerPos : MonoBehaviour
{
    [Header("Dependencies")]
    public Transform tableCalib;
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controllerdevice;
    public readInNetworkData networkData;
    public setupScene setupSc;

    void Start(){        
    }

    //Sends the current Position of the ViveController 
    void Update(){
        if (setupSc.getCalibrationInProgress()){
            trackedObj = GetComponent<SteamVR_TrackedObject>();
            controllerdevice = SteamVR_Controller.Input((int)trackedObj.index);
            if (controllerdevice.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)){
                Debug.Log("Controller triggered.");
                networkData.sendTCPstatus((int)readInNetworkData.TCPstatus.controllerButtonPressed);
                Debug.Log("Controller triggered send.");
                tableCalib.GetComponent<TableCalibration>().setPosition(controllerdevice.transform.pos);
            }
        }
    }
}