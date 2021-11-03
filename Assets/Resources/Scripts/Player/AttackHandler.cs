using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHandler : MonoBehaviour
{
    public delegate void Attacking(UnitInfo target);
    public event Attacking InAttackRange;

    [SerializeField] string _enemyTag;


    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == _enemyTag)
        {
            InAttackRange?.Invoke(collider.GetComponent<UnitInfo>());
        }
    }
}
