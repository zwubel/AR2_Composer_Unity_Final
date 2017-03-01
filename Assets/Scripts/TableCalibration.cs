using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TableCalibration : MonoBehaviour{
    
    private int foundCalibMarkers;

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
    private Vector3[] calibPositions;
    //public Vector3 positionOffsetAruco;

    private Vector3 controllerPos1;
    private Vector3 controllerPos2;

    public void setCalibrateBoth(bool status){
        calibrateBoth = status;
    }

    void Start(){
        foundCalibMarkers = 0;
        calibPositions = new Vector3[20];
    }    

    // This is called by ControllerPos.cs when the trigger on
    // the right vive controller is pressed during calibration
    public void setPosition(Vector3 position){
        if (foundCalibMarkers < 20){
            int statusReceived = networkData.receiveTCPstatus();
            switch (statusReceived){
                case (int)readInNetworkData.TCPstatus.arucoFound:
                    networkData.sendCalibPosition(position);
                    calibPositions[foundCalibMarkers] = position;
                    if(foundCalibMarkers != 19)
                        StartCoroutine(LongVibration(0.2f, 3999));
                    else
                        StartCoroutine(LongVibration(0.5f, 3999));
                    Debug.Log("[PLANE CALIBRATION] Position " + (foundCalibMarkers + 1) + " calibrated to ("
                        + position.x + ", " + position.y + ", " + position.z + ")");
                    foundCalibMarkers++;
                    break;
                case (int)readInNetworkData.TCPstatus.arucoNotFound:
                    Debug.LogError("[PLANE CALIBRATION] AruCo marker not found, please try again.");
                    break;
            }
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
    }

    private void loadPreviousScene(){
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("doPlaneCalibInVS"));
        SceneManager.LoadScene("SelectCalibrationTarget", LoadSceneMode.Additive);

        // Disable controller position script
        controllerPos.enabled = false;

        // Disable this script
        this.enabled = false;

        calibrateBoth = false;
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

            if (foundCalibMarkers > 19){ // Workspace calibration successful
                Debug.Log("Plane calibration: completed successfully.");

                // Tell setupScene that the calibration has been
                // completed and load / unload corresponding scenes
                setupScene.calibrationDone(calibPositions[0], calibPositions[1]);

                // Write text file                
                string[] CalibPos = {   "" + calibPositions[0].x, "" + calibPositions[0].y, "" + calibPositions[0].z,
                                        "" + calibPositions[1].x, "" + calibPositions[1].y, "" + calibPositions[1].z};
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Application.dataPath + "/Resources/planeCalibData.txt")){
                    foreach (string line in CalibPos){
                        file.WriteLine(line);
                    }
                }
                foundCalibMarkers = 0;
                loadNextScene();
            }
        }                        
    }
}
