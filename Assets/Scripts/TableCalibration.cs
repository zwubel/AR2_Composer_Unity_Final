using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TableCalibration : MonoBehaviour{
    
    private int foundCalibMarkers;
    public int calibrationSamples;

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

    private Vector3 controllerPos1;
    private Vector3 controllerPos2;

    public void setCalibrateBoth(bool status){
        calibrateBoth = status;
    }

    void Start(){
        foundCalibMarkers = 0;
        calibPositions = new Vector3[calibrationSamples];
    }    

    // This is called by ControllerPos.cs when the trigger on
    // the right vive controller is pressed during calibration
    public void setPosition(Vector3 position){
        if (foundCalibMarkers < calibrationSamples){
            int statusReceived = networkData.receiveTCPstatus();
            switch (statusReceived){
                case (int)readInNetworkData.TCPstatus.arucoFound:
                    networkData.sendCalibPosition(position);
                    calibPositions[foundCalibMarkers] = position;
                    if(foundCalibMarkers != calibrationSamples - 1)
                        StartCoroutine(LongVibration(0.2f, 3999));
                    else
                        StartCoroutine(LongVibration(1f, 3999));
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

    // Continue to 'CalibDone' menu
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

    // Go to 'ControllerNotFound'
    private void loadPreviousScene(){
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("doPlaneCalibInVS"));
        SceneManager.LoadScene("ControllerNotFound", LoadSceneMode.Additive);

        // Disable controller position script
        controllerPos.enabled = false;

        // Disable this script
        this.enabled = false;

        calibrateBoth = false;
    }

    void Update() {
        // If both camera and workspace calibration have been selected, do camera calibration first...
        if (calibrateBoth) {
            calibrateBoth = false;
            if (networkData.receiveTCPstatus() == (int)readInNetworkData.TCPstatus.poseCalibDone) {
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("doPoseCalibInVS"));
                SceneManager.LoadScene("doPlaneCalibInVS", LoadSceneMode.Additive);                
            }
            // ...and then the workspace calibration
        } else {
            // Needed for haptic feedback
            if (!trackedObj.gameObject.activeSelf){ // Check whether controller is switched on
                Debug.LogError("[TABLE CALIBRATION] Controller (right) not found, please connect device and try again!");
                loadPreviousScene();
            }else{
                controllerdevice = SteamVR_Controller.Input((int)trackedObj.index);
            }         

            if (foundCalibMarkers > calibrationSamples - 1){ // Workspace calibration successful
                Debug.Log("Plane calibration: completed successfully.");

                // Write text file                
                string[] CalibPos = {   "" + calibPositions[0].x, "" + calibPositions[0].y, "" + calibPositions[0].z,
                                        "" + calibPositions[1].x, "" + calibPositions[1].y, "" + calibPositions[1].z};
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(Application.dataPath + "/Resources/planeCalibData.txt")){
                    foreach (string line in CalibPos){
                        file.WriteLine(line);
                    }
                }
                // Tell setupScene that the calibration has been
                // completed and load / unload corresponding scenes
                setupScene.calibrationDone(calibPositions[0], calibPositions[1]);

                GameObject corner0 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                corner0.name = "Corner0";
                corner0.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                corner0.transform.position = calibPositions[0];
                GameObject corner1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                corner1.name = "Corner1";
                corner1.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                corner1.transform.position = calibPositions[1];
                GameObject corner2 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                corner2.name = "Corner2";
                corner2.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                corner2.transform.position = calibPositions[2];
                GameObject corner3 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                corner3.name = "Corner3";
                corner3.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                corner3.transform.position = calibPositions[3];

                foundCalibMarkers = 0;
                loadNextScene();
            }
        }                        
    }
}
