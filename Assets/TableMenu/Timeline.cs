using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Timeline : MonoBehaviour {
	public DirectoryInfo currentDirectory;
	public FileInformation[] files;
	public GameObject TimeLineScrollView;
	public GameObject prefabButton;
    public bool debug = false;

	public void initTimeline(){

        string dir = Application.dataPath + "/Resources/saves/"; //This is our std. timeline path
        prefabButton.SetActive (true);
        currentDirectory = new DirectoryInfo(dir);
		FileInfo[] fia = currentDirectory.GetFiles();
		files = new FileInformation[fia.Length];
		int count=0;

        GameObject savedScenes = GameObject.Find("savedScenes");
        for(int i = 0; i< savedScenes.transform.childCount; i++)
        {
            if (savedScenes.transform.GetChild(i).name!= "TableMenuButtons_Entry")
                 Destroy(savedScenes.transform.GetChild(i));

        }

        //We crawl the file array and create buttons for all the ".xml" file
		for(int f=0;f<fia.Length;f++){
			files[f] = new FileInformation(fia[f]);
            Debug.Log(files[f].fi.Name);
			if(!files[f].fi.Name.Contains("meta") && files[f].fi.Name.Contains(".xml")){
				count++;
				GameObject button = Instantiate(prefabButton.gameObject);
				button.name = files [f].fi.Name;
				button.GetComponentInChildren<Text>().text =button.name.Remove(button.name.Length - 4);
				button.transform.SetParent(TimeLineScrollView.transform, false);
				button.transform.localPosition = new Vector3 (0,prefabButton.transform.localPosition.y +( 4 - count *4 ) , 0 );
                if(debug)
                    Debug.Log ("reading file: " + button.name);
			}

		}

		prefabButton.SetActive (false);
		
	}


	void Start () {
        Debug.Log("init Timeline Start()");
		initTimeline ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
