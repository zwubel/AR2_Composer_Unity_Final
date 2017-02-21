using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class performAction : MonoBehaviour {

    [Header("Dependencies")]
    public Slider scaleSlider;

    public void setState(int state){
        FindObjectOfType<setupScene>().setState(state);
    }    

    public void noCalibration(){
        if (FindObjectOfType<readInNetworkData>().getSocketReady())
            FindObjectOfType<setupScene>().noCalibration();
        else {
            FindObjectOfType<sceneLoad>().Close("CalibrateOrNot");
            FindObjectOfType<sceneLoad>().Open("SocketNotReady");
        }
    }

    public void setGlobalBuildingScale(){        
        FindObjectOfType<setupScene>().setGlobalBuildingScale(1f/scaleSlider.value);
    }

    public void setRecalibrationBreak(){
        FindObjectOfType<readInNetworkData>().sendTCPstatus((int)readInNetworkData.TCPstatus.reCalib);
    }
}
