using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    Animator _animator;
    bool _isActive = true;


    void Start()
    {
        _animator = GetComponent<Animator>();
        _isActive = gameObject.activeSelf;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_isActive)
            {
                StartCoroutine(Disappear());
            }
        }
    }

    IEnumerator Disappear()
    {
        _isActive = false;
        _animator.SetTrigger("Disappear");
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);
        gameObject.SetActive(false);
    }
}
