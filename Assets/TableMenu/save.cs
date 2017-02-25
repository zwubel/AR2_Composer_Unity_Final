using UnityEngine;
using System;
using System.Xml;
using System.IO;

public class save : MonoBehaviour {

	public XmlDocument doc; // This is the XML document
	private String timeStamp;  // We will save date and time
	string filepath; 
	public XmlNode root; //Root node of the XML document
    public bool debug = false;

	// Use this for initialization
	void Start () {
		
	}

	public void saveScene	(){
	    /*
         *First of all we take the current system date and time, this will be used in the XML document name. We will also have 
         *have a node at the beginning where date and time are saved.
        */

		String timeStamp = System.DateTime.Now.ToString();
		timeStamp = timeStamp.Replace("/", "-");
		timeStamp = timeStamp.Replace(":", "-");        

        filepath  = Application.dataPath + "/Resources/saves/" + timeStamp +".xml";
        if (debug){
            Debug.Log("TimeStamp: " + timeStamp);
            Debug.Log("Path: " + filepath);
        }
		doc = new XmlDocument();
		doc.LoadXml("<AR2COMPOSER_SCENE>" +
					"<time>" +  
						timeStamp  +
					"</time>" +
					"</AR2COMPOSER_SCENE>"); 
		root = doc.DocumentElement;

		Console.WriteLine(doc.OuterXml);
        GameObject tableObject = GameObject.Find("TableObject"); //We start with the "TableObject"...
        traverseHirarchy(tableObject, root); //.. and recursive traverse all the childs

        XmlNode globalBuildingScale = doc.CreateNode("element", "globalBuildingScale", "");
        globalBuildingScale.InnerText = GameObject.Find("SetupScene").GetComponent<setupScene>().globalBuildingScale.ToString();
        root.AppendChild(globalBuildingScale);
        if (debug) { 
            Debug.Log ("Objects crawled!");
		    Debug.Log ( "Saving at path: " + filepath );
        }
        doc.Save ( filepath );
	}

	void traverseHirarchy(GameObject obj, XmlNode parentNode){

        if (!obj.name.Contains("TablePlane"))
        {

            //We start creating the XML nodes here. 
            XmlNode newElem = doc.CreateNode("element", obj.name, ""); 

            XmlNode newName = doc.CreateNode("element", "Name", "");  //Name

            XmlNode newPosX = doc.CreateNode("element", "PositionX", ""); //Positions
            XmlNode newPosY = doc.CreateNode("element", "PositionY", "");
            XmlNode newPosZ = doc.CreateNode("element", "PositionZ", "");

            XmlNode newRotationX = doc.CreateNode("element", "RotationX", ""); //Rotations
            XmlNode newRotationY = doc.CreateNode("element", "RotationY", "");
            XmlNode newRotationZ = doc.CreateNode("element", "RotationZ", "");

            XmlNode newScaleX = doc.CreateNode("element", "ScaleX", ""); //Scales
            XmlNode newScaleY = doc.CreateNode("element", "ScaleY", "");
            XmlNode newScaleZ = doc.CreateNode("element", "ScaleZ", "");

            XmlNode active = doc.CreateNode("element", "Active", ""); //Active or not?

            //Now we can save the informations of the cubes in the innerText of the XML nodes
            newName.InnerText = obj.name;
            newPosX.InnerText = obj.transform.localPosition.x.ToString();
            newPosY.InnerText = obj.transform.localPosition.y.ToString();
            newPosZ.InnerText = obj.transform.localPosition.z.ToString();

            newRotationX.InnerText = obj.transform.localEulerAngles.x.ToString();
            newRotationY.InnerText = obj.transform.localEulerAngles.y.ToString();
            newRotationZ.InnerText = obj.transform.localEulerAngles.z.ToString();

            newScaleX.InnerText = obj.transform.localScale.x.ToString();
            newScaleY.InnerText = obj.transform.localScale.y.ToString();
            newScaleZ.InnerText = obj.transform.localScale.z.ToString();

            active.InnerText = obj.activeSelf.ToString();

            // Recursion
            foreach (Transform child in obj.transform){
                if (!obj.name.Contains("TablePlane"))
                    traverseHirarchy(child.gameObject, newElem);
            }

            // Here append the childs to the root node and set the parent node to the root node to the given parent node of the function
            newElem.AppendChild(newName);

            newElem.AppendChild(newPosX);
            newElem.AppendChild(newPosY);
            newElem.AppendChild(newPosZ);

            newElem.AppendChild(newRotationX);
            newElem.AppendChild(newRotationY);
            newElem.AppendChild(newRotationZ);

            newElem.AppendChild(newScaleX);
            newElem.AppendChild(newScaleY);
            newElem.AppendChild(newScaleZ);

            newElem.AppendChild(active);

            parentNode.AppendChild(newElem);
        }
	}

    // Update is called once per fram
    void Update() { }
}