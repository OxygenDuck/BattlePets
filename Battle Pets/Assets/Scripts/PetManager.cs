using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PetManager
{
    public static List<Actor> Pets = new List<Actor>();
    public static List<Actor> Enemies = new List<Actor>();
    public static List<Actor> Bosses = new List<Actor>();
    public static int Level = 0; //0 = level 1, 1 = level 2, 2 = level 3
    public static GameState GameState = GameState.PLAYING;
    public static bool ShowTutorials = true;
    public static bool FirstPetFallen = false;

    //Add Pet to the global pets list
    public static void AddPet(OriginActor Origin)
    {
        Actor newPet = new Actor(Origin);
        Pets.Add(newPet);
    }

    //Set the enemies for a level
    public static void SetEnemies(List<OriginActor> OriginEnemies, List<OriginActor> OriginBosses)
    {
        //Clear the old enemies
        Enemies.Clear();
        Bosses.Clear();

        //Set the regular enemies
        foreach (OriginActor Origin in OriginEnemies)
        {
            Enemies.Add(new Actor(Origin));
        }

        //Set the bosses
        foreach (OriginActor Origin in OriginBosses)
        {
            Bosses.Add(new Actor(Origin));
        }
    }
}

//Usable Actor Class
public class Actor
{
    //Constructor
    public Actor(OriginActor Origin)
    {
        //Copy from origin
        Name = Origin.Name;
        Hp = Origin.Hp;
        MaxHp = Origin.MaxHp;
        Attack = Origin.Attack;
        Defense = Origin.Defense;
        Speed = Origin.Speed;
        Sprite = Origin.Sprite;
        AttackMove = Origin.AttackMove;
        SpecialMove = Origin.SpecialMove;
    }

    //Set Variables
    public string Name;
    public float Hp, MaxHp;
    public float Attack, Defense, Speed;
    public Sprite Sprite;
    public ActorState State = ActorState.INACTIVE;
    public MoveEnum AttackMove;
    public MoveEnum SpecialMove;

    //Keep Health in bounds of allowed numbers
    public void KeepHealthInBounds()
    {
        if (Hp > MaxHp) Hp = MaxHp;
        else if (Hp < 0)
        {
            Hp = 0;
            Faint();
        }

        Hp = Mathf.RoundToInt(Hp);
    }

    //What to do when Hp reaches 0
    public void Faint()
    {
        State = ActorState.DEFEATED;
    }
}

public enum ActorState { ACTIVE, INACTIVE, DEFEATED }
public enum GameState { PLAYING, WON, LOST }