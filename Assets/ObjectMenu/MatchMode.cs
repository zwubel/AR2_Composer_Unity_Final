using UnityEngine;
using System.Collections;

public class MatchMode : MonoBehaviour {
	public Color colorStart = Color.red;
	public Color colorEnd = Color.green;
	public float duration = 1.0F;
	private Renderer rend;
    public GameObject Cube;
	private bool matchMode;
    private GameObject tableObject;

    void Start () {
		rend = gameObject.GetComponent<Renderer>();
        tableObject = GameObject.Find("TableObject");
    }

    public void setMatchMode(bool state){
        matchMode = state;
    }
	

	// Update is called once per frame
	void Update () {
		if (matchMode) {
            //transform.parent.gameObject.SetActive (true);
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
            //transform.parent.gameObject.SetActive (false);
            gameObject.GetComponent<BoxCollider>().enabled = true;
            transform.gameObject.SetActive(false);
            Cube.gameObject.SetActive(true);

        }
	}
}
