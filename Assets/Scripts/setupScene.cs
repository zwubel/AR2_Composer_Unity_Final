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
    //private Marker[] networkMarkersPrevFrame;

    private bool markerArraySet = false;
    //private GameObject table;
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
    private float globalHeight;

    // Calibrated positions for plane    
    private Vector3 calibratedPos0;
    private Vector3 calibratedPos1;
    private Vector3 calibratedPos2;
    private Vector3 calibratedPos3;
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

    public bool getCalibDone(){
        return calibDone;
    }

    public float getGlobalHeight(){
        return globalHeight;
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

    // Is called when 'no' is selected in 'CalibrateOrNot' menu
    public void noCalibration(){
        Debug.Log("[SETUP SCENE] No Calibration has been selected. Loading saved information.");
        string[] planeCalibDatText = System.IO.File.ReadAllLines(Application.dataPath + "/Resources/planeCalibData.txt");
        if (planeCalibDatText.Length != 12){
            Debug.LogError("[SETUP SCENE] 'No calibration' has been selected, but no valid text file has been read.");
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("CalibrateOrNot"));
            SceneManager.LoadScene("SelectCalibrationTarget", LoadSceneMode.Additive);
        }else{
            Vector3 pos0 = new Vector3();
            Vector3 pos1 = new Vector3();
            Vector3 pos2 = new Vector3();
            Vector3 pos3 = new Vector3();
            pos0.x = float.Parse(planeCalibDatText[0]);
            pos0.y = float.Parse(planeCalibDatText[1]);
            pos0.z = float.Parse(planeCalibDatText[2]);
            pos1.x = float.Parse(planeCalibDatText[3]);
            pos1.y = float.Parse(planeCalibDatText[4]);
            pos1.z = float.Parse(planeCalibDatText[5]);
            pos2.x = float.Parse(planeCalibDatText[6]);
            pos2.y = float.Parse(planeCalibDatText[7]);
            pos2.z = float.Parse(planeCalibDatText[8]);
            pos3.x = float.Parse(planeCalibDatText[9]);
            pos3.y = float.Parse(planeCalibDatText[10]);
            pos3.z = float.Parse(planeCalibDatText[11]);

            Debug.Log("[LOADING CALIBRATION DATA] Position 0: " + pos0);
            Debug.Log("[LOADING CALIBRATION DATA] Position 1: " + pos1);
            Debug.Log("[LOADING CALIBRATION DATA] Position 2: " + pos2);
            Debug.Log("[LOADING CALIBRATION DATA] Position 3: " + pos3);

            calibrationDone(pos0, pos1, pos2, pos3);

            SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("CalibrateOrNot"));
            SceneManager.LoadScene("SetScale", LoadSceneMode.Additive);
        }
    }

    // Is called when table calibration finishes (in TableCalibration.cs)
    public void calibrationDone(Vector3 pos0, Vector3 pos1, Vector3 pos2, Vector3 pos3){

        // Make plane corners available globally
        calibratedPos0 = pos0;
        calibratedPos1 = pos1;
        calibratedPos2 = pos2;
        calibratedPos3 = pos3;

        // Create cylinders marking the corners of the workspace
        GameObject cornerSprite = GameObject.Find("CornerSprite");
        GameObject corner0 = Instantiate(cornerSprite);
        corner0.name = "Corner0";
        corner0.transform.position = pos0;
        corner0.transform.localRotation = Quaternion.Euler(new Vector3(90.0f, 0.0f, 0.0f));
        GameObject corner1 = Instantiate(cornerSprite);
        corner0.name = "Corner1";
        corner1.transform.position = pos1;
        corner1.transform.localRotation = Quaternion.Euler(new Vector3(90.0f, 90.0f, 0.0f));
        GameObject corner2 = Instantiate(cornerSprite);
        corner0.name = "Corner2";
        corner2.transform.position = pos2;
        corner2.transform.localRotation = Quaternion.Euler(new Vector3(90.0f, 180.0f, 0.0f));
        GameObject corner3 = Instantiate(cornerSprite);
        corner0.name = "Corner3";
        corner3.transform.position = pos3;
        corner3.transform.localRotation = Quaternion.Euler(new Vector3(90.0f, 270.0f, 0.0f));
        cornerSprite.SetActive(false);

        // Put table menu next lower right corner cylinder
        GameObject tableMenuParent = GameObject.Find("TableMenuParent");
        tableMenuParent.transform.position = new Vector3(   corner3.transform.position.x + 0.1f,
                                                            corner3.transform.position.y,
                                                            corner3.transform.position.z
                                                        );
        tableMenuParent.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));

        // Used for leap height calibration
        globalHeight = (calibratedPos0.y + calibratedPos1.y + calibratedPos2.y + calibratedPos3.y) / 4;

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
        //networkMarkersPrevFrame = new Marker[0];
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
                
                markerCubes[i].transform.position = new Vector3(cur.getPosX(), globalHeight, cur.getPosZ());
                markerCubes[i].transform.rotation = Quaternion.Euler(0.0f, cur.getAngle(), 0.0f);
                
                if (cur.getStatus() == 1)
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