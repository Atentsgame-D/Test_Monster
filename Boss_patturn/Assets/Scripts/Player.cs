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
        hAxis = Input.GetAxisRaw("Horizontal");      // 상하좌우
        vAxis = Input.GetAxisRaw("Vertical");       // 상하좌우
        wDown = Input.GetButton("Walk");           // 걷기
        jDown = Input.GetButtonDown("Jump");      // 점프
    }
    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;// 상하좌우

        transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;  // 삼항연산자 걷기 달리기 속도조절

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec); // 나아가는 방향으로 바라본다
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
