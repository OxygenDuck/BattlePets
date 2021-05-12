using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateButton : MonoBehaviour
{
    public OriginActor OriginToCreate;

    //Create a pet
    public void CreatePet()
    {
        PetManager.AddPet(OriginToCreate);
        CreationManager cm = GameObject.Find("CreationManager").GetComponent<CreationManager>();
        cm.SetSlots();
        cm.ClearPreview();
    }

    //Continue to next scene
    public void Continue()
    {
        SceneManager.LoadScene("BattleScene");
    }
}
