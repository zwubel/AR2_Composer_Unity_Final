using UnityEngine;
using System.Collections;

public class MatchMode : MonoBehaviour {
    public Color colorStart;
    public Color colorEnd;
	public float duration = 1.0F;
	private Renderer rend;
    public GameObject Cube;
	private bool matchMode;

    void Start () {
		rend = gameObject.GetComponent<Renderer>();
        colorStart = new Color(0.5f, 0, 0);
        colorEnd = new Color(0.8f, 0, 0); 
    }

    public void setMatchMode(bool state){
        matchMode = state;
    }
	

	// If matchmode is active, controll the marker color and set all the unneccesary objects deactive
	void Update () {
		if (matchMode) {
            gameObject.GetComponent<BoxCollider>().enabled = false;
            transform.gameObject.SetActive(true);
            Cube.gameObject.SetActive(false);
            float lerp = Mathf.PingPong (Time.time, duration) / duration;
			rend.material.color = Color.Lerp (colorStart, colorEnd, lerp);
            gameObject.transform.FindChild("CubeA").GetComponent<BoxCollider>().isTrigger = true;
            gameObject.transform.FindChild("CubeB").GetComponent<BoxCollider>().isTrigger = true;
            gameObject.transform.FindChild("CubeC").GetComponent<BoxCollider>().isTrigger = true;
            gameObject.transform.FindChild("CubeD").GetComponent<BoxCollider>().isTrigger = true;

        } else {
            gameObject.GetComponent<BoxCollider>().enabled = true;
            transform.gameObject.SetActive(false);
            Cube.gameObject.SetActive(true);

        }
	}
}
