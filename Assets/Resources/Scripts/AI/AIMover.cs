using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMover : MonoBehaviour
{
    [HideInInspector] public MovementType MovementType;
    
    public bool isMoving
    {
        get
        {
            return _agent.hasPath;
        }
    }

    [SerializeField] Transform _objToTilt;
    [SerializeField] ParticleSystem _moveEffect;
    [SerializeField] ParticleSystem _trailEffect;

    bool _startedToTilt = false;
    bool _isTilt = false;

    float _walkSpeed;
    float _searchSpeed;
    float _chaseSpeed;
    float _escapeSpeed;

    NavMeshAgent _agent;
    Animator _animator;
    //MoveAndAnimSpeedRatio _speedRatio;


    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        //_speedRatio = GetComponent<MoveAndAnimSpeedRatio>();
        _walkSpeed = AIOptions.EnemyPatrolSpeed;
        _searchSpeed = AIOptions.EnemySearchSpeed;
        _chaseSpeed = AIOptions.EnemyChaseSpeed;
        _escapeSpeed = AIOptions.EnemyEscapeSpeed;
    }

    void Update()
    {
        if (_objToTilt != null)
        {
            if (isMoving)
            {
                if (!_moveEffect.isPlaying)
                {
                    _moveEffect.Play();
                    _trailEffect.Play();
                }
    
                if (!_isTilt)
                {
                    if (!_startedToTilt)
                    {
                        _startedToTilt = true;
                    }
    
                    Vector3 eulerRotation = _objToTilt.localRotation.eulerAngles;
                    Quaternion finalRotation = Quaternion.Euler(15f, eulerRotation.y, eulerRotation.z);
    
                    _objToTilt.localRotation = Quaternion.Slerp(_objToTilt.localRotation, finalRotation, 3f * Time.deltaTime);
    
                    if (Quaternion.Angle(_objToTilt.localRotation, finalRotation) < 1f)
                    {
                        _objToTilt.localRotation = finalRotation;
                        _isTilt = true;
                        _startedToTilt = false;
                    }
                }
            }
            else if (!isMoving)
            {
                if (_moveEffect.isPlaying)
                {
                    _moveEffect.Stop();
                    _trailEffect.Stop();
                }
    
                if (_isTilt || _startedToTilt)
                {
                    Vector3 eulerRotation = _objToTilt.localRotation.eulerAngles;
                    Quaternion finalRotation = Quaternion.Euler(0f, eulerRotation.y, eulerRotation.z);
    
                    _objToTilt.localRotation = Quaternion.Slerp(_objToTilt.localRotation, finalRotation, 3f * Time.deltaTime);
    
                    if (Quaternion.Angle(_objToTilt.localRotation, finalRotation) < 1f)
                    {
                        _objToTilt.localRotation = finalRotation;
                        _isTilt = false;
                        _startedToTilt = false;
                    }
                }
            }
        } 
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        if (_agent.path.corners.Length > 0)
        for (int i = 0; i < _agent.path.corners.Length - 1; ++i)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(_agent.path.corners[i], _agent.path.corners[i + 1]);
        }
    }

    public void MoveToDestination(Vector3 destination)
    {
        //_animator.SetBool("Move", true);
        //_animator.SetFloat("Speed", _speedRatio.AnimationSpeed(_agent.speed));
        //_animator.SetFloat("Speed", _agent.speed);

        switch (MovementType)
        {
            case (MovementType.Walk) :
            {
                _agent.speed = _walkSpeed;
                break;
            }
            case (MovementType.Search) :
            {
                _agent.speed = _searchSpeed;
                break;
            }
            case (MovementType.Chase) :
            {
                _agent.speed = _chaseSpeed;
                break;
            }
            case (MovementType.Escape) :
            {
                _agent.speed = _escapeSpeed;
                break;
            }
        }

        _agent.SetDestination(destination);
    }

    public float GetPathLength(Vector3 destination)
    {
        NavMeshPath path = new NavMeshPath();
        _agent.CalculatePath(destination, path);
        float distance = 0f;
        for (int i = 0; i < path.corners.Length - 1; ++i)
        {
            distance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }
        
        return distance;
    }

    public Vector3 GetDirectionToNextWaypointInCalculatedPath(Vector3 destination)
    {
        Vector3 direction = new Vector3();
        NavMeshPath path = new NavMeshPath();
        _agent.CalculatePath(destination, path);
        if (path.corners.Length < 2)
        {
            return Vector3.zero;
        }
        direction = (path.corners[1] - transform.position).normalized;
        return direction;
    }

    public void StopMoving()
    {
        //_animator.SetBool("Move", false);
        _agent.ResetPath();
    }
}

public enum MovementType
{
    Walk,
    Search,
    Chase,
    Escape
}
