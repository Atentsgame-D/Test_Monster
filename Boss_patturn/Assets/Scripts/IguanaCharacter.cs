﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어 이동 따라가기


public class IguanaCharacter : MonoBehaviour 
{
	//Animator iguanaAnimator;
    [SerializeField] private string animalName; // 동물의 이름
    [SerializeField] private int hp;  // 동물의 체력
    
    private Vector3 direction;  // 방향

    // 상태 변수
    private bool isAction;  // 행동 중인지 아닌지 판별
    private bool isWalking; // 걷는지, 안 걷는지 판별

    [SerializeField] private float walkSpeed;  // 걷기 속력
    [SerializeField] private float walkTime;  // 걷기 시간
    [SerializeField] private float waitTime;  // 대기 시간
    private float currentTime;

    // 필요한 컴포넌트
    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody rigidl;
    [SerializeField] private BoxCollider boxCol;

    void Start()
    {
        currentTime = waitTime;   // 대기 시작
        isAction = true;   // 대기도 행동
     //   iguanaAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        Rotation();
        ElapseTime();
    }

    private void Move()
    {
        if (isWalking)
            rigidl.MovePosition(transform.position + transform.forward * walkSpeed * Time.deltaTime);
    }

    private void Rotation()
    {
        if (isWalking)
        {
            Vector3 _rotation = Vector3.Lerp(transform.eulerAngles, direction, 0.01f);
            rigidl.MoveRotation(Quaternion.Euler(_rotation));
        }
    }

    private void ElapseTime()
    {
        if (isAction)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)  // 랜덤하게 다음 행동을 개시
                ReSet();
        }
    }

    private void ReSet()  // 다음 행동 준비
    {
        isWalking = false;
        isAction = true;
        anim.SetBool("Walking", isWalking);

        direction.Set(0f, Random.Range(0f, 360f), 0f);

        RandomAction();
    }

    private void RandomAction()
    {
        int _random = Random.Range(0, 4); // 대기, 풀뜯기, 두리번, 걷기

        if (_random == 0)
            Wait();
        else if (_random == 1)
            Eat();
        else if (_random == 2)
            Attack();
        else if (_random == 3)
            TryWalk();
    }

    private void Wait()  // 대기
    {
        currentTime = waitTime;
        Debug.Log("대기");
    }

    private void Eat()  // 뜯기
    {
        currentTime = waitTime;
        anim.SetTrigger("Eat");
        Debug.Log("뜯기");
    }

    private void Attack()  // 공격
    {
        currentTime = waitTime;
        anim.SetTrigger("Attack");
        Debug.Log("공격");
    }

    private void TryWalk()  // 걷기
    {
        currentTime = walkTime;
        isWalking = true;
        anim.SetBool("Walking", isWalking);
        Debug.Log("걷기");
    }

    
	
	/*public void Attack(){
		iguanaAnimator.SetTrigger("Attack");
	}
	
	public void Hit(){
		iguanaAnimator.SetTrigger("Hit");
	}
	
	*//*public void Eat(){
		iguanaAnimator.SetTrigger("Eat");
	}*//*

	public void Death(){
		iguanaAnimator.SetTrigger("Death");
	}

	public void Rebirth(){
		iguanaAnimator.SetTrigger("Rebirth");
	}


	
	public void Move(float v,float h){
		iguanaAnimator.SetFloat ("Forward", v);
		iguanaAnimator.SetFloat ("Turn", h);
	}*/

    // 플레이어 이동 따라가기
}
