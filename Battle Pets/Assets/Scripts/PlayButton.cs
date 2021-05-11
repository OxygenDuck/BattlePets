using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    //Go to the first Scene
    public void Play()
    {
        SceneManager.LoadScene("CreationScreen");
    }
}
