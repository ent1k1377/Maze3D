using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] AttackHandler _attackHandler;
    [SerializeField] LayerMask _obstacleLayer;


    UnitInfo _selfInfo;
    Animator _animator;

    Coroutine _attacking;


    void Start()
    {
        _selfInfo = GetComponent<UnitInfo>();
        _animator = GetComponent<Animator>();
        _attackHandler.InAttackRange += AttackTarget;
    }

    void AttackTarget(UnitInfo targetInfo)
    {
        if (_attacking == null && !targetInfo.haveBook) 
        {
            Vector3 direction = (targetInfo.transform.position - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, targetInfo.transform.position);
            if (!Physics.Raycast(transform.position, direction, distance, _obstacleLayer))
                _attacking = StartCoroutine(Attacking(targetInfo));
        }
    }

    IEnumerator Attacking(UnitInfo targetInfo)
    {
        //_animator.SetTrigger("Attack");
        //yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(1).length * 0.2f);
        yield return null;

        targetInfo.Death();

        _attacking = null;
    }

    void OnDestroy()
    {
        _attackHandler.InAttackRange -= AttackTarget;
    }
}
