using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class Enemy_Boss : MonoBehaviour,IHealth, IBattle
{
    NavMeshAgent agent;
    Animator anim;

    EnemyState state = EnemyState.Idle;

    //Idle �� --------------------------------------------------------------------------------------
    float waitTime = 3.0f;
    float timeCountDown = 3.0f;
    //������------------------------------------------------------------------------------------------
    float sightRange = 10.0f;
    float closeSightRange = 2.5f;
    Vector3 targetPosition = new();
    WaitForSeconds oneSecond = new WaitForSeconds(1.0f);
    IEnumerator repeatChase = null;
    float sightAngle = 150.0f;   //-45 ~ +45 ����
    //���ݿ� -----------------------------------------------------------------------------------------
    float attackCoolTime = 1.0f;
    float attackSpeed = 1.0f;
    IBattle attackTarget;

    //����� -----------------------------------------------------------------------------------------
    bool isDead = false;

    //IHealth -------------------------------------------------------------------------------------
    public float hp = 100.0f;
    float maxHP = 100.0f;
    public float HP
    {
        get => hp;
        set
        {
            hp = Mathf.Clamp(value, 0.0f, maxHP);
            onHealthChange?.Invoke();
        }
    }

    public float MaxHP { get => maxHP; }

    public System.Action onHealthChange { get; set; }


    //IBattle -------------------------------------------------------------------------------------
    public float attackPower = 10.0f;
    public float defencePower = 10.0f;
    float criticalRate = 0.1f;

    public float AttackPower { get => attackPower; }

    public float DefencePower { get => defencePower; }

    //---------------------------------------------------------------------------------------------

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        //if(patrolRoute!=null)
        //{
        //    agent.SetDestination(patrolRoute.position);  // ��ã��� ���귮�� ���� �۾�. SetDestination�� �����ϸ� �ȵȴ�.
        //}        

        switch (state)
        {
            case EnemyState.Idle:
                IdleUpdate();
                break;
           case EnemyState.Chase:
                ChaseUpdate();
                break;
            case EnemyState.Attack:
                AttackUpdate();
                break;
            case EnemyState.Dead:
            default:
                break;
        }
    }
    void IdleUpdate()
    {
        if (SearchPlayer())
        {
            ChangeState(EnemyState.Chase);
            return;
        }

       /* timeCountDown -= Time.deltaTime;
        if (timeCountDown < 0)
        {*/
           // ChangeState(EnemyState.Idle);
           // return;
        //}
    }

    bool SearchPlayer()
    {
        bool result = false;
        Collider[] colliders = Physics.OverlapSphere(transform.position, sightRange, LayerMask.GetMask("Player"));
        if (colliders.Length > 0)    // �þ� ���� �ȿ� �ִ°�?
        {
            Vector3 pos = colliders[0].transform.position;
            if (InSightAngle(pos))  // �þ� ���� �ȿ� �ִ°�?
            {
                if (!BlockByWall(pos))  // ���� ���ȴ°�?
                {
                    targetPosition = pos;
                    result = true;
                }
            }
            if (!result && (pos - transform.position).sqrMagnitude < closeSightRange * closeSightRange)
            {
                targetPosition = pos;
                result = true;
            }
        }
        return result;
    }

    void ChaseUpdate()
    {
        if (!SearchPlayer())
        {
            ChangeState(EnemyState.Idle);
            return;
        }
    }
    IEnumerator RepeatChase()
    {
        while (true)
        {
            yield return oneSecond;
            agent.SetDestination(targetPosition);
        }
    }
    
    
    void AttackUpdate()
    {
        attackCoolTime -= Time.deltaTime;
        transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(attackTarget.transform.position - transform.position), 0.1f);
        if (attackCoolTime < 0.0f)
        {
            anim.SetTrigger("Attack");
            Attack(attackTarget);
            attackCoolTime = attackSpeed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameManager.Inst.MainPlayer.gameObject)
        {
            attackTarget = other.GetComponent<IBattle>();
            ChangeState(EnemyState.Attack);
            return;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == GameManager.Inst.MainPlayer.gameObject)
        {
            ChangeState(EnemyState.Idle);
            return;
        }
    }

    void ChangeState(EnemyState newState)
    {
        if (isDead)
        {
            return;
        }

        // ���� ���¸� �����鼭 �ؾ��� �ϵ�
        switch (state)
        {
            case EnemyState.Idle:
                agent.isStopped = true;
                break;
             case EnemyState.Chase:
                agent.isStopped = true;
                StopCoroutine(repeatChase);
                break;
            case EnemyState.Attack:
                agent.isStopped = true;
                attackTarget = null;
                break;
            case EnemyState.Dead:
                agent.isStopped = true;
                isDead = false;
                break;
            default:
                break;
        }

        // �� ���·� ���鼭 �ؾ��� �ϵ�
        switch (newState)
        {
            case EnemyState.Idle:
                agent.isStopped = true;
                timeCountDown = waitTime;
                break;
           case EnemyState.Chase:
                agent.isStopped = false;
                agent.SetDestination(targetPosition);
                repeatChase = RepeatChase();
                StartCoroutine(repeatChase);
                break;
            case EnemyState.Attack:
                agent.isStopped = true;
                attackCoolTime = attackSpeed;
                break;
            case EnemyState.Dead:
                DiePresent();
                break;
            default:
                break;
        }

        state = newState;
        anim.SetInteger("EnemyState", (int)state);
    }
    void DiePresent()
    {
        
        anim.SetBool("Dead", true);
        anim.SetTrigger("Die");
        isDead = true;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        HP = 0;
    }

    IEnumerator DeadEffect()
    {

        EnemyHP_Bar hpBar = GetComponentInChildren<EnemyHP_Bar>();
        hpBar.gameObject.SetActive(false);

        yield return new WaitForSeconds(3.0f);
        Collider[] colliders = GetComponents<Collider>();
        foreach (var col in colliders)
        {
            col.enabled = false;
        }
        agent.enabled = false;
        Rigidbody rigid = GetComponent<Rigidbody>();
        rigid.isKinematic = false;
        rigid.drag = 20.0f;
        Destroy(this.gameObject, 5.0f);
    }
    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.blue;
        //Gizmos.DrawWireSphere(transform.position, sightRange);
        Handles.color = Color.blue;
        Handles.DrawWireDisc(transform.position, transform.up, sightRange);


        Handles.color = Color.green;
        if (state == EnemyState.Chase || state == EnemyState.Attack)
        {
            Handles.color = Color.red;  // �����̳� ���� ���� ���� ������
        }
        Handles.DrawWireDisc(transform.position, transform.up, closeSightRange); // ���� �þ� ����

        Vector3 forward = transform.forward * sightRange;
        Quaternion q1 = Quaternion.Euler(0.5f * sightAngle * transform.up);
        Quaternion q2 = Quaternion.Euler(-0.5f * sightAngle * transform.up);
        Handles.DrawLine(transform.position, transform.position + q1 * forward);    // �þ߰� ������ ��
        Handles.DrawLine(transform.position, transform.position + q2 * forward);    // �þ߰� ���� ��

        Handles.DrawWireArc(transform.position, transform.up, q2 * transform.forward, sightAngle, sightRange, 5.0f);// ��ü �þ߹���
    }
    bool InSightAngle(Vector3 targetPosition)
    {
        // �� ������ ���̰�
        float angle = Vector3.Angle(transform.forward, targetPosition - transform.position);
        // ������ �þ߹��� �������̿� �ִ��� ������
        return (sightAngle * 0.5f) > angle;
    }
    bool BlockByWall(Vector3 targetPosition)
    {
        bool result = true;
        Ray ray = new(transform.position, targetPosition - transform.position); 
        ray.origin += Vector3.up * 0.5f;    
        if (Physics.Raycast(ray, out RaycastHit hit, sightRange))
        {
            if (hit.collider.CompareTag("Player"))     
            {
                result = false; 
            }
        }

        return result;  
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
            anim.SetTrigger("Hit");
            attackCoolTime = attackSpeed;
        }
        else
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead == false)
        {
            ChangeState(EnemyState.Dead);
        }
    }
}
