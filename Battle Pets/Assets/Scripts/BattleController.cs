using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    //Set References
    public BattleActor PetActor1, PetActor2, EnemyActor1, EnemyActor2;
    private short ActiveActor = 0; //0 = first pet, 1 = second pet, 2 = first enemy, 3 = second enemy
    public BattleState BattleState = BattleState.ENDTURN;
    private DialogueController dialogueController;
    private ActorAction CurrentActorAction;

    //Set up variables
#pragma warning disable IDE0044 // Add readonly modifier
    SpeedCalculation[] Speeds = new SpeedCalculation[4];
#pragma warning restore IDE0044 // Add readonly modifier
    public List<OriginActor> Level1;

    //Start Function
    void Start()
    {
        dialogueController = gameObject.GetComponent<DialogueController>();

        //Get Player Pets
        PetActor1.SetActor(PetManager.Pets[0]);
        PetActor2.SetActor(PetManager.Pets[1]);

        //Get Enemy Monsters
        SetEnemies();
        EnemyActor1.SetActor(PetManager.Enemies[0]);
        EnemyActor2.SetActor(PetManager.Enemies[1]);

        //Set Active Actors
        PetManager.Pets[0].State = ActorState.ACTIVE;
        PetManager.Pets[1].State = ActorState.ACTIVE;
        PetManager.Enemies[0].State = ActorState.ACTIVE;
        PetManager.Enemies[1].State = ActorState.ACTIVE;

        //Prepare for Player Input
        BattleState = BattleState.PLAYERSELECTMOVE;
        NextAction();
    }

    private void SetEnemies()
    {
        switch (PetManager.Level)
        {
            case 0: //Level 1
                PetManager.SetEnemies(Level1); break;
                //TODO: Levels 2 and 3
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
                PlayTurn();
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
    private void PlayTurn()
    {
        //Setup the action queue
        //Queue<ActorAction> Actions = new Queue<ActorAction>();
        //foreach (var Speed in Speeds)
        //{
        //    Actions.Enqueue(Speed.Action);
        //}

        foreach (SpeedCalculation Speed in Speeds)
        {
            Debug.Log(Speed.Action.move.ToString());
            BattleActor Targeted = GetActionTarget(Speed.Action);

            dialogueController.SetText("Attacking.");

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
                    Speed.Actor.Berserk(Targeted);
                    break;
                default: //TODO: HANDLE EMPTY MOVE
                    break;
            }

            //TODO: Wait and show player results of this action
            Targeted.UpdateUI();
        }

        //Clear the speeds
        System.Array.Clear(Speeds, 0, Speeds.Length);
        ActiveActor = 0;

        BattleState = BattleState.PLAYERSELECTMOVE;
        NextAction();
    }

    //TODO: Turn Function
    //TODO: Use Move from Actor
    //TODO: Check for defeated opponent
    //TODO: Check for next Actor in Queue
    //TODO: Check for Battle Win Condition

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

        //TODO: Make queue calculator for this action
        Speeds[ActiveActor] = new SpeedCalculation(GetActiveActor(), CurrentActorAction);

        //Move to next selector
        NextAction();
    }

    //Enemy Select Move function
    private void EnemySelectMove()
    {
        //Get Enemy to Move
        BattleActor CurrentEnemy = GetActiveActor();

        //Get Player to attack
        short RandomTarget = (short)Random.Range(0, 2);

        //Set Action
        ActorAction EnemyAction = new ActorAction();
        EnemyAction.SetAction(CurrentEnemy.Actor.AttackMove, GetMoveTargetTeam(CurrentEnemy.Actor.AttackMove), RandomTarget, 1);

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