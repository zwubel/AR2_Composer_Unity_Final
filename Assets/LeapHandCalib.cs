using UnityEngine;

public class LeapHandCalib : MonoBehaviour {
    public GameObject rightHand;
    public SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controllerdevice;
    private bool controllerFailed;
    private bool heightSet;

    void Start () {
        Debug.Log("Put your right hand on the table and press key \"c\" to cablibrate Leap Height.");
        controllerFailed = false;
        heightSet = false;

    }
	
	void Update () {
        if (!heightSet) { 
            if (!trackedObj.gameObject.activeSelf && !controllerFailed){
                Debug.LogError("[TABLE HEIGHT CALIBRATION] Controller (right) not found, please connect device and try again!");
                controllerFailed = true;
            }else if(trackedObj.gameObject.activeSelf){
                controllerdevice = SteamVR_Controller.Input((int)trackedObj.index);
                if (controllerdevice.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)){
                    float posHand = rightHand.transform.position.y;
                    float plane = GameObject.Find("TablePlane").transform.position.y;
                    gameObject.transform.position += new Vector3(gameObject.transform.position.x, plane - posHand, gameObject.transform.position.z);
                    heightSet = true;
                }
            }
        }

        //if (Input.GetKeyDown(KeyCode.C)) {
        //    Debug.Log("Pressed C Key");
        //   float posHand= rightHand.transform.position.y;
        //   float plane = GameObject.Find("TablePlane").transform.position.y;
        //    gameObject.transform.position += new Vector3( gameObject.transform.position.x, plane - posHand, gameObject.transform.position.z);
        //   Debug.Log("Leap height calibrated with " + (plane-posHand).ToString());
        //}

    }
}
