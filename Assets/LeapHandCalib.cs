using UnityEngine;

public class LeapHandCalib : MonoBehaviour {
    public GameObject rightHand;
	
	void Start () {
        Debug.Log("Put your right hand on the table and press key \"c\" to cablibrate Leap Height.");
	}
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.C)) {
            Debug.Log("Pressed C Key");
           float posHand= rightHand.transform.position.y;
           float plane = GameObject.Find("TablePlane").transform.position.y;
            gameObject.transform.position += new Vector3( gameObject.transform.position.x, plane - posHand, gameObject.transform.position.z);
           Debug.Log("Leap height calibrated with " + (plane-posHand).ToString());
        }

    }
}
