using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [HideInInspector] public List<Vector3> SearchArea = new List<Vector3>();
    [HideInInspector] public List<Vector3> PatrolArea = new List<Vector3>();
    [HideInInspector] public AIStates CurrentState;
    [HideInInspector] public Transform BookToDefend;

    [SerializeField] MapGrid _map; 
    [SerializeField] LayerMask _obstaclesLayer;
    [SerializeField] BookDeactivating _bookDeactivator;
    [SerializeField] FlairArea _flair;
    [SerializeField] StagesVisualizationHandler _stagesVisualization;
    [SerializeField] int _waitingFramesCount;

    float _timeToCurious;
    float _timeToSearch;
    float _timeToChase;

    float _curiousDuration;
    float _searchDuration;
    float _senseDuration;
    float _chaseDuration;

    int _searchRadiusArea;

    int _currentWaitingFrames = 0;

    PatrolAreaMode _patrolAreaMode;
    int _bookRadiusArea;

    bool _disableLookingAround;
    float _lookAroundAngle;
    float _lookAroundSpeed;
    float _lookAroundDuration;

    AIMover _pathMover;
    AIAttack _attacker;
    UnitInfo _selfInfo;
    UnitFieldOfView _fow;  

    Transform _player;

    bool _seeSmthSuspicious = false;
    bool _readyToLookAround = false;
    bool _goingAltWay = false;
    bool _isSensing = false;
    bool _isRunningAway = false;
    bool _inCorner = false;
    bool _wasReturningToCalmness = false;
    bool _playerWasInFOW = false;

    Coroutine _playerInFOW;
    Coroutine _returnToCalmness;
    Coroutine _lookAround;
    Coroutine _rotateTo;
    Coroutine _sensePlayer;

    List<Vector3> _areaToEscape = new List<Vector3>();


    void Awake()
    {
        _bookDeactivator.OnBookDissapearing += SetEscapeSettings;
        _flair.OnSense += SensePlayer;
    }

    void Start()
    {
        _fow = GetComponent<UnitFieldOfView>();
        _attacker = GetComponent<AIAttack>();
        _pathMover = GetComponent<AIMover>();
        _selfInfo = GetComponent<UnitInfo>();

        _timeToCurious = AIOptions.TimeToCurious;
        _timeToSearch = AIOptions.TimeToSearch;
        _timeToChase = AIOptions.TimeToChase;

        _curiousDuration = AIOptions.CuriousDuration;
        _searchDuration = AIOptions.SearchDuration;
        _senseDuration = AIOptions.SenseDuration;
        _chaseDuration = AIOptions.ChaseDuration;

        _searchRadiusArea = AIOptions.SearchRadiusArea;

        _bookRadiusArea = AIOptions.BookPatrolRadius;
        _patrolAreaMode = AIOptions.EnemyAreaToPatrol;

        _disableLookingAround = AIOptions.DisableLookingAround;
        _lookAroundAngle = AIOptions.LookAroundAngle;
        _lookAroundSpeed = AIOptions.LookAroundSpeed;
        _lookAroundDuration = AIOptions.LookAroundDuration;

        if (_patrolAreaMode == PatrolAreaMode.Book)
            PatrolArea = _map.GetAreaAtNearbyPoint(new Vector3(BookToDefend.position.x, 0, BookToDefend.position.z), _bookRadiusArea);
        else if (_patrolAreaMode == PatrolAreaMode.Chunk)
        {
            foreach (MazeCell cell in BookToDefend.GetComponent<Book>().OriginChunk.MazeCells)
                PatrolArea.Add(cell.transform.position);
        }

        CurrentState = AIStates.idle;
        _stagesVisualization.EnableIdleSymbols();
    }
    
    void OnDrawGizmos()
    {
        if (_areaToEscape.Count > 0)
        foreach (Vector3 item in _areaToEscape)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(new Vector3(item.x, item.y + 0.6f, item.z), 0.1f);
        }
    }

    void SetEscapeSettings(BookDeactivating bookDeactivator)
    {
        _timeToCurious = AIOptions.TimeToCuriousEscape;
        _timeToSearch = AIOptions.TimeToSearchEscape;
        _timeToChase = AIOptions.TimeToChaseEscape;

        _curiousDuration = AIOptions.CuriousDurationEscape;
        _searchDuration = AIOptions.SearchDurationEscape;
        _senseDuration = AIOptions.SenseDurationEscape;
        _chaseDuration = AIOptions.ChaseDurationEscape;

        _disableLookingAround = AIOptions.DisableLookingAroundEscape;
        _lookAroundAngle = AIOptions.LookAroundAngleEscape;
        _lookAroundSpeed = AIOptions.LookAroundSpeedEscape;
        _lookAroundDuration = AIOptions.LookAroundDurationEscape;
    }

    void FixedUpdate()
    {
        if (AIOptions.IsPaused)
        {
            if (_playerInFOW != null)
            {
                _playerWasInFOW = true;
                CheckAndStopCoroutine(ref _playerInFOW);
            }
            else if (_returnToCalmness != null)
            {
                _wasReturningToCalmness = true;
                CheckAndStopCoroutine(ref _returnToCalmness);
            }
            CheckAndStopCoroutine(ref _rotateTo);
            CheckAndStopCoroutine(ref _lookAround);
            return;
        }
        else if (_playerWasInFOW)
        {
            _playerWasInFOW = false;
            _playerInFOW = StartCoroutine(PlayerInFOW());
        } 
        else if (_wasReturningToCalmness)
        {
            _wasReturningToCalmness = false;
            _returnToCalmness = StartCoroutine(ReturnToCalmness());
        }


        if (_selfInfo.haveBook)
            AggressiveStagesHandler();
        else
            EscapeStagesHandler();


        if (_fow.VisiblePlayer && !_seeSmthSuspicious && CurrentState < AIStates.chase)
        {
            _seeSmthSuspicious = true;
            _isSensing = false;

            CheckAndStopCoroutine(ref _rotateTo);
            CheckAndStopCoroutine(ref _lookAround);
            CheckAndStopCoroutine(ref _returnToCalmness);
            CheckAndStopCoroutine(ref _playerInFOW);
            CheckAndStopCoroutine(ref _sensePlayer);

            _playerInFOW = StartCoroutine(PlayerInFOW());
        }
        else if (!_fow.VisiblePlayer && _seeSmthSuspicious)
        {
            _seeSmthSuspicious = false;

            CheckAndStopCoroutine(ref _playerInFOW);
            CheckAndStopCoroutine(ref _returnToCalmness);

            _returnToCalmness = StartCoroutine(ReturnToCalmness());
        }

        if (_fow.VisiblePlayer && !_pathMover.isMoving && CurrentState > AIStates.patrol)
        {
            Vector3 spottedPlayer = (_fow.VisiblePlayer.transform.position - transform.position).normalized;
            Quaternion rotateToPoint = Quaternion.LookRotation(spottedPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotateToPoint, 5f * Time.deltaTime);
        }
    }

    void SensePlayer(Transform player)
    {
        if (!_fow.VisiblePlayer && CurrentState < AIStates.curious && _sensePlayer == null 
            && Physics.Raycast(transform.position, (player.position - transform.position).normalized, 
                Vector3.Distance(player.position, transform.position), _obstaclesLayer))
        {
            _sensePlayer = StartCoroutine(ReactOnSensedPlayer());
            _isSensing = true;
        }

        IEnumerator ReactOnSensedPlayer()
        {
            CurrentState = AIStates.None;
            _fow.FOWMode = FOWMaterial.attention;
            _stagesVisualization.EnableSearchSymbols();

            _pathMover.StopMoving();
            CheckAndStopCoroutine(ref _lookAround);
            CheckAndStopCoroutine(ref _rotateTo);

            yield return new WaitForSeconds(0.8f);

            if (player == null)
            {
                CurrentState = AIStates.idle;
                _sensePlayer = null;
                yield break;
            }

            CurrentState = AIStates.search;
            _pathMover.MovementType = MovementType.Search;
            SearchArea = _map.GetAreaAtNearbyPoint(player.position, 3);
            _seeSmthSuspicious = true;

            _sensePlayer = null;
        }
    }

    void AggressiveStagesHandler()
    {
        switch (CurrentState)
        {
            case AIStates.idle :
            {
                if (!_pathMover.isMoving && _lookAround == null)
                {
                    _lookAround = StartCoroutine(LookAround(_lookAroundDuration));
                    CurrentState = AIStates.patrol;
                }
                break;
            }
            case AIStates.patrol :
            {
                if (_lookAround == null)
                {               
                    MoveTo(GetRandomPoint(PatrolArea));
                    CurrentState = AIStates.idle;
                }
                break;
            }
            case AIStates.curious :
            {
                if (!_fow.VisiblePlayer)
                    CheckSuspiciousPosition(_fow.LastPlayerPosition);
                break;
            }
            case AIStates.search :
            {
                if (!_pathMover.isMoving && !_fow.VisiblePlayer)
                {
                    if (SearchArea == null || SearchArea.Count == 0)
                        SearchArea = _map.GetAreaInDirection(_fow.LastPlayerPosition, _searchRadiusArea, _fow.LastPlayerDirection);

                    if (!_isSensing)
                    {
                        CheckSuspiciousPosition(GetRandomPoint(SearchArea));
                    }
                    else if (_isSensing && !_pathMover.isMoving)
                    {
                        MoveTo(GetRandomPoint(SearchArea));
                    }
                }   
                break;
            }
            case AIStates.chase :
            {
                if (SearchArea != null || SearchArea.Count > 0)
                    SearchArea.Clear();

                if (_player != null)
                    ChasePlayer();
                else
                {
                    MoveTo(_fow.LastPlayerPosition);
                }
                break;
            }
        }
    }

    void EscapeStagesHandler()
    {
        switch (CurrentState)
        {
            case AIStates.idle :
            {
                if (_lookAround == null)
                {
                    CurrentState = AIStates.patrol;
                    MoveTo(GetRandomPoint(PatrolArea));
                }
                break;
            }
            case AIStates.patrol :
            {
                if (!_pathMover.isMoving && _lookAround == null)
                {
                    _lookAround = StartCoroutine(LookAround(_lookAroundDuration));
                    CurrentState = AIStates.idle;
                }
                break;
            }
            case AIStates.curious :
            {
                if (AIOptions.RunawayTillCalm && _isRunningAway)
                {
                    _stagesVisualization.EnableSearchSymbols();
                    RunningAway(_fow.LastPlayerPosition);
                    break;
                }



                if (!_fow.VisiblePlayer)
                {
                    if (_lookAround == null)
                        _lookAround = StartCoroutine(LookAround(_lookAroundDuration));
                }
                break;
            }
            case AIStates.search :
            {
                if (AIOptions.RunawayTillCalm && _isRunningAway)
                {
                    _stagesVisualization.EnableSearchSymbols();
                    RunningAway(_fow.LastPlayerPosition);
                    break;
                }



                if (_pathMover.isMoving && !_isSensing)
                    _pathMover.StopMoving();

                if (!_fow.VisiblePlayer && !_isSensing)
                {
                    CheckAndStopCoroutine(ref _lookAround);
                    
                    if (_rotateTo == null && transform.forward != (_fow.LastPlayerPosition - transform.position).normalized)
                        _rotateTo = StartCoroutine(RotateInDirection((_fow.LastPlayerPosition - transform.position).normalized, 6f));
                }
                else if (!_fow.VisiblePlayer && _isSensing && !_pathMover.isMoving)
                {
                    MoveTo(GetRandomPoint(SearchArea));
                }   
                break;
            }
            case AIStates.chase :
            {
                if (_player)
                {
                    _stagesVisualization.EnableFearSymbols();
                    RunningAway(_player.position);
                }
                break;
            }

            void RunningAway(Vector3 lastPlayerPosition)
            {
                if (!_pathMover.isMoving)
                {
                    RunAway(lastPlayerPosition, 4);
                    _goingAltWay = false;
                }
                else if (_fow.VisiblePlayer && _goingAltWay)
                {
                    RunAway(lastPlayerPosition, 4);
                    _goingAltWay = false;
                }
            }
        }
    }

    void MoveTo(Vector3 destination)
    {
        CheckAndStopCoroutine(ref _rotateTo);
        CheckAndStopCoroutine(ref _lookAround);

        _pathMover.MoveToDestination(destination);
    }

    void CheckSuspiciousPosition(Vector3 posToCheck)
    {
        if (_lookAround == null && _readyToLookAround == false)
        {
            MoveTo(posToCheck);
            _readyToLookAround = true;
        }
        else if (!_pathMover.isMoving && _readyToLookAround == true)
        {
            _lookAround = StartCoroutine(LookAround(_lookAroundDuration));
            _readyToLookAround = false;
        }
    }

    void ChasePlayer()
    {
        _attacker.AttackTarget(_player.GetComponent<UnitInfo>());

        MoveTo(_player.position);
    }

    void RunAway(Vector3 playerPosition, int escapeAreaRadius)
    {
        if (_inCorner && _currentWaitingFrames < _waitingFramesCount)
        {
            _currentWaitingFrames++;
            return;
        }
        _currentWaitingFrames = 0;

        _pathMover.MovementType = MovementType.Escape;

        MoveTo(GetRandomPoint(_map.Grid));
    }

    IEnumerator LookAround(float duration)
    {
        if (_disableLookingAround)
        {
           _lookAround = null;
            yield break; 
        }

        _readyToLookAround = false;
        _pathMover.StopMoving();

        Vector3 viewedZone = FindViewedZone();
        if (viewedZone != transform.position)
        {
            IEnumerator coroutine = RotateToPosition(viewedZone, 4f);
            yield return StartCoroutine(coroutine);
            StopCoroutine(coroutine);
        }
        else if (viewedZone == transform.position)
        {
            _lookAround = null;
            yield break;
        }

        float defaultAngle = transform.eulerAngles.y;
        float startTime = Time.time;
        Quaternion angleToRight = Quaternion.Euler(0, defaultAngle + _lookAroundAngle, 0);
        Quaternion angleToLeft = Quaternion.Euler(0, defaultAngle - _lookAroundAngle, 0);
        Quaternion rotateToPoint = angleToRight;

        while (Time.time - startTime < duration)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotateToPoint, _lookAroundSpeed * Time.deltaTime);

            if (Quaternion.Angle(transform.rotation, angleToRight) < 5f)
                rotateToPoint = angleToLeft;
            else if (Quaternion.Angle(transform.rotation, angleToLeft) < 5f)
                rotateToPoint = angleToRight;  

            yield return new WaitForFixedUpdate();
        }

        _lookAround = null;
    }

    IEnumerator RotateToPosition(Vector3 positionToRotate, float rotationSpeed)
    {
        Vector3 direction = (positionToRotate - transform.position).normalized;
        Quaternion rotationAngle = Quaternion.LookRotation(direction);
        while (Quaternion.Angle(transform.rotation, rotationAngle) > 5f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotationAngle, rotationSpeed * Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }
        transform.rotation = rotationAngle;
    }

    IEnumerator RotateInDirection(Vector3 direction, float rotationSpeed)
    {
        Quaternion rotationAngle = Quaternion.LookRotation(direction);
        while (Quaternion.Angle(transform.rotation, rotationAngle) > 5f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotationAngle, rotationSpeed * Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }
        transform.rotation = rotationAngle;
    }

    IEnumerator PlayerInFOW()
    {
        if (CurrentState < AIStates.curious)
        {
            yield return new WaitForSeconds(_timeToCurious);
            CurrentState = AIStates.curious;
            _fow.FOWMode = FOWMaterial.attention;
            _pathMover.MovementType = MovementType.Search;
            _stagesVisualization.EnableSearchSymbols();
            _pathMover.StopMoving();

            yield return new WaitForSeconds(_timeToSearch);
            CurrentState = AIStates.search;
            
            yield return new WaitForSeconds(_timeToChase);
            SetStateToChase();
        }
        else if (CurrentState == AIStates.curious)
        {
            yield return new WaitForSeconds(_timeToSearch);
            CurrentState = AIStates.search;
            _fow.FOWMode = FOWMaterial.attention;
            _pathMover.MovementType = MovementType.Search;
            _pathMover.StopMoving();

            yield return new WaitForSeconds(_timeToChase);
            SetStateToChase();
        }
        else if (CurrentState == AIStates.search)
        {
            yield return new WaitForSeconds(_timeToChase);
            SetStateToChase();
        }

        void SetStateToChase()
        {
            CheckAndStopCoroutine(ref _rotateTo);
            CheckAndStopCoroutine(ref _lookAround);
            _isRunningAway = true;
            CurrentState = AIStates.chase;
            _fow.FOWMode = FOWMaterial.alarm;
            _pathMover.MovementType = MovementType.Chase;
            _player = _fow.VisiblePlayer;
            if (_selfInfo.haveBook)
                _stagesVisualization.EnableChaseSymbols();
            else
                _stagesVisualization.EnableFearSymbols();
        }
    }

    IEnumerator ReturnToCalmness()
    {
        if (CurrentState == AIStates.chase)
        {
            if (!_selfInfo.haveBook)
                _stagesVisualization.EnableFearSymbols();

            yield return new WaitForSeconds(_chaseDuration);
            _fow.FOWMode = FOWMaterial.attention;
            CurrentState = AIStates.search;
            _stagesVisualization.EnableSearchSymbols();
            _player = null;

            yield return new WaitForSeconds(_searchDuration);
            SearchArea.Clear();
            CurrentState = AIStates.curious;
            _pathMover.MovementType = MovementType.Search;

            yield return new WaitForSeconds(_curiousDuration);
            _isRunningAway = false;
            _fow.FOWMode = FOWMaterial.calm;
            CurrentState = AIStates.idle;
            _stagesVisualization.EnableIdleSymbols();
            _pathMover.MovementType = MovementType.Walk;
        }
        else if (CurrentState == AIStates.search)
        {
            if (_isSensing)
            {
                yield return new WaitForSeconds(_senseDuration);
                _isSensing = false;
            }
            else yield return new WaitForSeconds(_searchDuration);
            SearchArea.Clear();
            CurrentState = AIStates.curious;
            _stagesVisualization.EnableSearchSymbols();
            _pathMover.MovementType = MovementType.Search;


            yield return new WaitForSeconds(_curiousDuration);
            _isRunningAway = false;
            _fow.FOWMode = FOWMaterial.calm;
            CurrentState = AIStates.idle;
            _stagesVisualization.EnableIdleSymbols();
            _pathMover.MovementType = MovementType.Walk;
        }
        else if (CurrentState == AIStates.curious)
        {
            yield return new WaitForSeconds(_curiousDuration * 2);
            SearchArea.Clear();
            _isRunningAway = false;
            _fow.FOWMode = FOWMaterial.calm;
            CurrentState = AIStates.idle;
            _stagesVisualization.EnableIdleSymbols();
            _pathMover.MovementType = MovementType.Walk;
        }
    }

    Vector3 FindViewedZone()
    {
        List<Vector3> accessibleDirs = new List<Vector3>();
        List<Vector3> supposedDirs = new List<Vector3>
        {
            new Vector3( 1,  0,  0),
            new Vector3(-1,  0,  0),
            new Vector3( 0,  0,  1),
            new Vector3( 0,  0, -1),
        }; 

        foreach (Vector3 dir in supposedDirs)
        {
            if (Physics.OverlapSphere(PosToCheck(dir), 0.05f, _obstaclesLayer).Length == 0)
                accessibleDirs.Add(dir);
        }

        if (accessibleDirs.Count == 1)
            return transform.position + accessibleDirs[0];
        else if (accessibleDirs.Count > 1)
            return transform.position + accessibleDirs[Random.Range(0, accessibleDirs.Count)];
        else 
            return transform.position;

        Vector3 PosToCheck(Vector3 direction)
        {
            Vector3 posToLook = transform.position + direction;
            posToLook = new Vector3(Mathf.Round(posToLook.x), 1, Mathf.Round(posToLook.z));
            return posToLook;
        }
    }

    Vector3 GetRandomPoint(Vector3[,] map)
    {
        if (map.Length == 0)
        {
            return transform.position;
        }

        List<Vector3> tempMap = ArrayToList(map);

        float maxPathLength = Mathf.Sqrt(tempMap.Count) * 2f;
        
        while (tempMap.Count > 0)
        {
            Vector3 point = tempMap[Random.Range(0, tempMap.Count)];
            tempMap.Remove(point);
            Vector3 dirToWaypoint = _pathMover.GetDirectionToNextWaypointInCalculatedPath(point);

            if (_player == null && _pathMover.GetPathLength(point) < maxPathLength)
            {
                _inCorner = false;
                return point;
            }
            else if (_player != null && _pathMover.GetPathLength(_player.position) > 10f)
            {
                _inCorner = false;
                _goingAltWay = true;
                return point;
            }
            else if (_player != null && dirToWaypoint != Vector3.zero && !IsDirectionToPlayerEqualToCompareDirection(_player.position, dirToWaypoint))
            {
                _inCorner = false;
                return point;
            }
        }
        _inCorner = true;
        return transform.position;

        List<Vector3> ArrayToList(Vector3[,] array)
        {
            List<Vector3> list = new List<Vector3>();
            foreach(Vector3 item in array)
                list.Add(item);
            return list;
        }
    }

    Vector3 GetRandomPoint(List<Vector3> map)
    {
        if (map.Count == 0)
        {
            return transform.position;
        }

        List<Vector3> tempMap = new List<Vector3>(map);

        float maxPathLength = Mathf.Sqrt(tempMap.Count) * 2f;
        
        while (tempMap.Count > 0)
        {
            Vector3 point = tempMap[Random.Range(0, tempMap.Count)];
            tempMap.Remove(point);
            Vector3 dirToWaypoint = _pathMover.GetDirectionToNextWaypointInCalculatedPath(point);

            if (_player == null && _pathMover.GetPathLength(point) < maxPathLength)
            {
                _inCorner = false;
                return point;
            }
            else if (_player != null && _pathMover.GetPathLength(_player.position) > 10f)
            {
                _inCorner = false;
                _goingAltWay = true;
                return point;
            }
            else if (_player != null && dirToWaypoint != Vector3.zero && !IsDirectionToPlayerEqualToCompareDirection(_player.position, dirToWaypoint))
            {
                _inCorner = false;
                return point;
            }
        }
        _inCorner = true;
        return transform.position;
    }

    bool IsDirectionToPlayerEqualToCompareDirection(Vector3 playerPosition, Vector3 directionToCompare)
    {
        Vector3 directionToPlayer = (playerPosition - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(directionToCompare, directionToPlayer);
        if (angleToPlayer < 75f)
        {
            return true;
        }
        else 
        {
            return false;
        }
    }

    bool IsPlayerInFrontFOW(Vector3 playerPosition)
    {
        Vector3 directionToPlayer = (playerPosition - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer < 45f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void CheckAndStopCoroutine(ref Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    void OnEnable()
    {
        _stagesVisualization.gameObject.SetActive(true);
    }

    void OnDisable()
    {
        _stagesVisualization.gameObject.SetActive(false);
    }
}

public enum AIStates
{
    None,
    idle,
    patrol,
    curious,
    search,
    chase
}

public enum PatrolAreaMode
{
    Chunk,
    Book
}
