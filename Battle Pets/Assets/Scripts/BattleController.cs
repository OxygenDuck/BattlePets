using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleController : MonoBehaviour
{
    //Set References
    public TutorialCanvas Tutorial;
    private bool FirstTurnCompleted = false;
    public BattleActor PetActor1, PetActor2, EnemyActor1, EnemyActor2;
    public OriginActor TestActor;
    private short ActiveActor = 0; //0 = first pet, 1 = second pet, 2 = first enemy, 3 = second enemy
    public BattleState BattleState = BattleState.ENDTURN;
    private DialogueController dialogueController;
    private ActorAction CurrentActorAction;
    private GameObject PetSelectCursor, SelectPetCanvas;
    private Text LevelDisplay;

    //Set up variables
#pragma warning disable IDE0044 // Add readonly modifier
    SpeedCalculation[] Speeds = new SpeedCalculation[4];
#pragma warning restore IDE0044 // Add readonly modifier
    public List<OriginActor> Level1, Level1Boss, Level2, Level2Boss, Level3, Level3Boss;

    //Start Function
    void Start()
    {
        dialogueController = gameObject.GetComponent<DialogueController>();

        //For debugging, set a standard array of pets if none are present
        if (PetManager.Pets.Count == 0)
        {
            for (int i = 0; i < 5; i++)
            {
                PetManager.AddPet(TestActor);
            }
        }

        //Set the level Display
        LevelDisplay = GameObject.Find("TxtLevelDisplay").GetComponent<Text>();
        LevelDisplay.text = "Level " + (PetManager.Level + 1).ToString();

        //Get Player Pets
        PetActor1.SetActor(PetManager.Pets[0]);
        PetActor2.SetActor(PetManager.Pets[1]);

        //Get Enemy Monsters
        SetEnemies();
        EnemyActor1.SetActor(PetManager.Enemies[0]);
        EnemyActor2.SetActor(PetManager.Enemies[1]);

        //Get private references
        SelectPetCanvas = GameObject.Find("Select Pet Canvas");
        PetSelectCursor = GameObject.Find("SprSelector");
        SetupPetSelectCanvas();

        //Set Active Actors
        PetManager.Pets[0].State = ActorState.ACTIVE;
        PetManager.Pets[1].State = ActorState.ACTIVE;
        PetManager.Enemies[0].State = ActorState.ACTIVE;
        PetManager.Enemies[1].State = ActorState.ACTIVE;

        //Prepare for Player Input
        BattleState = BattleState.PLAYERSELECTMOVE;
        NextAction();

        //Check if first turn tutorial has to be played
        if (PetManager.ShowTutorials && PetManager.Level == 0)
        {
            Tutorial.gameObject.SetActive(true);
            Tutorial.SetText("" +
                "This is it, your first foray into battle. Your pets shall in turn select their move and then their target, your enemies will do the same." +
                "\n\nTo select a Move, press it's corresponding button and click \"Confirm\", afterwards you can select your target in the same manner." +
                "\nAttack the enemies to dwindle down their health and defeat them!");
        }
        else
        {
            Tutorial.Continue();
        }
    }

    //Set the enemies
    private void SetEnemies()
    {
        switch (PetManager.Level)
        {
            case 0: //Level 1
                PetManager.SetEnemies(Level1, Level1Boss); break;
            case 1: //Level 2
                PetManager.SetEnemies(Level2, Level2Boss); break;
            case 2: //Level 3
                PetManager.SetEnemies(Level3, Level3Boss); break;
        }
    }

    //Perform action
    private void NextAction()
    {
        switch (ActiveActor)
        {
            case 0:
                if (BattleState == BattleState.PLAYERSELECTMOVE)
                {
                    AskPlayerMove();
                }
                else
                {
                    ActiveActor++;
                    AskPlayerMove();
                }
                break;
            case 1:
                ActiveActor++;
                EnemySelectMove();
                break;
            case 2:
                ActiveActor++;
                EnemySelectMove();
                break;
            case 3:
                CalculateQueue();
                StartCoroutine(PlayTurn());
                break;
            default:
                break;
        }
    }

    //Queue Order Calculation
    private void CalculateQueue()
    {
        System.Array.Sort(Speeds, (y, x) => x.Speed.CompareTo(y.Speed));
        foreach (var item in Speeds)
        {
            Debug.Log(item.Actor.Actor.Name + ", Speed = " + item.Speed.ToString());
        }
    }

    //Play out the turn
    //Use coroutine to wait until everything is displayed per turn
    IEnumerator PlayTurn()
    {
        //Loop through the speeds and play in order
        foreach (SpeedCalculation Speed in Speeds)
        {
            //Skip move if the user has been defeated
            if (Speed.Actor.Actor.State != ActorState.ACTIVE)
            {
                continue;
            }
            Debug.Log(Speed.Action.move.ToString());
            BattleActor Targeted = GetActionTarget(Speed.Action);

            dialogueController.TypeText(Speed.Actor.Actor.Name + " used " + Speed.Action.move.ToString() + "!");

            //Speed.Action.move.UseSelectedMove();
            switch (Speed.Action.move)
            {
                case MoveEnum.ATTACK:
                    Speed.Actor.Attack(Targeted);
                    break;
                case MoveEnum.HEAL:
                    Speed.Actor.Heal(Targeted);
                    break;
                case MoveEnum.BERSERK:
                    BattleActor[] Targets = { EnemyActor1, EnemyActor2 };
                    if (Targeted == PetActor1 || Targeted == PetActor2)
                    {
                        Targets = new[] { PetActor1, PetActor2 };
                    }
                    Speed.Actor.Berserk(Targets);
                    break;
                default: //Nothing happens lol
                    break;
            }

            //Wait and show player results of this action
            yield return new WaitForSeconds(3);
        }

        //Clear the speeds
        System.Array.Clear(Speeds, 0, Speeds.Length);
        ActiveActor = 0;

        //Set the Next players
        if (PetActor1.Actor.State == ActorState.DEFEATED || PetActor2.Actor.State == ActorState.DEFEATED)
        {
            OpenSelectCanvas();
        }
        else
        {
            ReplaceEnemies();
        }
    }

    //Add the next enemies
    private void ReplaceEnemies()
    {
        //Check for Battle Win Condition
        int EnemiesDefeated = 0;
        foreach (Actor Enemy in PetManager.Enemies)
        {
            if (Enemy.State == ActorState.ACTIVE) //If enemy is active, skip it
            {
                continue;
            }
            if (Enemy.State == ActorState.DEFEATED) //If enemy is defeated, add to counter and skip it
            {
                EnemiesDefeated++;
                continue;
            }
            if (EnemyActor1.Actor.State == ActorState.DEFEATED) //Replace actor 1
            {
                EnemyActor1.SetActor(Enemy);
                Enemy.State = ActorState.ACTIVE;
                EnemyActor1.UpdateUI();
                continue;
            }
            if (EnemyActor2.Actor.State == ActorState.DEFEATED) //Replace actor 2
            {
                EnemyActor2.SetActor(Enemy);
                Enemy.State = ActorState.ACTIVE;
                EnemyActor2.UpdateUI();
                break;
            }
        }

        //Check if boss defeated
        bool BossesDefeated = true;
        foreach (Actor Boss in PetManager.Bosses)
        {
            if (Boss.State != ActorState.DEFEATED)
            {
                BossesDefeated = false;
                break;
            }
        }

        if (BossesDefeated)
        {
            //Win condition
            StartCoroutine(WinBattle());
            return;
        }
        else if (EnemiesDefeated == PetManager.Enemies.Count)
        {
            if (PetManager.Bosses[0].State == ActorState.INACTIVE && PetManager.ShowTutorials && PetManager.Level == 0)
            {
                Tutorial.gameObject.SetActive(true);
                Tutorial.SetText("" +
                    "The bosses have shown up! If you defeat them the army will temporarily retreat, and we will have time to prepare for the next attack!");
            }
            EnemyActor1.SetActor(PetManager.Bosses[0]);
            if (PetManager.Bosses.Count == 2)
            {
                EnemyActor2.SetActor(PetManager.Bosses[1]);
            }
        }

        //Check to see if turn 2 tutorial has to be played
        if (!FirstTurnCompleted && PetManager.ShowTutorials && PetManager.Level == 0)
        {
            Tutorial.gameObject.SetActive(true);
            Tutorial.SetText("" +
                "Now that you have played the first round, you get to select your actions again. You might have noticed that targeting an enemy who was defeated earlier in the turn will result in no damage being delt, so keep that in mind");
            FirstTurnCompleted = true;
        }

        //Setup the next turn
        BattleState = BattleState.PLAYERSELECTMOVE;
        NextAction();
    }

    //Win the battle
    IEnumerator WinBattle()
    {
        //Set text to show battle win
        dialogueController.TypeText("You won this battle!");

        //Linger a little
        yield return new WaitForSeconds(3);

        if (PetManager.Level == 2) //End of the final level
        {
            PetManager.GameState = GameState.WON;
            SceneManager.LoadScene("EndScreen");
            yield break;
        }

        //Setup the next level
        //Remove defeated Pets
        for (int i = PetManager.Pets.Count - 1; i >= 0; i--)
        {
            if (PetManager.Pets[i].State == ActorState.DEFEATED)
            {
                PetManager.Pets.RemoveAt(i);
            }
            else //Heal
            {
                PetManager.Pets[i].Hp = PetManager.Pets[i].MaxHp;
            }
        }

        //Clear enemies
        PetManager.Enemies.Clear();

        //Advance level
        PetManager.Level++;

        //Go back to Pet Creation Screen
        SceneManager.LoadScene("CreationScreen");
    }

    //Setup Pet Selection Canvas
    private void SetupPetSelectCanvas()
    {
        for (int i = 0; i < PetManager.Pets.Count; i++)
        {
            GameObject Button = GameObject.Find("btnPet" + i.ToString());
            Button.transform.GetChild(0).GetComponent<Image>().sprite = PetManager.Pets[i].Sprite;
        }
        SelectPetCanvas.SetActive(false);

        //Check if Fallen Pet tutorial has to be played
        if (PetManager.ShowTutorials && !PetManager.FirstPetFallen)
        {
            Tutorial.gameObject.SetActive(true);
            Tutorial.SetText("" +
                "Oh dear, it seems one of your pets has fallen. Luckily we now have the time to send out another." +
                "\n\nTo select a pet, click on the one you want and an indicator will show above it. Note that Pets who are already in the field cannot be selected.");
            PetManager.FirstPetFallen = true;
        }
    }

    //Open Select Pet canvas
    private void OpenSelectCanvas()
    {
        //Set variables and activate the canvas
        int PetsDefeated = 0;
        bool PetSelected = false;
        SelectPetCanvas.SetActive(true);

        //Get the fainted pet
        if (PetActor1.Actor.State == ActorState.DEFEATED) //Actor 1
        {
            GameObject.Find("TxtSelectPet").GetComponent<Text>().text = "Who do you want to replace " + PetActor1.Actor.Name + "?";
        }
        else //Actor 2
        {
            GameObject.Find("TxtSelectPet").GetComponent<Text>().text = "Who do you want to replace " + PetActor2.Actor.Name + "?";
        }

        for (int i = 0; i < PetManager.Pets.Count; i++)
        {
            switch (PetManager.Pets[i].State)
            {
                case ActorState.ACTIVE: //Cant switch in an already active pet
                    GameObject.Find("btnPet" + i.ToString()).GetComponent<Button>().interactable = false;
                    break;
                case ActorState.INACTIVE:
                    if (!PetSelected)
                    {
                        SelectNewPet(i);
                        PetSelected = true;
                    }
                    GameObject.Find("btnPet" + i.ToString()).GetComponent<Button>().interactable = true;
                    break;
                case ActorState.DEFEATED: //Defeated actor cant battle
                    GameObject.Find("btnPet" + i.ToString()).GetComponent<Button>().interactable = false;
                    PetsDefeated++;
                    if (PetsDefeated == PetManager.Pets.Count) //All actors defeated = lost
                    {
                        //Lose Condition
                        PetManager.GameState = GameState.LOST;
                        SceneManager.LoadScene("EndScreen");
                        return;
                    }
                    break;
            }

            //Check if everything has been checked, no lose condition has been met and nothing was selected
            if (i == PetManager.Pets.Count - 1 && PetsDefeated != PetManager.Pets.Count && !PetSelected)
            {
                //Skip to replace enemies
                ReplaceEnemies();
                SelectPetCanvas.SetActive(false);
            }
        }
    }

    //Select new pet on Pet selector canvas
    public void SelectNewPet(int index)
    {
        //Deselect every button
        for (int i = 0; i < 5; i++)
        {
            GameObject.Find("btnPet" + i.ToString()).GetComponent<SelectPetButton>().Selected = false;
        }

        //Now select the given button
        GameObject ButtonToSelect = GameObject.Find("btnPet" + index.ToString());
        ButtonToSelect.GetComponent<SelectPetButton>().Selected = true;

        //Set the cursor to indicate selected
        PetSelectCursor.transform.localPosition = new Vector3(-420 + index * 210, 150, 0);
    }

    //Confirm the selected Pet
    public void ConfirmSelectedPet()
    {
        BattleActor ActorToReplace;
        //Get the fainted pet
        if (PetActor1.Actor.State == ActorState.DEFEATED) //Actor 1
        {
            ActorToReplace = PetActor1;
        }
        else //Actor 2
        {
            ActorToReplace = PetActor2;
        }

        //Set the selected actor
        for (int i = 0; i < 5; i++)
        {
            if (GameObject.Find("btnPet" + i.ToString()).GetComponent<SelectPetButton>().Selected == true)
            {
                ActorToReplace.SetActor(PetManager.Pets[i]);
            }
        }
        SelectPetCanvas.SetActive(false);

        //Check if there are any other fainted pets
        if (PetActor2.Actor.State == ActorState.DEFEATED)
        {
            OpenSelectCanvas();
        }
        else
        {
            ReplaceEnemies();
        }
    }

    #region Move and Target Selection
    //Handle Prompt when selecting move
    private void AskPlayerMove()
    {
        BattleActor ActiveActor = GetActiveActor();
        dialogueController.SetText("What will " + ActiveActor.Actor.Name + " do?");
        dialogueController.SetOption1Text(ActiveActor.Actor.AttackMove.ToString());
        dialogueController.SetOption2Text(ActiveActor.Actor.SpecialMove.ToString());

        BattleState = BattleState.PLAYERSELECTMOVE;
    }

    //Select the move the player has chosen
    public void SelectPlayerMove(short SelectedOption)
    {
        //Select the move
        MoveEnum MoveToSelect;
        switch (SelectedOption)
        {
            case 1: MoveToSelect = GetActiveActor().Actor.AttackMove; break;
            case 2: MoveToSelect = GetActiveActor().Actor.SpecialMove; break;
            default: return; //0 = nothing selected, 3+ = unused
        }
        //Set the move in the current action;
        CurrentActorAction = new ActorAction();
        CurrentActorAction.SetAction(MoveToSelect, 0, 0, 0);

        //Setup for Target selection
        AskPlayerTarget(MoveToSelect);
    }

    //Handle Prompt when selecting target
    private void AskPlayerTarget(MoveEnum SelectedMove)
    {
        BattleActor ActiveActor = GetActiveActor();
        dialogueController.SetText("Who will " + ActiveActor.Actor.Name + " target?");

        //Get Target Team

        if (GetMoveTargetTeam(SelectedMove) == Target.OPPONENT)
        {
            dialogueController.SetOption1Text(EnemyActor1.Actor.Name);
            dialogueController.SetOption2Text(EnemyActor2.Actor.Name);
        }
        else
        {
            dialogueController.SetOption1Text(PetActor1.Actor.Name);
            dialogueController.SetOption2Text(PetActor2.Actor.Name);
        }

        BattleState = BattleState.PLAYERSELECTTARGET;
    }

    //Get a move's Target Team
    private Target GetMoveTargetTeam(MoveEnum Move)
    {
        Target TargetTeam = Target.OPPONENT;
        if (Move == MoveEnum.HEAL)
        {
            TargetTeam = Target.SELF;
        }
        return TargetTeam;
    }

    //Select the move the player has chosen
    public void SetPlayerTarget(short SelectedOption)
    {
        SelectedOption--; //Oopsie, has to be 0 or 1
        //Set the move in the current action;
        CurrentActorAction.SetAction(CurrentActorAction.move, GetMoveTargetTeam(CurrentActorAction.move), SelectedOption, 0);

        //Edit the Speed queue entry to acccomodate the target
        Speeds[ActiveActor] = new SpeedCalculation(GetActiveActor(), CurrentActorAction);

        //Move to next selector
        NextAction();
    }

    //Enemy Select Move function
    private void EnemySelectMove()
    {
        //Get Enemy to Move
        BattleActor CurrentEnemy = GetActiveActor();
        short RandomTarget = (short)Random.Range(0, 3);
        MoveEnum Move = CurrentEnemy.Actor.AttackMove;
        if (RandomTarget == 2)
        {
            Move = CurrentEnemy.Actor.SpecialMove;
        }

        RandomTarget = (short)Random.Range(0, 2);
        //Get Player to attack
        if (Move != MoveEnum.HEAL)
        {
            //Attack as normal
            //Dont attack an inactive or fainted player
            if (PetActor1.Actor.State != ActorState.ACTIVE)
            {
                RandomTarget = 1;
            }
            else if (PetActor2.Actor.State != ActorState.ACTIVE)
            {
                RandomTarget = 0;
            }
        }
        else
        {
            //Only heal the other character (Maiden's move)
            switch (ActiveActor)
            {
                case 2:
                    RandomTarget = 1;
                    break;
                case 3:
                    RandomTarget = 0;
                    break;
            }
        }

        //Set Action
        ActorAction EnemyAction = new ActorAction();
        EnemyAction.SetAction(Move, GetMoveTargetTeam(CurrentEnemy.Actor.AttackMove), RandomTarget, 1);

        //Add action to queue
        Speeds[ActiveActor] = new SpeedCalculation(CurrentEnemy, EnemyAction);
        NextAction();
    }

    //Get Active Player Actor
    private BattleActor GetActiveActor()
    {
        switch (ActiveActor)
        {
            case 0: return PetActor1;
            case 1: return PetActor2;
            case 2: return EnemyActor1;
            case 3: return EnemyActor2;
            default: return null;
        }
    }
    #endregion

    //Struct to set Actor Action
    public struct ActorAction
    {
        public MoveEnum move;
        public Target targetTeam;
        public short userTeam;
        public short target;
        public void SetAction(MoveEnum Move, Target TargetTeam, short Target, short UserTeam)
        {
            move = Move;
            userTeam = UserTeam;
            targetTeam = TargetTeam;
            target = Target;
        }
    }

    //Get the target of an action
    public BattleActor GetActionTarget(ActorAction Action)
    {
        BattleActor TargetActor = null;
        if (Action.userTeam == 0) //Check if player
        {
            if (Action.targetTeam == Target.OPPONENT) //Enemy Opponent
            {
                switch (Action.target)
                {
                    case 0:
                        TargetActor = EnemyActor1; break;
                    case 1:
                        TargetActor = EnemyActor2; break;
                }
            }
            else //Own Players
            {
                switch (Action.target)
                {
                    case 0:
                        TargetActor = PetActor1; break;
                    case 1:
                        TargetActor = PetActor2; break;
                }
            }
        }
        else //Enemy
        {
            if (Action.targetTeam == Target.OPPONENT) //Player Opponent
            {
                switch (Action.target)
                {
                    case 0:
                        TargetActor = PetActor1; break;
                    case 1:
                        TargetActor = PetActor2; break;
                }
            }
            else //Own Enemies
            {
                switch (Action.target)
                {
                    case 0:
                        TargetActor = EnemyActor1; break;
                    case 1:
                        TargetActor = EnemyActor2; break;
                }
            }
        }
        return TargetActor;
    }

    //Struct to calculate turn order
    public struct SpeedCalculation
    {
        public SpeedCalculation(BattleActor BattleActor, ActorAction Action)
        {
            Actor = BattleActor;
            Speed = UnityEngine.Random.Range(BattleActor.Actor.Speed * 0.85f, BattleActor.Actor.Speed);
            this.Action = Action;
        }
        public BattleActor Actor;
        public float Speed;
        public ActorAction Action;
    }
}

//Battle State Enum
public enum BattleState { PLAYERSELECTMOVE, PLAYERSELECTTARGET, ENEMYSELECTACTION, TURN, ENDTURN, BUSY }