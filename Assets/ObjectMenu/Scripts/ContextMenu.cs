using UnityEngine;
using UnityEngine.UI;
using System;


public class ContextMenu : MonoBehaviour
{
    [Header("Dependencies")]
    public setupScene setupSceneObj;

    GameObject marker;
    GameObject cube;
    GameObject contextMenu;
    GameObject canvasTransform;
    float globalBuildingScale;

    int buildingID;
    float livingArea;
    int floors;
    Vector3 dims;
    Text textArea;
    Camera cam;
    int displayValueOffset;
    Vector3 oriPos;
    private float floorHeight;

    // Use this for initialization
    void Start(){
        globalBuildingScale = setupSceneObj.getGlobalBuildingScale();
        cam = GameObject.Find("Camera (eye)").GetComponent<Camera>();
        textArea = gameObject.GetComponent<Text>();
        contextMenu = textArea.transform.parent.gameObject;
        canvasTransform = contextMenu.transform.parent.gameObject;
        cube = canvasTransform.transform.parent.FindChild("Pivot").gameObject;
        marker = cube.transform.parent.gameObject;
        String id = marker.name.Substring(6);
        if (!id.Equals("Master"))
            buildingID = System.Int32.Parse(marker.name.Substring(6));
        else
            buildingID = 0;
        displayValueOffset = 10;
        oriPos = canvasTransform.transform.localPosition;
        floorHeight = setupSceneObj.getFloorHeight();
    }

    // Update is called once per frame
    void Update(){
        // cube = canvasTransform.transform.parent.transform.parent.FindChild("Pivot").gameObject;
        dims.x = cube.transform.localScale.x * displayValueOffset * globalBuildingScale;
        dims.y = cube.transform.localScale.y * displayValueOffset * globalBuildingScale;
        dims.z = cube.transform.localScale.z * displayValueOffset * globalBuildingScale;
        floors = (int)(dims.y / floorHeight/displayValueOffset/globalBuildingScale);

        canvasTransform.transform.LookAt(2 * canvasTransform.transform.position - cam.transform.position);
        canvasTransform.transform.localPosition = new Vector3(oriPos.x, oriPos.y + dims.y/displayValueOffset/globalBuildingScale, oriPos.z);
        livingArea = dims.x * dims.z * floors;

        textArea.text = "Building ID: \t" + buildingID + "\n" +
            "Scale: \t\t\t1:" + Math.Round((1 / 0.005) / globalBuildingScale) + "\n" +
            "Width: \t\t\t" + dims.x.ToString("0.##") + " m\n" +
            "Depth: \t\t\t" + dims.z.ToString("0.##") + " m\n" +
            "Height: \t\t" + dims.y.ToString("0.##") + " m\n" +
            "Floors: \t\t\t" + floors + "\n" +
            "Base area: \t" + (dims.x * dims.z).ToString("0.##") + " m²\n" +
            "Living area: \t" + livingArea.ToString("0.##") + " m²";
    }
}
