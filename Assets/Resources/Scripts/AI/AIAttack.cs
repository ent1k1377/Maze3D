using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAttack : MonoBehaviour
{
    public Coroutine Attacking;

    float _attackRange;

    Animator _selfAnimator;
    UnitInfo _selfInfo;


    void Start()
    {
        _selfAnimator = GetComponent<Animator>();
        _selfInfo = GetComponent<UnitInfo>();

        _attackRange = AIOptions.AttackRange;
    }

    public void AttackTarget(UnitInfo targetInfo)
    {
        if (Attacking == null && Vector3.Distance(transform.position, targetInfo.transform.position) < _attackRange && targetInfo.gameObject.activeSelf)
        {
            Attacking = StartCoroutine(KillTarget(targetInfo));
        }
    }

    public IEnumerator KillTarget(UnitInfo targetInfo)
    {
        //_selfAnimator.SetTrigger("Attack");
        //yield return new WaitForSeconds(_selfAnimator.GetCurrentAnimatorStateInfo(0).length * 0.2f);
            
        targetInfo.Death();

        GetComponent<EnemyAI>().enabled = false;
        GetComponent<AIMover>().StopMoving();
        
        Attacking = null;

        yield return null;
    }
}
