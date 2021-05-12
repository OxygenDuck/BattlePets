using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialCanvas : MonoBehaviour
{
    public Text TutorialText;

    // Start is called before the first frame update
    void Start()
    {
        //Hide tutorial canvas if tutorials are turned off
        if (!PetManager.ShowTutorials)
        {
            Continue();
        }   
    }

    //Set the tutorial text
    public void SetText(string text)
    {
        TutorialText.text = text;
    }

    //Continue the tutorial.. just hide the whole canvas lol
    public void Continue()
    {
        gameObject.SetActive(false);
    }
}
