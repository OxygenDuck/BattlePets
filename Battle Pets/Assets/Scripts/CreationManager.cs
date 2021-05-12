using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreationManager : MonoBehaviour
{
    //Variables
    public Elements element1 = Elements.NULL, element2 = Elements.NULL;
    public TutorialCanvas Tutorial;
    public List<OriginActor> OriginPets;
    private short CurrentSelector = 1; //1 = element1, -1 = element2
    private GameObject Selector1, Selector2, CreationPreview, CreateButton, ContinueButton;
    private bool SlotsFilled = false;
    public Text StatPreview;

    //Start function
    private void Start()
    {
        //Define private references and set settings
        Selector1 = GameObject.Find("Selector1");
        Selector2 = GameObject.Find("Selector2");
        CreationPreview = GameObject.Find("Creation Preview");
        CreateButton = GameObject.Find("BtnCreate");
        ContinueButton = GameObject.Find("BtnContinue");

        //Set the first tutorial
        if (PetManager.ShowTutorials)
        {
            Tutorial.gameObject.SetActive(true);
            switch (PetManager.Level)
            {
                case 0:
                    Tutorial.SetText("An army of bugs has risen and is threatening to conquer the world! There has to be something we can do against it!" +
                "\nLuckily we have a secret weapon, we can create different pets to fight off the insects! It is up to you to guide these pets into battle." +
                "\n\nSelect any combination of two elements to create a pet. There are 6 total different combinations." +
                "\n\nGood Luck!");
                    break;
                case 1:
                    Tutorial.SetText("We have some spare time now that the first wave has been fought." +
                        "\nYour pets have been fully healed, and if you lost any in the previous battle you can create new ones." +
                        "\nPress the \"Continue\" button when you're ready");
                    break;
                default:
                    Tutorial.Continue();
                    break;
            }
        }
        else
        {
            Tutorial.Continue();
        }

        SetSlots();
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
        StatPreview.text = "";
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
        StatPreview.text = "" +
            origin.Name +
            "\nATTACK: " + origin.Attack.ToString() +
            "\nDEFENSE: " + origin.Defense.ToString() +
            "\nSPEED: " + origin.Speed.ToString() +
            "\nSPECIAL MOVE: " + origin.SpecialMove.ToString();
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

        //Set a tutorial
        if (PetManager.Pets.Count == 1 && PetManager.ShowTutorials && PetManager.Level == 0)
        {
            Tutorial.gameObject.SetActive(true);
            Tutorial.SetText("" +
                "You have succesfully created a pet! You can see which pets you have created on the right side of the screen." +
                "\n\nYou need to create 4 more before we can head into battle! Try to get a good variety to help you against different enemies." +
                "\nOnce you are done press the \"Continue\" button that will shwo up on the bottom right of the screen.");
        }
    }
}

//Elements enum
public enum Elements { Water, Earth, Fire, Air, NULL }