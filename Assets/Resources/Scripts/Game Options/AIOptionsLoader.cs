using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIOptionsLoader : MonoBehaviour
{
    [Space(3)]
    [Header("AI movement settings")]
    [SerializeField] [Range(0.1f, 2)] float _enemyPatrolSpeed;
    [SerializeField] [Range(0.1f, 3)] float _enemySearchSpeed;
    [SerializeField] [Range(0.1f, 4)] float _enemyChaseSpeed;
    [SerializeField] [Range(0.1f, 4)] float _enemyEscapeSpeed;

    [Space(3)]
    [Header("AI patrol settings")]
    [SerializeField] PatrolAreaMode _enemyAreaToPatrol;
    [SerializeField] [Range(0, 10)] int _bookPatrolRadius;

    [Space(3)]
    [Header("AI search area radius")]
    [SerializeField] int _searchRadiusArea;

    [Space(3)]
    [Header("AI attack range")]
    [SerializeField] [Range(0.1f, 2)] float _attackRange;


    [Space(3)]
    [Header("Agressive AI settings", order = 0)]
    
    [Space(15)]
    [Header("FOW settings", order = 1)]
    [SerializeField] [Range(0f, 360f)] float _angleOfView;
    [SerializeField] [Range(0f, 10f)] float _rangeOfView;
    [SerializeField] [Range(0f, 5f)] float _aroundAreaOfView;

    [Space(2)]
    [Header("Stages upgrade speed")]
    [SerializeField] float _timeToCurious;
    [SerializeField] float _timeToSearch;
    [SerializeField] float _timeToChase;

    [Space(2)]
    [Header("Stages degrade speed")]
    [SerializeField] float _curiousDuration;
    [SerializeField] float _searchDuration;
    [SerializeField] float _senseDuration;
    [SerializeField] float _chaseDuration;

    [Space(2)]
    [Header("Look around settings")]
    [SerializeField] bool _disableLookingAround;
    [SerializeField] float _lookAroundAngle;
    [SerializeField] [Range(0.2f, 5f)] float _lookAroundSpeed;
    [SerializeField] float _lookAroundDuration;



    [Space(3)]
    [Header("Escaper AI settings", order = 0)]
    
    [Space(15)]
    [Header("FOW settings", order = 1)]
    [SerializeField] [Range(0f, 360f)] float _angleOfViewEscape;
    [SerializeField] [Range(0f, 10f)] float _rangeOfViewEscape;
    [SerializeField] [Range(0f, 5f)] float _aroundAreaOfViewEscape;

    [Space(2)]
    [Header("Stages upgrade speed")]
    [SerializeField] float _timeToAttention;
    [SerializeField] float _timeToNervous;
    [SerializeField] float _timeToEscape;

    [Space(2)]
    [Header("Stages degrade speed")]
    [SerializeField] bool _runawayTillCalm;
    [SerializeField] float _attentionDuration;
    [SerializeField] float _nervousDuration;
    [SerializeField] float _senseDurationEscape;
    [SerializeField] float _escapeDuration;

    [Space(2)]
    [Header("Look around settings")]
    [SerializeField] bool _disableLookingAroundEscape;
    [SerializeField] float _lookAroundAngleEscape;
    [SerializeField] [Range(0.2f, 5f)] float _lookAroundSpeedEscape;
    [SerializeField] float _lookAroundDurationEscape;



    void Awake()
    {
        AIOptions.EnemyPatrolSpeed = _enemyPatrolSpeed;
        AIOptions.EnemySearchSpeed = _enemySearchSpeed;
        AIOptions.EnemyChaseSpeed = _enemyChaseSpeed;
        AIOptions.EnemyEscapeSpeed = _enemyEscapeSpeed;

        AIOptions.EnemyAreaToPatrol = _enemyAreaToPatrol;
        AIOptions.BookPatrolRadius = _bookPatrolRadius;
        AIOptions.SearchRadiusArea = _searchRadiusArea;

        AIOptions.AttackRange = _attackRange;


        AIOptions.AngleOfView = _angleOfView;
        AIOptions.RangeOfView = _rangeOfView;
        AIOptions.AroundAreaOfView = _aroundAreaOfView;

        AIOptions.AngleOfViewEscape = _angleOfViewEscape;
        AIOptions.RangeOfViewEscape = _rangeOfViewEscape;
        AIOptions.AroundAreaOfViewEscape = _aroundAreaOfViewEscape;
 


        AIOptions.TimeToCurious = _timeToCurious;
        AIOptions.TimeToSearch = _timeToSearch;
        AIOptions.TimeToChase = _timeToChase;
 
        AIOptions.CuriousDuration = _curiousDuration;
        AIOptions.SearchDuration = _searchDuration;
        AIOptions.SenseDuration = _senseDuration;
        AIOptions.ChaseDuration = _chaseDuration;
 
        AIOptions.DisableLookingAround = _disableLookingAround;
        AIOptions.LookAroundAngle = _lookAroundAngle;
        AIOptions.LookAroundSpeed = _lookAroundSpeed;
        AIOptions.LookAroundDuration = _lookAroundDuration;



        AIOptions.TimeToCuriousEscape = _timeToAttention;
        AIOptions.TimeToSearchEscape = _timeToNervous;
        AIOptions.TimeToChaseEscape = _timeToEscape;
 
        AIOptions.RunawayTillCalm = _runawayTillCalm;
        AIOptions.CuriousDurationEscape = _attentionDuration;
        AIOptions.SearchDurationEscape = _nervousDuration;
        AIOptions.SenseDurationEscape = _senseDurationEscape;
        AIOptions.ChaseDurationEscape = _escapeDuration;
 
        AIOptions.DisableLookingAroundEscape = _disableLookingAroundEscape;
        AIOptions.LookAroundAngleEscape = _lookAroundAngleEscape;
        AIOptions.LookAroundSpeedEscape = _lookAroundSpeedEscape;
        AIOptions.LookAroundDurationEscape = _lookAroundDurationEscape;
    }
}
