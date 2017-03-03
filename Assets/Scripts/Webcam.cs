using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class Webcam : MonoBehaviour {
    public RawImage rawImage;
    public Renderer rend;

    // Crawling throug the array of connected cameras and connect the creative gesture cam
    void Start () {
	    WebCamDevice[] devices  = WebCamTexture.devices;
        rend = GetComponent<Renderer>();
        rend.enabled = true;
        WebCamTexture leftCam = new WebCamTexture();
        leftCam.deviceName = devices[1].name;

        for ( int i = 0; i< devices.Length; i++)
        {
            Debug.Log("Webcam found: " + devices[i].name);
            if (devices[i].name == "Creative GestureCam")
            {
                leftCam.deviceName = devices[i].name;
                Debug.Log("Monocamera connected successfully.");    
            }
        }
      
        //Setting the image of the webcam to the webcam plane
        rend.materials[0].mainTexture = leftCam;
        leftCam.Play();
    }

    // Update is called once per frame
    void Update () {
	
	}
}
