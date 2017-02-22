using UnityEngine;
using System;
using System.Collections;
using System.Xml;

public class open : MonoBehaviour {

    public bool debug = false;
	private String projectPath;    

    void Start () {
        projectPath = Application.dataPath + "/Resources/";
    }

    //This methode crawles through all the nodes
	void crawlXML( XmlNodeList nodes ){

        XmlNodeList markers = null;
        ArrayList activeMarkerIDs = new ArrayList();
        //int activeMarkerCounter = 0;
        for (int i=0; i< nodes.Count; i++){

            if (nodes[i].Name == "TableObject"){
                markers = nodes[i].ChildNodes; //We now have every marker node in the XmlNodeList called Marker                

                for (int j = 0; j < markers.Count -11; j++) // We don't need the last 11 items ( this are the position/scale/rot. from the TableObject node.)
                {
                    if(markers.Item(j).ChildNodes.Item(16).InnerText == "True")
                    {
                        activeMarkerIDs.Add(j);
                        if(debug)
                            Debug.Log("Marker is saved as active.");
                    }                    
                }
            }
		}
        if (debug)
            Debug.Log("We found " + activeMarkerIDs.Count + "active Markers");

        for (int i = 0; i < activeMarkerIDs.Count; i++){
            XmlNode node = markers[(int)activeMarkerIDs[i]];
            int originalID = int.Parse(node.Name.Substring(6));
            if (debug) { 
                Debug.Log("Original Marker ID:" + originalID);
                Debug.Log("Original marker name: " + node.Name);
            }
            GameObject originalCube = GameObject.Find(node.Name);
            
            if (originalCube != null){
                if (debug)
                    Debug.Log("Instanciating marker: " + originalID);
                GameObject newMarker = Instantiate(originalCube);

                GameObject.Find("TableMenuButtons_Apply").GetComponent<tableMenuTrigger>().increaseSavedCubesCounter();

                newMarker.name = "Marker" + (originalID + 100);
                newMarker.transform.parent = GameObject.Find("TableObject").transform;                                

                // Position
                float PosX = float.Parse(node.ChildNodes.Item(7).InnerText, System.Globalization.CultureInfo.CurrentCulture);
                float PosY = float.Parse(node.ChildNodes.Item(8).InnerText, System.Globalization.CultureInfo.CurrentCulture);
                float PosZ = float.Parse(node.ChildNodes.Item(9).InnerText, System.Globalization.CultureInfo.CurrentCulture);
                newMarker.transform.localPosition = new Vector3(PosX, PosY, PosZ);

                // Rotation
                float RotX = float.Parse(node.ChildNodes.Item(10).InnerText, System.Globalization.CultureInfo.CurrentCulture);
                float RotY = float.Parse(node.ChildNodes.Item(11).InnerText, System.Globalization.CultureInfo.CurrentCulture);
                float RotZ = float.Parse(node.ChildNodes.Item(12).InnerText, System.Globalization.CultureInfo.CurrentCulture);
                newMarker.transform.localRotation = Quaternion.Euler(new Vector3(RotX, RotY, RotZ));
                
                Transform pivot = newMarker.transform.FindChild("Pivot");
                XmlNode pivotNode = node.ChildNodes.Item(2);                
                float ScaleX = float.Parse(pivotNode.ChildNodes.Item(8).InnerText, System.Globalization.CultureInfo.CurrentCulture);
                float ScaleY = float.Parse(pivotNode.ChildNodes.Item(9).InnerText, System.Globalization.CultureInfo.CurrentCulture);
                float ScaleZ = float.Parse(pivotNode.ChildNodes.Item(10).InnerText, System.Globalization.CultureInfo.CurrentCulture);
                pivot.localScale = new Vector3(ScaleX, ScaleY, ScaleZ);
                Debug.Log("pivot scale: " + new Vector3(ScaleX, ScaleY, ScaleZ));
                pivot.gameObject.SetActive(false);
                pivot.FindChild("ScaledCube").gameObject.SetActive(true);

                // Position X_Handle
                XmlNode handleX = node.ChildNodes.Item(3);
                float handleXPosX = float.Parse(handleX.ChildNodes.Item(2).InnerText, System.Globalization.CultureInfo.CurrentCulture);
                float handleXPosY = float.Parse(handleX.ChildNodes.Item(3).InnerText, System.Globalization.CultureInfo.CurrentCulture);
                float handleXPosZ = float.Parse(handleX.ChildNodes.Item(4).InnerText, System.Globalization.CultureInfo.CurrentCulture);
                GameObject xHandle = newMarker.transform.FindChild("X_Handle").gameObject;              
                xHandle.transform.localPosition = new Vector3(handleXPosX, handleXPosY, handleXPosZ);

                // Position Y_Handle
                XmlNode handleY = node.ChildNodes.Item(4);
                float handleYPosX = float.Parse(handleY.ChildNodes.Item(2).InnerText, System.Globalization.CultureInfo.CurrentCulture);
                float handleYPosY = float.Parse(handleY.ChildNodes.Item(3).InnerText, System.Globalization.CultureInfo.CurrentCulture);
                float handleYPosZ = float.Parse(handleY.ChildNodes.Item(4).InnerText, System.Globalization.CultureInfo.CurrentCulture);
                GameObject yHandle = newMarker.transform.FindChild("Y_Handle").gameObject;                
                yHandle.transform.localPosition = new Vector3(handleYPosX, handleYPosY, handleYPosZ);                

                // Make sure the context menu is not visible
                xHandle.SetActive(false);
                yHandle.SetActive(false);
                newMarker.transform.FindChild("X_Handle").gameObject.SetActive(false);
                newMarker.transform.FindChild("CanvasTransform").gameObject.SetActive(false);

                // Enable match mode
                GameObject greenCube = newMarker.transform.FindChild("greenCube").gameObject;
                greenCube.SetActive(true);
                MatchMode matchMode = greenCube.GetComponent<MatchMode>();
                greenCube.GetComponent<BoxCollider>().enabled = false;
                matchMode.setMatchMode(true);
                matchMode.enabled = true;

                // TODO: The scale has to be saved in XML, then the following function can be used...
                float buildingScale = float.Parse(nodes[2].InnerText, System.Globalization.CultureInfo.CurrentCulture);
                Debug.Log("loaded scale: " + buildingScale);
                GameObject.FindObjectOfType<setupScene>().setGlobalBuildingScale(buildingScale);
            }
        }
    }

    public void setPath(){
        if (debug)
            Debug.Log ("Opening: "+ gameObject.name);
		openXml(gameObject.name);
	}

   

	public void openXml(String filePath){		
		String fullFilePath = projectPath + "/saves/" + filePath;

        // Read XML file
		string xmlString = System.IO.File.ReadAllText( fullFilePath );

		XmlDocument xml = new XmlDocument();
		xml.LoadXml(xmlString);
		XmlNode root = xml.FirstChild;
		XmlNodeList children = root.ChildNodes;

        // Begin crawling XML nodes
		crawlXML (children); 
	}


	void Update () {		
       


	}
}
