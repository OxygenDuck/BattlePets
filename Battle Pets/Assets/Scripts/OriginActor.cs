using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Origin Actor")]
public class OriginActor : ScriptableObject
{
    //Set Variables
    public string Name = "Name";
    public float Hp = 10, MaxHp = 10;
    public float Attack = 5, Defense = 3, Speed = 10;
    public Sprite Sprite;
    public ActorState State = ActorState.INACTIVE;
    public MoveEnum AttackMove;
    public MoveEnum SpecialMove;

    //Keep Health in bounds of allowed numbers
    public void KeepHealthInBounds()
    {
        if (Hp > MaxHp) Hp = MaxHp;
        else if (Hp < 0) Hp = 0;

        Hp = Mathf.RoundToInt(Hp);
    }
}