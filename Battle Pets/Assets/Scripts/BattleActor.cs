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
        if (Actor.State != ActorState.DEFEATED)
        {
            Actor.State = ActorState.ACTIVE;
        }
    }

    //Display the current status of this actor on the UI
    public void UpdateUI()
    {
        switch (Actor.State)
        {
            case ActorState.DEFEATED: //Display a defeated enemy
                NameText.text = "";
                HealthBar.fillAmount = 0;
                HealthBar.gameObject.transform.GetChild(0).GetComponent<Text>().text = "Defeated";
                SlotSprite.sprite = null;
                break;
            default: //Show normal information
                NameText.text = Actor.Name;
                HealthBar.fillAmount = (Actor.MaxHp - (Actor.MaxHp - Actor.Hp)) / Actor.MaxHp;
                HealthBar.gameObject.transform.GetChild(0).GetComponent<Text>().text = Actor.Hp.ToString() + " / " + Actor.MaxHp.ToString();
                SlotSprite.sprite = Actor.Sprite;
                break;
        }
    }

    //Battle event handlers

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

        //Check if taget is defeated and act accordingly
        if (Target.Actor.Hp <= 0)
        {
            Target.Actor.Faint();
        }
        Target.UpdateUI();
    }

    //Heal move
    public void Heal(BattleActor Target)
    {
        Target.Actor.Hp += (Target.Actor.MaxHp / 4);
        Target.Actor.KeepHealthInBounds();
        Target.UpdateUI();
    }

    //Berserk move
    public void Berserk(BattleActor[] Targets)
    {
        //Damage both opponents
        foreach (BattleActor Target in Targets)
        {
            //Calculate Damage
            float damage = 1f;
            float damageCalc = Actor.Attack - Target.Actor.Defense;
            damageCalc = Mathf.RoundToInt(Random.Range(damageCalc * 0.85f, damageCalc));
            if (damageCalc > damage) damage = damageCalc;

            //Apply Damage
            Target.Actor.Hp -= damage;
            Target.Actor.KeepHealthInBounds();

            //Check if taget is defeated and act accordingly
            if (Target.Actor.Hp <= 0)
            {
                Target.Actor.Faint();
            }
            Target.UpdateUI();
        }

        //Apply Self Damage
        Actor.Hp -= Actor.MaxHp / 5;
        Actor.KeepHealthInBounds();
        UpdateUI();
    }
    #endregion
}