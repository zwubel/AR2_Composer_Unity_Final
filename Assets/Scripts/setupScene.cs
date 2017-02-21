using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

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
    public float globalBuildingScale;
    public float floorHeight;

    // Global scale of each marker to fit size of virtual to real markers
    //public float markerScale = 0.05f;
    public float planeHeightOffset = -0.023f;
    public float markerHeightOffset = -0.023f;

    // Calibrated positions for plane
    private Vector3 calibratedLL;
    private Vector3 calibratedUR;
    private bool calibDone;

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
        globalBuildingScale = value * 200; // Value mapped to 1:200 being the
                                           // "original" size of the markers        
    }

    public float getFloorHeight(){
        return floorHeight;
    }

    public void noCalibration(){
        Debug.Log("[SETUP SCENE] No Calibration has been selected. Loading saved information.");
        string[] planeCalibDatText = System.IO.File.ReadAllLines(Application.dataPath + "/Resources/planeCalibData.txt");
        if (planeCalibDatText.Length != 6){
            Debug.LogError("[SETUP SCENE] 'No calibration' has been selected, but no valid text file has been read.");
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("CalibrateOrNot"));
            SceneManager.LoadScene("SelectCalibrationTarget", LoadSceneMode.Additive);

        }
        else{
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
        // Make marker positions available globally
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
        Vector3 position = new Vector3(
                                        (calibratedLL.x + calibratedUR.x) / 2,
                                        ((calibratedLL.y + calibratedUR.y) / 2) + planeHeightOffset,
                                        (calibratedLL.z + calibratedUR.z) / 2
                                      );
        table.transform.position = position;
        table.transform.localScale = new Vector3(width / 10, 1, height / 10);

        GameObject tableMenuParent = GameObject.Find("TableMenuParent");
        tableMenuParent.transform.parent    = table.transform;
        tableMenuParent.transform.position = new Vector3(position.x  +width / 2 , position.y, position.z - height / 2  );

        calibDone = true;        
    }

    void Start() {
        // Initialization
        calibDone = false;
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

    // Create markers (cubes)
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
    public void setMarkerArraySet(bool state){
        markerArraySet = state;
    }

    // Returns the position on the plane for the tracked (normalized) marker position
    private Vector3 getCalibratedMarkerPos(Vector3 position){
        // Linear interpolation of X
        float xMin = calibratedLL.x;
        float xMax = calibratedUR.x;
        float newX = xMin + position.x * (xMax - xMin);

        // Linear interpolation of Y
        float newY = (calibratedUR.y + calibratedLL.y) / 2;
        newY += markerHeightOffset;

        // Linear interpolation of Z
        float zMin = calibratedUR.z;
        float zMax = calibratedLL.z;
        float newZ = zMin + position.z * (zMax - zMin);

        return new Vector3(newX, newY, newZ);
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

                // This makes no sense, but is a temporary fix
                // for a problem perhaps caused by the Vive HMD
                Vector3 position = new Vector3(cur.getPosX(), 0.0f, cur.getPosY());

                if (calibDone)
                    markerCubes[i].transform.position = getCalibratedMarkerPos(position);
                else
                    markerCubes[i].transform.position = position;
                markerCubes[i].transform.rotation = Quaternion.Euler(0.0f, cur.getAngle(), 0.0f);

                if (cur.getStatus() == 1) // Is marker visible?
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
                    Debug.Log("[STATE LOOP] Entered state: planeCalib");
                    tableCalib.enabled = true;
                    controllerPos.enabled = true;
                    networkData.sendTCPstatus((int)readInNetworkData.TCPstatus.planeOnlyCalib);
                    // Continue in TableCalibration.cs
                    break;
                
                // MENU SELECTION: 'Workspace and camera' in 'SelectCalibrationTarget' scene
                case (int)state.poseAndPlaneCalib:
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
                    Debug.Log("[STATE LOOP] Entered state: startScene");
                    networkData.sendTCPstatus((int)readInNetworkData.TCPstatus.sceneStart);
                    networkData.setSceneStarted(true);
                    doRender = true;
                    break;
                default: Debug.Log("[STATE LOOP] State loop: no state specified."); break;
            }
        }
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        throw new NotImplementedException();
    }
}