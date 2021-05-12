using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndScreenButton : MonoBehaviour
{
    public Text EndText;
    AudioSource Audio;

    //Check what to do on the endscreen at the start
    private void Start()
    {
        Audio = GameObject.Find("AudioManager").GetComponent<AudioSource>();
        //Get the pets as a string
        string PetString = "";
        for (int i = 0; i < PetManager.Pets.Count; i++)
        {
            PetString += PetManager.Pets[i].Name;
            switch (i)
            {
                case 3:
                    PetString += " and ";
                    break;
                case 4:
                    //Do nothing
                    break;
                default:
                    PetString += ", ";
                    break;
            }
        }

        switch (PetManager.GameState)
        {
            case GameState.PLAYING:
                SetEndText("Whoa this isnt supposed to happen, The game should still be playing. Oops");
                break;
            case GameState.WON:
                Audio.Play();
                SetEndText("Well done! You've beaten the bug army and saved the world with your pets. You have done it together with " + PetString + "!");
                break;
            case GameState.LOST:
                SetEndText("Oh dear, the bug army has overwelmed you and has succesfully taken over the world. Your final pets were " + PetString + ".");
                break;
        }
    }

    //Set the text on the endscreen
    private void SetEndText(string Message)
    {
        EndText.text = Message;
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("TitleScreen");
    }
}
