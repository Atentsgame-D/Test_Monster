using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Player : MonoBehaviour, IHealth,  IBattle
{
    GameObject weapon;
    GameObject sheild;

    ParticleSystem ps;
    Animator anim;

    // IHealth ------------------------------------------------------------------------------------
    public float hp = 100.0f;
    float maxHP = 100.0f;

    public float HP
    {
        get => hp;
        set
        {
            if(hp != value)
            {
                hp = Mathf.Clamp(value, 0, maxHP);
                onHealthChange?.Invoke();
            }
        }
    }

    public float MaxHP
    {
        get => maxHP;
    }

    public System.Action onHealthChange { get; set; }

    // IMana --------------------------------------------------------------------------------------
    public float mp = 150.0f;
    float maxMP = 150.0f;

    public float MP
    {
        get => mp;
        set
        {
            if (mp != value)
            {
                mp = Mathf.Clamp(value, 0, maxMP);
                onManaChange?.Invoke();
            }
        }
    }

    public float MaxMP => maxMP;

    public System.Action onManaChange { get ; set; }


    // IBattle ------------------------------------------------------------------------------------
    public float attackPower = 30.0f;
    public float defencePower = 10.0f;
    public float criticalRate = 0.3f;
    public float AttackPower { get => attackPower; }

    public float DefencePower { get => defencePower; }




    private void Awake()
    {
        anim = GetComponent<Animator>();
    } 

   

    public void Attack(IBattle target)
    {
        if (target != null)
        {
            float damage = AttackPower;
            if (Random.Range(0.0f, 1.0f) < criticalRate)
            {
                damage *= 2.0f;
            }
            target.TakeDamage(damage);
        }
    }

    public void TakeDamage(float damage)
    {
        float finalDamage = damage - defencePower;
        if (finalDamage < 1.0f)
        {
            finalDamage = 1.0f;
        }
        HP -= finalDamage;

        if (HP > 0.0f)
        {
            //살아있다.
            anim.SetTrigger("Hit");
        }
        else
        {
            //죽었다.
            //Die();
        }
    }

   
}
