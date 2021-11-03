using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float PlayerMovementSpeed
    {
        set
        {
            _playerMoveSpeed = value;
        }
    }
    public float RotationSpeed
    {
        set
        {
            _rotationSpeed = value;
        }
    }

    [SerializeField] private float _startRotationSpeed;
    [SerializeField] private float _startRotationFadingSpeed;
    [SerializeField] private float _shiftToStartMove;

    private float _playerMoveSpeed;
    private float _rotationSpeed;

    private float _tempRotationSpeed;

    private Animator _playerAnimator;
    private MoveAndAnimSpeedRatio _speedRatio;

    private Vector3 _startMousePosition;
    private Vector3 _movementDirection;

    void Start()
    {
        _playerMoveSpeed = PlayerOptions.PlayerMovementSpeed;
        _rotationSpeed = PlayerOptions.PlayerRotationSpeed;
        _playerAnimator = transform.GetComponent<Animator>();
        _speedRatio = GetComponent<MoveAndAnimSpeedRatio>();
    }

    void FixedUpdate()
    {
        if (AIOptions.IsPaused) return;
        
        if (Input.GetMouseButtonDown(0))
        { 
            _startMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            _tempRotationSpeed = _startRotationSpeed;
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 newMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            _movementDirection = new Vector3(newMousePosition.x - _startMousePosition.x, 0.0f, newMousePosition.y - _startMousePosition.y).normalized;

            if (_movementDirection != Vector3.zero && Vector2.Distance(_startMousePosition, newMousePosition) > _shiftToStartMove)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_movementDirection), _tempRotationSpeed * Time.fixedDeltaTime);
                transform.Translate(transform.forward * _playerMoveSpeed * Time.deltaTime, Space.World);
                
                _tempRotationSpeed = Mathf.Lerp(_tempRotationSpeed, _rotationSpeed, _startRotationFadingSpeed * Time.fixedDeltaTime);

                _playerAnimator.SetBool("Move", true);
                _playerAnimator.SetFloat("Speed", _speedRatio.AnimationSpeed(_playerMoveSpeed));
            }
            else if (_playerAnimator.GetBool("Move"))
            {
                _playerAnimator.SetBool("Move", false);
            } 
        }
        else if (!Input.GetMouseButton(0) && _playerAnimator.GetBool("Move"))
        {
            _playerAnimator.SetBool("Move", false);
        }
    }
}

