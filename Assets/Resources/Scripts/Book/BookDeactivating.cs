using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookDeactivating : MonoBehaviour
{
    public delegate void BookDisappearing(BookDeactivating bookDeactivator);
    public event BookDisappearing OnBookDissapearing;

    [SerializeField] Animator _animator;
    [SerializeField] ParticleSystem _ambientEffect;  
    [SerializeField] ParticleSystem _explosionEffect; 
    [SerializeField] BookCollisionHandler _bookHandler;
    [SerializeField] IdleRotater _book;

    float _deactivateDuration;
    float _rotateAcceleration;
    float _animationLengthMult;
    float _animationSpeed;

    void Awake()
    {
        _bookHandler.OnTakingBook += Deactivating;
    }

    void Start()
    {
        _deactivateDuration = BookOptions.DeactivatingDuration;
        _rotateAcceleration = BookOptions.RotateAcceleration;
        _animationLengthMult = BookOptions.AnimationLengthMult;
        _animationSpeed = BookOptions.AnimationSpeed;
    }

    void Deactivating(BookCollisionHandler coll)
    {
        StartCoroutine(BookDeactivatingEffects(_deactivateDuration, _rotateAcceleration));
    }

    IEnumerator BookDeactivatingEffects(float duration, float acceleration)
    {
        _animator.SetFloat("ClosingSpeedMult", _animationSpeed);
        _animator.SetTrigger("Deactivate");
        _ambientEffect.Play();
        var emission = _ambientEffect.emission;
        float emissionRate = emission.rateOverTime.constant;
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length * _animationLengthMult);
        
        float startTime = Time.time;
        
        while (Time.time - startTime < duration)
        {
            _book.RotationSpeed += acceleration;
            yield return new WaitForFixedUpdate();
            emissionRate +=  _book.RotationSpeed / 100;
            emission.rateOverTime = emissionRate;
        }
        _ambientEffect.Stop();
        _book.gameObject.SetActive(false);
        _explosionEffect.Play();
        OnBookDissapearing?.Invoke(this);
    }

    void OnDestroy()
    {
        _bookHandler.OnTakingBook -= Deactivating;
    }
}
