using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreationManager : MonoBehaviour
{
    //Variables
    public Elements element1 = Elements.NULL, element2 = Elements.NULL;
    public List<OriginActor> OriginPets;
    private short CurrentSelector = 1; //1 = element1, -1 = element2
    private GameObject Selector1, Selector2, CreationPreview, CreateButton, ContinueButton;
    private bool SlotsFilled = false;

    //Start function
    private void Start()
    {
        //Define private references and set settings
        Selector1 = GameObject.Find("Selector1");
        Selector2 = GameObject.Find("Selector2");
        CreationPreview = GameObject.Find("Creation Preview");
        CreateButton = GameObject.Find("BtnCreate");
        ContinueButton = GameObject.Find("BtnContinue");

        ClearPreview();
    }

    //Clear the preview
    public void ClearPreview()
    {
        element1 = Elements.NULL;
        element2 = Elements.NULL;
        CurrentSelector = 1;
        Selector1.GetComponent<SpriteRenderer>().color = Color.clear;
        Selector2.GetComponent<SpriteRenderer>().color = Color.clear;
        CreationPreview.GetComponent<SpriteRenderer>().sprite = null;
        CreateButton.SetActive(false);
        ContinueButton.SetActive(SlotsFilled);
    }

    //Get the element from a button and acto accordingly
    public void GetElementFromButton(GameObject button)
    {
        Elements element = button.GetComponent<ElementButton>().Element;
        //Return if not enough elements are present or all slots are already filled
        if (element == element1 || element == element2 || SlotsFilled)
        {
            return;
        }

        switch (CurrentSelector)
        {
            case 1:
                element1 = element;
                Selector1.transform.parent = button.transform;
                DrawSelector(Selector1);
                break;
            case -1:
                element2 = element;
                Selector2.transform.parent = button.transform;
                DrawSelector(Selector2);
                break;
        }

        CurrentSelector *= -1;
        CreatePreview();
    }

    //Draw the selector
    private void DrawSelector(GameObject Selector)
    {
        Selector.transform.position = Selector.transform.parent.position - new Vector3(0, 0, 1);
        Selector.GetComponent<SpriteRenderer>().color = Color.white;
    }

    //Create preview from elements
    public void CreatePreview()
    {
        if (element1 == Elements.NULL || element2 == Elements.NULL)
        {
            return;
        }

        switch (element1)
        {
            case Elements.Water:
                switch (element2)
                {
                    case Elements.Earth:
                        PreviewSetup(OriginPets[4]); //Fish
                        break;
                    case Elements.Fire:
                        PreviewSetup(OriginPets[0]); //Cat
                        break;
                    case Elements.Air:
                        PreviewSetup(OriginPets[5]); //Seagull
                        break;
                }
                break;
            case Elements.Earth:
                switch (element2)
                {
                    case Elements.Water:
                        PreviewSetup(OriginPets[4]); //Fish
                        break;
                    case Elements.Fire:
                        PreviewSetup(OriginPets[2]); //bear
                        break;
                    case Elements.Air:
                        PreviewSetup(OriginPets[1]); //Bee
                        break;
                }
                break;
            case Elements.Fire:
                switch (element2)
                {
                    case Elements.Water:
                        PreviewSetup(OriginPets[0]); //Cat
                        break;
                    case Elements.Earth:
                        PreviewSetup(OriginPets[2]); //bear
                        break;
                    case Elements.Air:
                        PreviewSetup(OriginPets[3]);//Beetle
                        break;
                }
                break;
            case Elements.Air:
                switch (element2)
                {
                    case Elements.Water:
                        PreviewSetup(OriginPets[5]); //Seagull
                        break;
                    case Elements.Earth:
                        PreviewSetup(OriginPets[1]);//Bee
                        break;
                    case Elements.Fire:
                        PreviewSetup(OriginPets[3]);//Beetle
                        break;
                }
                break;
        }
    }

    //Setup the preview
    private void PreviewSetup(OriginActor origin)
    {
        CreationPreview.GetComponent<SpriteRenderer>().sprite = origin.Sprite;
        CreateButton.SetActive(true);
        CreateButton.GetComponent<CreateButton>().OriginToCreate = origin;
    }

    //Set the pet slots
    public void SetSlots()
    {
        for (int i = 0; i < PetManager.Pets.Count; i++)
        {
            SpriteRenderer PetSlotSprite = GameObject.Find("PetSlotContainer" + i.ToString()).transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
            PetSlotSprite.sprite = PetManager.Pets[i].Sprite;
            if (i == 4)
            {
                SlotsFilled = true;
            }
        }
    }
}

//Elements enum
public enum Elements { Water, Earth, Fire, Air, NULL }