using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    float hAxis;
    float vAxis;
    bool wDown;
    bool jDown;
    bool isJump;

    Vector3 moveVec;

    Rigidbody rigid;
    Animator anim;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();

    }
    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");      // �����¿�
        vAxis = Input.GetAxisRaw("Vertical");       // �����¿�
        wDown = Input.GetButton("Walk");           // �ȱ�
        jDown = Input.GetButtonDown("Jump");      // ����
    }
    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;// �����¿�

        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;  // ���׿����� �ȱ� �޸��� �ӵ�����

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec); // ���ư��� �������� �ٶ󺻴�
    }

    void Jump()
    {
        if(jDown && !isJump)
        {
            rigid.AddForce(Vector3.up * 5, ForceMode.Impulse);
            isJump = true;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            isJump = false;
        }
    }
}
