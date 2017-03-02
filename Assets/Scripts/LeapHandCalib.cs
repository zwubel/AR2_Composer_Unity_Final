using UnityEngine;

public class LeapHandCalib : MonoBehaviour {
    public GameObject rightHand;
    public SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controllerdevice;
    private bool heightSet;
    private Transform tablePlane;

    void Start () {
        heightSet = false;
        tablePlane = null;// GameObject.Find("TableObject").transform.FindChild("TablePlane");
    }
	
	void Update () {
        if (!heightSet) {
            if (trackedObj.gameObject.activeSelf){
                if (tablePlane == null) { 
                    tablePlane = GameObject.Find("TableObject").transform.FindChild("TablePlane");
                }else{
                    controllerdevice = SteamVR_Controller.Input((int)trackedObj.index);
                    if (controllerdevice.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)){
                        float newY = tablePlane.position.y - rightHand.transform.position.y;
                        gameObject.transform.position += new Vector3(gameObject.transform.position.x, newY, gameObject.transform.position.z);
                        heightSet = true;
                        Debug.Log("Leap height set.");
                    }
                }
            }                
        }
    }
}
