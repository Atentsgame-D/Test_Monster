using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Iguana : MonoBehaviour
{
    Animator iguanaAni;
    public Transform target;
    public float iguanaSpeed;
    bool enableAct;             // 액션 스위치
    int atkStep;                // 공격 패턴

    private void Start()
    {
        iguanaAni = GetComponent<Animator>();
        enableAct = true;                       

    }

    private void Update()
    {
        if (enableAct)
        {
            RotateIguana();
            MoveIguana();
        }
    }

    void RotateIguana()
    {
        Vector3 dir = target.position - transform.position;

        transform.localRotation =
            Quaternion.Slerp(transform.localRotation,           //Slerp 부드럽게 회전
            Quaternion.LookRotation(dir), 10 * Time.deltaTime);
    }

    void MoveIguana()
    {
        if((target.position - transform.position).magnitude >= 10) // 플레이어를 향해 이동
        {
            iguanaAni.SetBool("Walk", true);
            transform.Translate(Vector3.forward * iguanaSpeed
                * Time.deltaTime, Space.Self);
        }

        if((target.position - transform.position).magnitude < 10) // 10이하면 정지
        {
            iguanaAni.SetBool("Walk", false);
        }
    }

    void IguanaAtk()
    {
        if ((target.position - transform.position).magnitude < 10)
        {
            switch (atkStep)
            {
                case 0:
                    atkStep += 1;
                    iguanaAni.Play("Attack"); // 할퀴기
                    break;
                case 1:
                    atkStep += 1;
                    iguanaAni.Play("EatDown"); // 물기
                    break;
                case 2:
                    atkStep = 0;
                    iguanaAni.Play("WalkAttack"); // 돌진하면서 공격
                    break;

            }
        }
    }

    void FreezeIguana()
    {
        enableAct = false;
    }

    void UnFreezeIguana()
    {
        enableAct = true;
    }



} 
