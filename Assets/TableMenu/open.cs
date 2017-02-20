using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class open : MonoBehaviour {

	private String projectPath;

	void Start () {
        projectPath = Application.dataPath + "/Resources/";
    }

    //This methode crawles through all the nodes
	void crawlXML( XmlNodeList nodes ){

        XmlNodeList markers = null;
        ArrayList activeMarkerIDs = new ArrayList();
        int activeMarkerCounter = 0;
        for (int i=0; i< nodes.Count; i++){

            if (nodes[i].Name == "TableObject")
            {
                markers = nodes[i].ChildNodes; //We now have every marker node in the XmlNodeList called Marker

                for (int j = 0; j < markers.Count -11; j++) // We don't need the last 11 items ( this are the position/scale/rot. from the TableObject node.)
                {
                    if(markers.Item(j).ChildNodes.Item(16).InnerText == "True")
                    {
                        activeMarkerIDs.Add(j);
                        Debug.Log("Marker is saved as active.");
                    }
                }
            }
		}
        Debug.Log("We found " + activeMarkerIDs.Count + "active Markers");
        for (int i = 0; i < activeMarkerIDs.Count; i++)
        {
           
            XmlNode node = markers[(int)activeMarkerIDs[i]];
            int originalID = int.Parse(node.Name.Substring(6));
            Debug.Log("Original Marker ID:" + originalID);
            Debug.Log("Original marker name: " + node.Name);
            GameObject originalCube = GameObject.Find(node.Name);  //TODO: Hier stimmt etwas nicht. Der originale Cube wird nicht gefunden (er ist immer null).
            if (originalCube != null)                              //Daher wird der Cube unten noch nicht richtig instanziiert. Obwohl der Name vom Marker korrekt ist.
            {
                Debug.Log("Instanciating marker: " + originalID);
                GameObject newMarker = Instantiate(originalCube); 
                newMarker.name = "Marker" + originalID + 100;
                newMarker.transform.parent = GameObject.Find("TableObject").transform;
            }

        }
    }

    public void setPath(){
			Debug.Log ("Opening: "+ gameObject.name);
			openXml(gameObject.name);
	}

	public void openXml(String filePath){
		
		String fullFilePath = projectPath + "/saves/" + filePath;

        //Reading the XML file
		string xmlString = System.IO.File.ReadAllText( fullFilePath );

		XmlDocument xml = new XmlDocument();
		xml.LoadXml(xmlString);
		XmlNode root = xml.FirstChild;
		XmlNodeList children = root.ChildNodes;

        //begin to crawl the xml nodes
		crawlXML (children); 

	}


	void Update () {
		
	}
}
