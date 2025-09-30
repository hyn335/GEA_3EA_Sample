using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyState { Idle, Trace, Attack }

    public EnemyState state = EnemyState.Idle;

    public float moveSpeed = 2f; //�̵� �ӵ�

    public float traceRange = 15f; //���� �ð� �Ÿ� 

    public float attackRange = 6f; // ���� ���� �Ÿ� 

    public float attackCooldown = 1.5f;

    public GameObject projectileprefab; // ����ü ������

    public Transform firePoint;   //�߻� ��ġ

    private Transform player;    //�÷��̾� ������

    private float lastAttackTime;

    public int maxHP = 5;

    private int currentHP;
  
    
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        lastAttackTime = -attackCooldown;
        currentHP = maxHP;
    }




    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(player.position, transform.position);

        // FSM ���� ��ȯ 
        switch(state)
        {
            case EnemyState.Idle:
                if (dist < traceRange)
                    state = EnemyState.Trace;
                break;

            case EnemyState.Trace:
                if (dist < attackRange)
                    state = EnemyState.Attack;
                else if (dist > traceRange)
                    state = EnemyState.Idle;
                else
                    TracePlayer();
                break;

            case EnemyState.Attack:
                if (dist > attackRange)
                    state = EnemyState.Trace;
                else
                    AttackPlayer();
                break;
                
        }
    }
    
   
    void TracePlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
        transform.LookAt(player.position);
    }
    
    void AttackPlayer()
    {
        //���� ��ٿ�� �߻� 
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            Shootprojectile();
        }
    }
    
    void Shootprojectile()
    {
        if (projectileprefab != null && firePoint != null)
        {
            transform.LookAt(player.position);
            GameObject proj = Instantiate(projectileprefab, firePoint.position, firePoint.rotation);
            EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
            if (ep != null)
            {
                Vector3 dir = (player.position - firePoint.position).normalized;
                ep.SetDirection(dir);
            }

        }
    }

}
