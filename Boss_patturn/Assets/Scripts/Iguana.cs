using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Iguana : MonoBehaviour
{
    Animator iguanaAni;
    public Transform target;
    public float iguanaSpeed;
    bool enableAct;             // �׼� ����ġ
    int atkStep;                // ���� ����

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
            Quaternion.Slerp(transform.localRotation,           //Slerp �ε巴�� ȸ��
            Quaternion.LookRotation(dir), 10 * Time.deltaTime);
    }

    void MoveIguana()
    {
        if((target.position - transform.position).magnitude >= 10) // �÷��̾ ���� �̵�
        {
            iguanaAni.SetBool("Walk", true);
            transform.Translate(Vector3.forward * iguanaSpeed
                * Time.deltaTime, Space.Self);
        }

        if((target.position - transform.position).magnitude < 10) // 10���ϸ� ����
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
                    iguanaAni.Play("Attack"); // ������
                    break;
                case 1:
                    atkStep += 1;
                    iguanaAni.Play("EatDown"); // ����
                    break;
                case 2:
                    atkStep = 0;
                    iguanaAni.Play("WalkAttack"); // �����ϸ鼭 ����
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
