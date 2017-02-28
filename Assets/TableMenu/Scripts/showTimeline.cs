using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showTimeline : MonoBehaviour {
    public GameObject ScrollView;
    public GameObject savedScenes;
    public GameObject saveSprite;
    public GameObject loadSprite;
    public GameObject cancelSprite;


	void Start () {
		
	}

   //Simple function for switching the visibility of the timeline
   public void  showHideTimeLine( )
    {
        if (ScrollView.activeSelf == true)
        {
            ScrollView.SetActive(false);
            savedScenes.SetActive(false);
            loadSprite.SetActive(false);
            cancelSprite.SetActive(false);
            saveSprite.SetActive(true);
        }

       
       else if (ScrollView.activeSelf == false)
        {
            ScrollView.SetActive(true);
            savedScenes.SetActive(true);
            loadSprite.SetActive(false);
            cancelSprite.SetActive(true);
            saveSprite.SetActive(false);
        }
            
    }

	// Update is called once per frame
	void Update () {
		
	}
}
