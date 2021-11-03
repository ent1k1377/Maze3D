using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFOW : MonoBehaviour
{
    public delegate void SeeingTargets(List<Transform> targets);
    public event SeeingTargets OnSeeingTargets;
    public event SeeingTargets OnNotSeeingTargets;

    [SerializeField] float viewAngle;
    [SerializeField] float viewRadius;
    [SerializeField] LayerMask _targetMask;
    [SerializeField] LayerMask _obstacleMask;
    [SerializeField] List<Transform> _allPossibleTargets = new List<Transform>();

    List<Transform> _visibleTargets = new List<Transform>();
    


    void FixedUpdate()
    {
        _visibleTargets.Clear();
        FindVisibleTargets();

        if (_visibleTargets.Count > 0)
        {
            OnSeeingTargets?.Invoke(_visibleTargets);
        }

        OnNotSeeingTargets?.Invoke(TargetsOutOfView());
    }

    void FindVisibleTargets()
    {
        Collider[] targetsInRadius = Physics.OverlapSphere(transform.position + Vector3.up, viewRadius, _targetMask);

        foreach (Collider target in targetsInRadius)
        {
            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
            
            float angleToTarget;

            angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

            if (angleToTarget < viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, _obstacleMask))
                {
                    if (target.GetComponent<UnitInfo>().isPlayer == false)
                    {
                        _visibleTargets.Add(target.transform);
                    }
                }
            }
        }
    }

    List<Transform> TargetsOutOfView()
    {
        List<Transform> targetsOutOfView = new List<Transform>(_allPossibleTargets);

        if (_visibleTargets.Count > 0)
        {
            foreach (Transform visibleTarget in _visibleTargets)
            {
                targetsOutOfView.Remove(visibleTarget); //возможно понадобится проверка на наличие элемента в списке перед ремувом
            }
        }

        return targetsOutOfView;
    }

}
