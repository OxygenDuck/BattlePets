using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    //Setup the globals of the game before playing
    private void Start()
    {
        PetManager.GameState = GameState.PLAYING;
        PetManager.Pets.Clear();
        PetManager.Enemies.Clear();
        PetManager.Level = 0;
        PetManager.FirstPetFallen = false;
    }

    //Go to the first Scene
    public void Play()
    {
        SceneManager.LoadScene("CreationScreen");
    }
}
