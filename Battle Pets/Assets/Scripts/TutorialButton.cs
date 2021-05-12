using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialButton : MonoBehaviour
{
    private bool Enabled = false;

    //Set Tutorials on at the start of the game
    private void Start()
    {
        SetTutorialsOnOff();
    }

    //Set tutorials on/off
    public void SetTutorialsOnOff()
    {
        if (Enabled) //Already ON, turn OFF
        {
            Enabled = false;
            gameObject.transform.GetChild(0).GetComponent<Text>().text = "Tutorials OFF";
        }
        else //Already ON, turn OFF
        {
            Enabled = true;
            gameObject.transform.GetChild(0).GetComponent<Text>().text = "Tutorials ON";
        }
        PetManager.ShowTutorials = Enabled;
    }
}
