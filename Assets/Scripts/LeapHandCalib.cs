using UnityEngine;

public class LeapHandCalib : MonoBehaviour {
    public GameObject rightHand;
    public SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controllerdevice;
    private bool heightSet;
    private Transform tablePlane;
    public setupScene setupScene;

    void Start () {
        heightSet = false;
    }
	
    //If there is an height offset between the table and the LEAP Handmodel, this function eliminates it. 
    //This is neccesary, because the table menu might not be touchable in that case
	void Update () {
        if (!heightSet) {
            if (trackedObj.gameObject.activeSelf && setupScene.getCalibDone()){
                controllerdevice = SteamVR_Controller.Input((int)trackedObj.index);
                if (controllerdevice.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)){
                    float newY = setupScene.getGlobalHeight() - rightHand.transform.position.y - 0.01f;
                    gameObject.transform.position += new Vector3(gameObject.transform.position.x, newY, gameObject.transform.position.z);
                    heightSet = true;
                }
            }                
        }
    }
}
