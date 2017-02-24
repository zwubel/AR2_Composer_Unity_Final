﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TableCalibration : MonoBehaviour{
    private int markerCounter;
    private bool LLset;
    private bool URset;
    private bool additionalLRset;

    [Header("Dependencies")]
    public setupScene setupScene;
    public readInNetworkData networkData;
    public ControllerPos controllerPos;

    // Needed for haptic feedback
    public SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controllerdevice;

    [Header("Calibration")]
    public Vector3 lowerLeft;
    public Vector3 upperRight;
    public Vector3 additionalLowerRight;
    private bool calibrateBoth;
    public Vector3 positionOffsetAruco;

    public void setCalibrateBoth(bool status){
        calibrateBoth = status;
    }

    void Start(){
        //controllerExceptionThrown = false;
    }

    //// This is called by ControllerPos.cs when the trigger on
    //// the right vive controller is pressed during calibration
    //public void setPosition(Vector3 position){
    //    int statusReceived = networkData.receiveTCPstatus();
    //    switch (statusReceived){
    //        case (int)readInNetworkData.TCPstatus.arucoFound1:
    //            if(!LLset && !URset) {
    //                lowerLeft = position;
    //                LLset = true;
    //                StartCoroutine(LongVibration(0.2f, 3999));
    //                Debug.Log("[PLANE CALIBRATION] Lower left corner calibrated to " + position);
    //            }else{
    //                Debug.LogError("[PLANE CALIBRATION] Lower left corner: received status arucoFound1," +
    //                    " but one position has already been set.");
    //            }
    //            break;
    //        case (int)readInNetworkData.TCPstatus.arucoFound2:
    //            if (LLset && !URset){
    //                upperRight = position;
    //                URset = true;
    //                StartCoroutine(LongVibration(0.5f, 3999));
    //                System.Threading.Thread.Sleep(2000);
    //                Debug.Log("[PLANE CALIBRATION] Upper right corner calibrated to " + position);
    //            }else{
    //                Debug.LogError("[PLANE CALIBRATION] Upper right corner: received status arucoFound2," +
    //                    " but either lower left has not been set yet or upper right already has been.");
    //            }
    //            break;
    //        case (int)readInNetworkData.TCPstatus.arucoNotFound: Debug.LogError("[PLANE CALIBRATION] " +
    //            "AruCo marker not found, please try again."); break;
    //        case -1: Debug.LogError("[PLANE CALIBRATION] Failed, because of a socket error."); break;
    //        default: Debug.LogError("[PLANE CALIBRATION] Unknown status received: " + statusReceived); break;
    //    }
    //}

    // This is called by ControllerPos.cs when the trigger on
    // the right vive controller is pressed during calibration
    public void setPosition(Vector3 position){
        int statusReceived = networkData.receiveTCPstatus();
        switch (statusReceived){
            case (int)readInNetworkData.TCPstatus.arucoFound1:
                if (!LLset && !URset && !additionalLRset){
                    lowerLeft = position + positionOffsetAruco;
                    LLset = true;
                    StartCoroutine(LongVibration(0.2f, 3999));
                    Debug.Log("[PLANE CALIBRATION] Lower left corner calibrated to ("
                        + lowerLeft.x + ", " + lowerLeft.y + ", " + lowerLeft.z + ")");
                }
                else{
                    Debug.LogError("[PLANE CALIBRATION] Lower left corner: error!");
                }
                break;
            case (int)readInNetworkData.TCPstatus.arucoFound2:
                if (LLset && !URset && !additionalLRset){
                    upperRight = position + positionOffsetAruco;
                    URset = true;
                    StartCoroutine(LongVibration(0.2f, 3999));
                    Debug.Log("[PLANE CALIBRATION] Upper right corner calibrated to (" 
                        + upperRight.x + ", " + upperRight.y +", " + upperRight.z + ")");
                }else{
                    Debug.LogError("[PLANE CALIBRATION] Upper right corner: error!");
                }
                break;
            case (int)readInNetworkData.TCPstatus.arucoFound3:
                if (LLset && URset && !additionalLRset){
                    additionalLowerRight = position + positionOffsetAruco;
                    additionalLRset = true;
                    StartCoroutine(LongVibration(0.5f, 3999));
                    System.Threading.Thread.Sleep(2000);
                    Debug.Log("[PLANE CALIBRATION] Additional lower right corner calibrated to ("
                        + additionalLowerRight.x + ", " + additionalLowerRight.y + ", " + additionalLowerRight.z + ")");
                }
                else{
                    Debug.LogError("[PLANE CALIBRATION] Additional lower right corner: error!");
                }
                break;
            case (int)readInNetworkData.TCPstatus.arucoNotFound:
                Debug.LogError("[PLANE CALIBRATION] AruCo marker not found, please try again."); break;
            case -1: Debug.LogError("[PLANE CALIBRATION] Failed, because of a socket error."); break;
            default: Debug.LogError("[PLANE CALIBRATION] Unknown status received: " + statusReceived); break;
        }
    }

    IEnumerator LongVibration(float length, float strength){
        for (float i = 0; i < length; i += Time.deltaTime){
            controllerdevice.TriggerHapticPulse((ushort)Mathf.Lerp(0, 3999, strength));
            yield return null;
        }
    }

    private void loadNextScene(){
        if (networkData.receiveTCPstatus() == (int)readInNetworkData.TCPstatus.planeCalibDone){
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("doPlaneCalibInVS"));
            SceneManager.LoadScene("CalibDone", LoadSceneMode.Additive);
        }

        // Disable controller position script
        controllerPos.enabled = false;

        // Disable this script
        this.enabled = false;

        calibrateBoth = false;
        LLset = false;
        URset = false;
        additionalLRset = false;
    }

    private void loadPreviousScene(){
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("doPlaneCalibInVS"));
        SceneManager.LoadScene("SelectCalibrationTarget", LoadSceneMode.Additive);

        // Disable controller position script
        controllerPos.enabled = false;

        // Disable this script
        this.enabled = false;

        calibrateBoth = false;
        LLset = false;
        URset = false;
        additionalLRset = false;
    }

    void Update() {
        // If both calibrations have been selected, do pose calibration first
        if (calibrateBoth) {
            calibrateBoth = false;            
            if (networkData.receiveTCPstatus() == (int)readInNetworkData.TCPstatus.poseCalibDone) {
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("doPoseCalibInVS"));
                SceneManager.LoadScene("doPlaneCalibInVS", LoadSceneMode.Additive);                
            }
            // and then the workspace calibration
        } else {
            // Needed for haptic feedback
            if (!trackedObj.gameObject.activeSelf){ // Check whether controller is switched on
                Debug.LogError("[TABLE CALIBRATION] Controller (right) not found, please connect device and try again!");
                loadPreviousScene();
            }else{
                controllerdevice = SteamVR_Controller.Input((int)trackedObj.index);
            }
            //if (LLset && URset){ // Workspace calibration successful
            //    Debug.Log("Plane calibration: completed successfully.");

            //    // Tell setupScene that the calibration has been
            //    // completed and load / unload corresponding scenes
            //    setupScene.calibrationDone(lowerLeft, upperRight);

            //    // Write text file            
            //    string[] CalibPos = { "" + lowerLeft.x, "" + lowerLeft.y, "" + lowerLeft.z, "" + upperRight.x, "" + upperRight.y, "" + upperRight.z };
            //    using (System.IO.StreamWriter file = new System.IO.StreamWriter(Application.dataPath + "/Resources/planeCalibData.txt"))
            //    {
            //        foreach (string line in CalibPos)
            //        {
            //            file.WriteLine(line);
            //        }
            //    }
            //    loadNextScene();
            //}
            if (LLset && URset && additionalLRset){ // Workspace calibration successful
                Debug.Log("Plane calibration: completed successfully.");

                // Tell setupScene that the calibration has been
                // completed and load / unload corresponding scenes
                Vector3 heightDeviations = networkData.receiveHeightDeviations();
                setupScene.calibrationDone(lowerLeft, upperRight, additionalLowerRight, heightDeviations);

                // Write text file
                string[] CalibPos = {   "" + lowerLeft.x, "" + lowerLeft.y, "" + lowerLeft.z,
                                        "" + upperRight.x, "" + upperRight.y, "" + upperRight.z,
                                        "" + additionalLowerRight.x, "" + additionalLowerRight.y, "" + additionalLowerRight.z,
                                        "" + heightDeviations.x, "" + heightDeviations.y, "" + heightDeviations.z};
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Application.dataPath + "/Resources/planeCalibData.txt")){
                    foreach (string line in CalibPos){
                        file.WriteLine(line);
                    }
                }
                loadNextScene();
            }
        }                        
    }
}
