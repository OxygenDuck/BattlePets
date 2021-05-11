using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class BattleActor : MonoBehaviour
{
    //Set up References
    public Text NameText;
    public Image HealthBar;
    public Actor Actor;
    private SpriteRenderer SlotSprite;

    void Awake()
    {
        SlotSprite = gameObject.GetComponent<SpriteRenderer>();
    }

    //Set Actor function
    public void SetActor(Actor newActor)
    {
        Actor = newActor;
        if (Actor.Hp > Actor.MaxHp)
        {
            Actor.Hp = Actor.MaxHp;
        }
        UpdateUI();
        Actor.State = ActorState.ACTIVE;
    }

    public void UpdateUI()
    {
        NameText.text = Actor.Name;
        HealthBar.fillAmount = (Actor.MaxHp - (Actor.MaxHp - Actor.Hp)) / Actor.MaxHp;
        SlotSprite.sprite = Actor.Sprite;
    }

    //TODO: Battle event handlers

    #region Moves
    //Moves
    public void Attack(BattleActor Target)
    {
        //Calculate Damage
        float damage = 1f;
        float damageCalc = Actor.Attack - Target.Actor.Defense;
        damageCalc = Mathf.RoundToInt(Random.Range(damageCalc * 0.85f, damageCalc));
        if (damageCalc > damage) damage = damageCalc;

        //Apply Damage
        Target.Actor.Hp -= damage;
        Target.Actor.KeepHealthInBounds();
    }

    //Heal move
    public void Heal(BattleActor Target)
    {
        Target.Actor.Hp += (Target.Actor.MaxHp / 2);
        Target.Actor.KeepHealthInBounds();
    }

    //TODO: Berserk move
    public void Berserk(BattleActor Target)
    {
        //TODO: Damage both opponents?
        //Calculate Damage
        float damage = 1f;
        float damageCalc = Actor.Attack - Target.Actor.Defense;
        damageCalc = Mathf.RoundToInt(Random.Range(damageCalc * 0.85f, damageCalc));
        if (damageCalc > damage) damage = damageCalc;

        //Apply Damage
        Target.Actor.Hp -= damage;
        Target.Actor.KeepHealthInBounds();

        //Apply Self Damage
        Actor.Hp -= Actor.MaxHp / 5;
        Actor.KeepHealthInBounds();
    }
    #endregion
}