using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// �Է¿� ���� �÷��̾��� �ൿ�� ó���� Ŭ����
/// </summary>
public class PlayerInputController : MonoBehaviour
{
    /// <summary>
    /// �޸��� �ӵ�
    /// </summary>
    public float runSpeed = 6.0f;

    /// <summary>
    /// ������ �ӵ�
    /// </summary>
    public float walkSpeed = 3.0f;

    /// <summary>
    /// �̵� ��� ������ enum
    /// </summary>
    enum MoveMode
    {
        Walk = 0,
        Run
    }
    /// <summary>
    /// �⺻ �̵����� Run ����
    /// </summary>
    MoveMode moveMode = MoveMode.Run;

    /// <summary>
    /// ȸ���� �� �ӵ�. 1/turnSpeed�ʿ� ���� ȸ��
    /// </summary>
    public float turnSpeed = 10.0f;

    /// <summary>
    /// �׼Ǹ� ��ü
    /// </summary>
    Boss_patturn actions;

    /// <summary>
    /// kinematic���� ����ϴ� ������ٵ𺸴� ������ �̵��� ������Ʈ
    /// </summary>
    CharacterController controller;

    /// <summary>
    /// �ִϸ����� ������Ʈ
    /// </summary>
    Animator anim;

    /// <summary>
    /// �Է� ���� ������ �����Ͽ� ���������� ������ ����
    /// </summary>
    Vector3 inputDir = Vector3.zero;

    /// <summary>
    /// �ٶ� ������ ����� ȸ��
    /// </summary>
    Quaternion targetRotation = Quaternion.identity;


    /// <summary>
    /// ���� ������Ʈ�� ����ִ� Player ������Ʈ
    /// </summary>
    Player player;


    /// <summary>
    /// ������Ʈ�� ���� ���� ȣ��
    /// </summary>
    private void Awake()
    {
        actions = new();    // �׼Ǹ� ��ü ����
        controller = GetComponent<CharacterController>();   //ĳ���� ��Ʈ�ѷ� ������Ʈ ��������
        anim = GetComponent<Animator>();
        player = GetComponent<Player>();
    }

    /// <summary>
    /// awake ���� ������Ʈ�� Ȱ��ȭ �� �� ����
    /// </summary>
    private void OnEnable()
    {
        actions.Player.Enable();                    // "Player" �׼Ǹ� �ѱ�
        actions.Player.Move.performed += OnMove;    // "Player" �׼Ǹʿ� �Լ� ���
        actions.Player.Move.canceled += OnMove;
        actions.Player.Attack.performed += OnAttack;
    }

    /// <summary>
    /// ������Ʈ�� ��Ȱ��ȭ �� �� ����
    /// </summary>
    private void OnDisable()
    {
        actions.Player.Attack.performed -= OnAttack;
        actions.Player.Move.canceled -= OnMove;
        actions.Player.Move.performed -= OnMove;    // ����� ���Ҵ� �Լ� ����
        actions.Player.Disable();                   // "Player" �׼Ǹ� ����
    }

   

    

    private void OnAttack(InputAction.CallbackContext _)
    {
       // anim.SetFloat("ComboState", Mathf.Repeat(anim.GetCurrentAnimatorStateInfo(0).normalizedTime, 1.0f));
        anim.ResetTrigger("Attack");
        anim.SetTrigger("Attack");
    }

    /// <summary>
    /// Ű �Է� ������ ��� ���� (Run <=> Walk)
    /// </summary>    
    private void OnMoveModeChage(InputAction.CallbackContext _)
    {
        if (moveMode == MoveMode.Walk)
        {
            moveMode = MoveMode.Run;
        }
        else
        {
            moveMode = MoveMode.Walk;
        }
    }

    /// <summary>
    /// WASD�� �������ų� ���� �� ����� �Լ�
    /// </summary>
    /// <param name="context">�Է� ���� ����</param>
    private void OnMove(InputAction.CallbackContext context)
    {
        // �Է� ���� �� ����
        Vector2 input = context.ReadValue<Vector2>();
        //Debug.Log(input);

        // �Է� ���� ���� 3���� ���ͷ� ����. (xz������� ��ȯ)
        inputDir.x = input.x;   // ������ ����
        inputDir.y = 0.0f;
        inputDir.z = input.y;   // �� ��
        //inputDir.Normalize();

        //�Է����� ���� ���� �ִ��� Ȯ��
        if (inputDir.sqrMagnitude > 0.0f)
        {
            // ī�޶��� y�� ȸ���� ���� �и��ؼ� inputDir�� ����
            inputDir = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0) * inputDir;
            // �̵��ϴ� ������ �ٶ󺸴� ȸ���� ����
            targetRotation = Quaternion.LookRotation(inputDir);
            // �׻� �ٴڿ� �ٵ��� ó��
            inputDir.y = -2f;
        }
    }

    /// <summary>
    /// �� �����Ӹ��� ȣ��
    /// </summary>
    private void Update()
    {

        // �̵� �Է� Ȯ��
        if (inputDir.sqrMagnitude > 0.0f)
        {
            float speed = 1.0f;
            if (moveMode == MoveMode.Run)
            {
                // �� ���� �޸��� �ִϸ��̼ǰ� 6�� �̵� �ӵ� ����
                //anim.SetFloat("Speed", 1.0f);
                speed = runSpeed;
            }
            else if (moveMode == MoveMode.Walk)
            {
                // �ȱ� ���� �ȴ� �ִϸ��̼ǰ� 3�� �̵� �ӵ� ����
               // anim.SetFloat("Speed", 0.3f);
                speed = walkSpeed;
            }

            // ������ �̵��ӵ��� ���� ĳ���� �̵�
            controller.Move(speed * Time.deltaTime * inputDir);
        }
        else
        {
            // �Է��� ������ idle �ִϸ��̼����� ����
           // anim.SetFloat("Speed", 0.0f);
        }
        // ��ǥ������ �ٶ󺸵��� ȸ���ϸ� ����
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }
}
