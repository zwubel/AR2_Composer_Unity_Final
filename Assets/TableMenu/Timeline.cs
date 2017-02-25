using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class Timeline : MonoBehaviour {
	public DirectoryInfo currentDirectory;
	public FileInformation[] files;
	public GameObject SavedScenes;
    public GameObject SavedScenes1;
    public GameObject SavedScenes2;
    public GameObject SavedScenes3;

    public GameObject prefabButton;
    public bool debug = false;

    private int PageCounter;
    int fileCounter;

    public void initTimeline(){

        string dir = Application.dataPath + "/Resources/saves/"; //This is our std. timeline path
        prefabButton.SetActive (true);
        currentDirectory = new DirectoryInfo(dir);
		FileInfo[] fia = currentDirectory.GetFiles();
		files = new FileInformation[fia.Length];		

        if (SavedScenes != null) {
            for (int i = 0; i < SavedScenes.transform.childCount; i++){
                Transform child = SavedScenes.transform.GetChild(i);
                if (child != null && child.name != "TableMenuButtons_Entry")
                    Destroy(child.gameObject);
            }
        }
      
        if (SavedScenes1 != null){
            for (int i = 0; i < SavedScenes1.transform.childCount; i++){
                Transform child = SavedScenes1.transform.GetChild(i);
                if (child != null && child.name != "TableMenuButtons_Entry")
                    Destroy(child.gameObject);
            }
        }
    
        if (SavedScenes2 != null){
            for (int i = 0; i < SavedScenes2.transform.childCount; i++){
                Transform child = SavedScenes2.transform.GetChild(i);
                if (child != null && child.name != "TableMenuButtons_Entry")
                    Destroy(child.gameObject);
            }
        }

        if (SavedScenes3 != null){
            for (int i = 0; i < SavedScenes3.transform.childCount; i++){
                Transform child = SavedScenes3.transform.GetChild(i);
                if (child != null && child.name != "TableMenuButtons_Entry")
                    Destroy(child.gameObject);
            }
        }

        // Crawl file array and create buttons for all ".xml" file
        //PageCounter = 0;
        //fileCounter = 0;
        //int count = 0;
        ArrayList xmlFiles = new ArrayList();
        int counter = 0;
        for (int i = fia.Length - 1; i >= 0; i--){
            FileInformation fi = new FileInformation(fia[i]);
            if (counter >= 23)
                break;
            if (!fi.fi.Name.Contains(".meta")){
                xmlFiles.Add(fi);
                counter++;
            }            
        }
        int positioningCounter = 0;
        for(int j = 0; j < xmlFiles.Count; j++){            
            GameObject button = Instantiate(prefabButton.gameObject);
            int page = j / 6;
            switch (page){
                case 0: button.transform.parent = SavedScenes.transform;
                    SavedScenes.SetActive(true);
                    SavedScenes1.SetActive(false);
                    SavedScenes2.SetActive(false);
                    SavedScenes3.SetActive(false);
                    break;
                case 1: button.transform.parent = SavedScenes1.transform;
                    SavedScenes.SetActive(false);
                    SavedScenes1.SetActive(true);
                    SavedScenes2.SetActive(false);
                    SavedScenes3.SetActive(false);
                    break;
                case 2: button.transform.parent = SavedScenes2.transform;
                    SavedScenes.SetActive(false);
                    SavedScenes1.SetActive(false);
                    SavedScenes2.SetActive(true);
                    SavedScenes3.SetActive(false);
                    break;
                case 3: button.transform.parent = SavedScenes3.transform;
                    SavedScenes.SetActive(false);
                    SavedScenes1.SetActive(false);
                    SavedScenes2.SetActive(false);
                    SavedScenes3.SetActive(true);
                    break;
                default: Debug.LogError("The selected page is not available on the table menu."); break;
            }

            button.name = ((FileInformation)xmlFiles[j]).fi.Name;
            button.GetComponentInChildren<Text>().text = button.name.Remove(button.name.Length - 4);
            button.transform.localPosition = new Vector3(0, prefabButton.transform.localPosition.y - 4 + (4 - positioningCounter * 4), 0);
            button.transform.localScale = new Vector3(1, 1, 1);
            button.transform.localRotation = Quaternion.Euler(new Vector3());

            positioningCounter++;
            if (j == 5 || j == 11 || j == 17)
                positioningCounter = 0;            
        }
		prefabButton.SetActive (false);
	}

	void Start () {
        if(debug)
            Debug.Log("init Timeline Start()");
        initTimeline ();
	}
	
	void Update () {	
	}
}
