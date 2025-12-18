using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyState { Idle, Trace, Attack }

    public EnemyState state = EnemyState.Idle;

    public float moveSpeed = 2f; //이동 속도

    public float traceRange = 15f; //추적 시간 거리 

    public float attackRange = 6f; // 공격 시작 거리 

    public float attackCooldown = 1.5f;

    public GameObject projectileprefab; // 투사체 프리팹

    public Transform firePoint;   //발사 위치

    private Transform player;    //플레이어 추적용

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

        // FSM 상태 전환 
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
        //일정 쿨다운마다 발사 
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
