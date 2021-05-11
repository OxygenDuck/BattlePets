using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Move")]
public class Move : ScriptableObject
{
    public Target Target = Target.OPPONENT;
    public string Name = "Move";
    public delegate void MoveInstructions(BattleActor User, BattleActor Target);
    public UnityEvent MoveToUse;
    public MoveInstructions UseMove;

    //This used to be a needed constructor
    //public Move(string Name, Target Target, MoveInstructions MoveToUse)
    //{
    //    this.Name = Name;
    //    this.Target = Target;
    //    UseMove = MoveToUse;
    //}
    public void UseSelectedMove()
    {
        MoveToUse.Invoke();
    }
    
    //Normal Attack
    public static void Attack()
    {
        //TODO: get user and selected target
        BattleActor User = null;
        BattleActor Target = null;

        //Calculate Damage
        float damage = 1f;
        float damageCalc = User.Actor.Attack - Target.Actor.Defense;
        damageCalc = Mathf.RoundToInt(Random.Range(damageCalc * 0.85f, damageCalc));
        if (damageCalc > damage) damage = damageCalc;

        //Apply Damage
        Target.Actor.Hp -= damage;
        Target.Actor.KeepHealthInBounds();
    }

    #region Special Moves
    //Heal move
    //public static void Heal(BattleActor User, BattleActor Target)
    //{
    //    Target.Actor.Hp += (Target.Actor.MaxHp / 2);
    //    Target.Actor.KeepHealthInBounds();
    //}

    //Heal move
    public static void Heal()
    {
        BattleActor Target = null;
        Target.Actor.Hp += (Target.Actor.MaxHp / 2);
        Target.Actor.KeepHealthInBounds();
    }

    //Berserk move
    public static void Berserk(BattleActor User, BattleActor Target)
    {
        //TODO: Damage both opponents?
        //Calculate Damage
        float damage = 1f;
        float damageCalc = User.Actor.Attack - Target.Actor.Defense;
        damageCalc = Mathf.RoundToInt(Random.Range(damageCalc * 0.85f, damageCalc));
        if (damageCalc > damage) damage = damageCalc;

        //Apply Damage
        Target.Actor.Hp -= damage;
        Target.Actor.KeepHealthInBounds();

        //Apply Self Damage
        User.Actor.Hp -= User.Actor.MaxHp / 5;
        User.Actor.KeepHealthInBounds();
    }
    #endregion

}

public enum Target {SELF, OPPONENT }

public enum MoveEnum {ATTACK, HEAL, BERSERK }