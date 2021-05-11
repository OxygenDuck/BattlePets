using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    //Get References
    public GameObject DialogueBox;
    private GameObject ButtonContainer;
    private Text DialogueText, Option1Text, Option2Text;
    private Button Button1, Button2, ButtonConfirm;
    private BattleController BattleController;

    public void Awake()
    {
        DialogueText = DialogueBox.transform.GetChild(0).GetComponent<Text>();
        ButtonContainer = DialogueBox.transform.GetChild(1).gameObject;
        Button1 = ButtonContainer.transform.GetChild(0).GetComponent<Button>();
        Button2 = ButtonContainer.transform.GetChild(1).GetComponent<Button>();
        ButtonConfirm = ButtonContainer.transform.GetChild(2).GetComponent<Button>();
        Option1Text = Button1.transform.GetChild(0).GetComponent<Text>();
        Option2Text = Button2.transform.GetChild(0).GetComponent<Text>();
        BattleController = gameObject.GetComponent<BattleController>();
    }

    //Set Variables
    public DialogueState DialogueState = DialogueState.DISPLAY;
    private short SelectedOption = 0; //0 = Nothing Selected, 1 = Option 1, 2= Option 2

    //Functions
    private void Start()
    {
        Debug.Log(DialogueText.text);
        Debug.Log(Option1Text.text);
        Debug.Log(Option2Text.text);
    }

    public void SelectOption1()
    {
        SelectedOption = 1;
        Button1.image.color = Color.green;
        Button2.image.color = Color.white;
        ButtonConfirm.interactable = true;
    }
    public void SelectOption2()
    {
        SelectedOption = 2;
        Button2.image.color = Color.green;
        Button1.image.color = Color.white;
        ButtonConfirm.interactable = true;
    }

    public void ConfirmSelection()
    {
        switch (BattleController.BattleState)
        {
            case BattleState.PLAYERSELECTMOVE:
                BattleController.SelectPlayerMove(SelectedOption);
                break;
            case BattleState.PLAYERSELECTTARGET:
                BattleController.SetPlayerTarget(SelectedOption);
                break;
            default:
                break;
        }
        ButtonConfirm.interactable = false;
        SelectedOption = 0;

        Button1.image.color = Color.white;
        Button2.image.color = Color.white;
    }

    //Set text values
    public void SetText(string Text)
    {
        DialogueText.text = Text;
        //TODO: Animate
    }

    public void SetOption1Text(string Text)
    {
        Option1Text.text = Text;
    }

    public void SetOption2Text(string Text)
    {
        Option2Text.text = Text;
    }
}

public enum DialogueState { ACTION, TARGET, DISPLAY }
