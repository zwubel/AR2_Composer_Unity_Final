using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Timeline : MonoBehaviour {
	public DirectoryInfo currentDirectory;
	public FileInformation[] files;
	public GameObject TimeLineScrollView;
	public GameObject prefabButton;

	public void initTimeline(){

        string dir = Application.dataPath + "/resources/saves/"; //This is our std. timeline path
        prefabButton.SetActive (true);
        currentDirectory = new DirectoryInfo(dir);
		FileInfo[] fia = currentDirectory.GetFiles();
		files = new FileInformation[fia.Length];
		int count=0;

        //We crawl the file array and create buttons for all the ".xml" file
		for(int f=0;f<fia.Length;f++){
			files[f] = new FileInformation(fia[f]);
			if(!files[f].fi.Name.Contains("meta")&&files[f].fi.Name.Contains(".xml")){
				count++;
				GameObject button = (GameObject)Instantiate(prefabButton.gameObject);
				button.name = files [f].fi.Name;
				button.GetComponentInChildren<Text>().text =button.name.Remove(button.name.Length - 4);
				button.transform.SetParent(TimeLineScrollView.transform, false);
				button.transform.localPosition = new Vector3 (0,prefabButton.transform.localPosition.y +( 4 - count *4 ) , 0 );
				Debug.Log ("reading file: " + button.name);
			}

		}

		prefabButton.SetActive (false);
		
	}


	void Start () {
		initTimeline ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
