using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class open : MonoBehaviour {

	public GameObject markerMaster;
	private String projectPath;

	void Start () {

        projectPath = Application.dataPath + "/Resources/";


    }

    //This methode crawles through all the nodes
	void crawlXML( XmlNodeList nodes ){
		for(int i=0; i< nodes.Count; i++){

            String newCubesName = "";

            if (nodes [i].Name.Contains ("Marker")) {

                GameObject newCube = Instantiate(markerMaster); //Every cube is an instance of the markerMaster cube

				newCube.transform.parent = GameObject.Find ("TableObject").transform;
                newCube.transform.parent.gameObject.SetActive(true);

                //Initializing the variables
				float PosX=0.0f;
				float PosY=0.0f; 
				float PosZ=0.0f;

				float RotX = 0.0f;
				float RotY = 0.0f;
				float RotZ = 0.0f;

				float ScaleX = 0.0f;
				float ScaleY = 0.0f;
				float ScaleZ = 0.0f;

                bool setActive = false;

                //We check every name of the given XML nodes, and define what to do in the different cases..
				foreach (XmlNode node in nodes[i]) {

                    if (node.Name == "Name")
                    {
                        newCubesName = node.InnerText;
                    }

                    if (node.Name == "PositionX") {
						PosX = float.Parse (node.InnerText,System.Globalization.CultureInfo.CurrentCulture); 
					}
					if (node.Name == "PositionY") {
						PosY= float.Parse (node.InnerText,System.Globalization.CultureInfo.InvariantCulture); 
					}
					if (node.Name == "PositionZ") {
						PosZ = float.Parse (node.InnerText,System.Globalization.CultureInfo.InvariantCulture);
					}

					if (node.Name == "RotationX") {
						RotX = float.Parse (node.InnerText,System.Globalization.CultureInfo.InvariantCulture); 
					}
					if (node.Name == "RotationY") {
						RotY= float.Parse (node.InnerText,System.Globalization.CultureInfo.InvariantCulture); 
					}
					if (node.Name == "RotationZ") {
						RotZ = float.Parse (node.InnerText,System.Globalization.CultureInfo.InvariantCulture);
					}

					if (node.Name == "ScaleX") {
						ScaleX = float.Parse (node.InnerText,System.Globalization.CultureInfo.InvariantCulture); 
						if (ScaleX == 0) ScaleX = 1;
					}
					if (node.Name == "ScaleY") {
						ScaleY = float.Parse (node.InnerText,System.Globalization.CultureInfo.InvariantCulture); 
						if (ScaleY == 0) ScaleX = 1;
					}
					if (node.Name == "ScaleZ") {
						ScaleZ = float.Parse (node.InnerText,System.Globalization.CultureInfo.InvariantCulture);
						if (ScaleZ == 0) ScaleX = 1;
					}

                    //We check if the cube hase been active or not, while we have a fixed-array size this is important
                    //because we save the whole array of cubes, if they are active or not
                    if (node.Name == "Active")
                    {
                        if (node.InnerText == "true")
                        {
                            setActive = true;
                        }
                        else
                            setActive = false;
                    }

                    newCube.name = newCubesName;

					newCube.transform.localPosition = (new Vector3(PosX,PosY,PosZ));
                    newCube.transform.localEulerAngles = (new Vector3(RotX, RotY, RotZ));
                    newCube.transform.localScale = (new Vector3 (ScaleX, ScaleY, ScaleZ));
                    newCube.SetActive(setActive);
                    newCube.transform.FindChild("Pivot").transform.FindChild("MatchModePlane").GetComponent<MatchMode>().matchMode = true;
                }
			}
			crawlXML(nodes [i].ChildNodes);
		}
	}

	public void setPath(){

			Debug.Log ("Opening: "+ this.gameObject.name);
			openXml (this.gameObject.name);
	}

	public void openXml(String filePath){
		Debug.Log ("Distroying current active markers..");
		GameObject tableObject = GameObject.Find ("TableObject");
		foreach (Transform child in tableObject.transform)
		{
			if (child.name.Contains ("Marker")) {
				Destroy (child.gameObject);
			}
		}
		Debug.Log ("..done.");

		String fullFilePath = projectPath + "/saves/" + filePath;
		Debug.Log ( "Scene saved at:" + fullFilePath );

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
