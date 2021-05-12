using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectPetButton : MonoBehaviour
{
    //Variables
    public int index = 0; //Index in PetManager.Pets
    public bool Selected = false;

    public void SelectPet()
    {
        GameObject.Find("BattleSceneController").GetComponent<BattleController>().SelectNewPet(index);
    }

    public void ConfirmSelected()
    {
        GameObject.Find("BattleSceneController").GetComponent<BattleController>().ConfirmSelectedPet();
    }
}
