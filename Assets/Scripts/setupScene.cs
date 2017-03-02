using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System.IO;

public class setupScene : MonoBehaviour{
    private GameObject[] markerCubes;

    // Marker data received over TCP
    private Marker[] networkMarkers;
    private Marker[] networkMarkersPrevFrame;

    private bool markerArraySet = false;
    private GameObject table;
    private GameObject parent;

    [Header("Dependencies")]
    // TableCalibration script input
    public TableCalibration tableCalib;
    private readInNetworkData networkData;
    public ControllerPos controllerPos;

    // This is overwritten by inspector input
    [Header("Scene Settings")]
    // Maximum number of markers that can be displayed (virtual markers)
    private int markersToRender;
    private float globalBuildingScale;
    public float floorHeight;

    // Global scale of each marker to fit size of virtual to real markers
    //public float markerScale = 0.05f;
    public float planeHeightOffset = -0.023f;
    //public float markerHeightOffset = -0.023f;

    // Calibrated positions for plane
    private Vector3 calibratedLL;
    private Vector3 calibratedUR;
    private bool calibDone;
    private Plane workspacePlane;
    private bool calibrationInProgress;

    // State for the main loop
    public enum state { planeCalib, poseAndPlaneCalib, startScene }
    bool statusChanged;
    int currentState;
    bool doRender;

    // Can be called to set the current state of the "Update()-loop"
    public void setState(int state){
        currentState = state;
        statusChanged = true;
        doRender = false;
        Debug.Log("[STATE LOOP] State changed to: " + Enum.GetName(typeof(state), state));
    }

    public float getGlobalBuildingScale(){
        return globalBuildingScale;
    }

    public void setGlobalBuildingScale(float value){
        globalBuildingScale = value * 200; // Value mapped to 1:200
    }

    public float getFloorHeight(){
        return floorHeight;
    }

    public bool getCalibrationInProgress(){
        return calibrationInProgress;
    }

    public void setCalibrationInProgress (bool state) {
        calibrationInProgress = state;
    }

    public void noCalibration(){
        Debug.Log("[SETUP SCENE] No Calibration has been selected. Loading saved information.");
        string[] planeCalibDatText = System.IO.File.ReadAllLines(Application.dataPath + "/Resources/planeCalibData.txt");
        if (planeCalibDatText.Length != 6){
            Debug.LogError("[SETUP SCENE] 'No calibration' has been selected, but no valid text file has been read.");
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("CalibrateOrNot"));
            SceneManager.LoadScene("SelectCalibrationTarget", LoadSceneMode.Additive);
        }else{
            Vector3 lowerLeft = new Vector3();
            Vector3 upperRight = new Vector3();
            lowerLeft.x = float.Parse(planeCalibDatText[0]);
            lowerLeft.y = float.Parse(planeCalibDatText[1]);
            lowerLeft.z = float.Parse(planeCalibDatText[2]);
            upperRight.x = float.Parse(planeCalibDatText[3]);
            upperRight.y = float.Parse(planeCalibDatText[4]);
            upperRight.z = float.Parse(planeCalibDatText[5]);

            Debug.Log("[LOADING CALIBRATION DATA] lowerLeft: " + lowerLeft);
            Debug.Log("[LOADING CALIBRATION DATA] upperRight: " + upperRight);

            calibrationDone(lowerLeft, upperRight);

            SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("CalibrateOrNot"));
            SceneManager.LoadScene("SetScale", LoadSceneMode.Additive);
        }
    }

    // Is called when table calibration finishes (in TableCalibration.cs)
    public void calibrationDone(Vector3 lowerLeft, Vector3 upperRight){
        
        // Make plane corners available globally
        calibratedLL = lowerLeft;
        calibratedUR = upperRight;

        GameObject alreadyCalibrated = GameObject.Find("TablePlane");
        if (alreadyCalibrated != null)
            Destroy(alreadyCalibrated);

        // Create plane (table surface)
        table = GameObject.CreatePrimitive(PrimitiveType.Plane);
        table.name = "TablePlane";
        table.transform.parent = parent.transform;
        float width = Math.Abs(calibratedUR.x - calibratedLL.x);
        float height = Math.Abs(calibratedUR.z - calibratedLL.z);
        Vector3 position = new Vector3( (calibratedLL.x + calibratedUR.x) / 2,
                                        ((calibratedLL.y + calibratedUR.y) / 2) + planeHeightOffset,
                                        (calibratedLL.z + calibratedUR.z) / 2
                                      );
        table.transform.position = position;

        // A PrimitiveType.Plane is instantiated as 10x10 in the XZ plane,
        // therefore we need to bring it down to one in either direction
        table.transform.localScale = new Vector3(width / 10, 1, height / 10);
        table.GetComponent<MeshCollider>().enabled = false;        

        // Put table menu next to table plane
        GameObject tableMenuParent = GameObject.Find("TableMenuParent");
        tableMenuParent.transform.parent = table.transform;        
        tableMenuParent.transform.position = new Vector3((position.x + width / 2), 0, position.z - height / 2);
        tableMenuParent.transform.localPosition = new Vector3(tableMenuParent.transform.localPosition.x, 0, tableMenuParent.transform.localPosition.z);
        tableMenuParent.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));

        calibrationInProgress = false;
        calibDone = true;
    }

    // This is called when a new calibration is done, because
    // the old saves are not compatible with a newly calibrated
    // camera and/or workspace.
    private void deleteSavesDir(){
        string path = Application.dataPath + "/Resources/saves/";
        if (Directory.Exists(path)) { 
            Directory.Delete(path, true);
            Directory.CreateDirectory(path);
        }
    }

    void Start() {
        // Initialization
        calibDone = false;
        calibrationInProgress = false;
        tableCalib.enabled = false;        
        networkMarkersPrevFrame = new Marker[0];
        networkData = gameObject.GetComponent<readInNetworkData>();
        markersToRender = networkData.getMarkersToReceive() + 1;
        markerCubes = new GameObject[markersToRender];

        // Create parent object (plane and cubes are attached to this)
        parent = new GameObject();
        parent.transform.name = "TableObject";
        createMarkers();
    }

    // Create marker GameObjects
    private void createMarkers(){        
        GameObject MarkerMaster = GameObject.Find("MarkerMaster");
        for (int i = 1; i < markersToRender; i++)
        {
            markerCubes[i] = Instantiate(MarkerMaster);
            markerCubes[i].transform.SetParent(parent.transform);
            markerCubes[i].SetActive(false);
            markerCubes[i].transform.name = "Marker" + i;
        }
        MarkerMaster.SetActive(false);
    }

    // Set whether the marker array has been filled
    // (is called from readInNetworkData.cs)
    public void setMarkerArraySet(bool state){
        markerArraySet = state;
    }

    private void renderMarkersFromTCP(){
        if (markerArraySet){
            // Pull markers for current frame from readInNetworkData script
            networkMarkers = networkData.getMarkers();

            for (int i = 1; i < networkMarkers.Length; i++){
                Marker cur = networkMarkers[i];
                if (cur == null) // Not necessary any more, but can't hurt
                    continue;
                if (cur.getID() == -1){
                    markerCubes[i].SetActive(false);
                    continue;
                }
                if (cur.getID() == -2) // End of frame reached
                    break;

                markerCubes[i].transform.position = new Vector3(cur.getPosX(), table.transform.position.y, cur.getPosZ());
                markerCubes[i].transform.rotation = Quaternion.Euler(0.0f, cur.getAngle(), 0.0f);
                
                if (cur.getStatus() == 1 || markerCubes[i].transform.FindChild("CoverCollider").GetComponent<CoverController>().isTriggeredMarker())
                    markerCubes[i].SetActive(true);
                else
                    markerCubes[i].SetActive(false);
            }
            markerArraySet = false;
        }
    }

    void Update(){
        if (doRender)
            renderMarkersFromTCP();
        else if (statusChanged) {
            statusChanged = false;
            switch (currentState){

                // MENU SELECTION: 'Workspace' in 'SelectCalibrationTarget' scene
                case (int)state.planeCalib:
                    deleteSavesDir();
                    calibrationInProgress = true;
                    Debug.Log("[STATE LOOP] Entered state: planeCalib");
                    tableCalib.enabled = true;
                    controllerPos.enabled = true;
                    networkData.sendTCPstatus((int)readInNetworkData.TCPstatus.planeOnlyCalib);
                    // Continue in TableCalibration.cs
                    break;
                
                // MENU SELECTION: 'Workspace and camera' in 'SelectCalibrationTarget' scene
                case (int)state.poseAndPlaneCalib:
                    deleteSavesDir();
                    calibrationInProgress = true;
                    SceneManager.LoadScene("doPoseCalibInVS", LoadSceneMode.Additive);
                    Debug.Log("[STATE LOOP] Entered state: poseAndPlaneCalib");
                    networkData.sendTCPstatus((int)readInNetworkData.TCPstatus.planeAndPoseCalib);
                    tableCalib.enabled = true;
                    controllerPos.enabled = true;
                    tableCalib.setCalibrateBoth(true);
                    // Continue in TableCalibration.cs                    
                    break;

                // MENU SELECTION: 'Start' in 'SetScale' scene
                case (int)state.startScene:
                    GameObject.Find("TableMenuButtons_Save").GetComponent<Timeline>().initTimeline();
                    Debug.Log("[STATE LOOP] Entered state: startScene");
                    networkData.sendTCPstatus((int)readInNetworkData.TCPstatus.sceneStart);
                    networkData.setSceneStarted(true);
                    doRender = true;
                    break;
                default: Debug.Log("[STATE LOOP] State loop: no state specified."); break;
            }
        }
    }
}