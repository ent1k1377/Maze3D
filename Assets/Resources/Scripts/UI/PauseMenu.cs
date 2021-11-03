using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject _pauseMenu;
    [SerializeField] List<NavMeshAgent> _ghostsAgents;

    Animator _UIAnimator;

    Coroutine _appearing;
    Coroutine _disappearing;

    List<float> _currentVelocities = new List<float>();

    
    void Start()
    {
        _UIAnimator = _pauseMenu.GetComponent<Animator>();
    }

    public void CallPauseMenu()
    {
        if (_appearing == null)
        {
            if (_disappearing != null)
            {
                StopCoroutine(_disappearing);
                _disappearing = null;
            }
            _appearing = StartCoroutine(AppearingPauseMenu());
        }
    }

    public void DisablePauseMenu()
    {
        if (_disappearing == null)
        {
            if (_appearing != null)
            {
                StopCoroutine(_appearing);
                _appearing = null;
            }
            _disappearing = StartCoroutine(DisappearingPauseMenu());
        }
    }

    IEnumerator AppearingPauseMenu()
    {
        SetAIPauseTo(true);
        _pauseMenu.SetActive(true);
        _UIAnimator.SetTrigger("Appear");
        yield return new WaitForSeconds(_UIAnimator.GetCurrentAnimatorStateInfo(0).length * 0.5f);
    }

    IEnumerator DisappearingPauseMenu()
    {
        _UIAnimator.SetTrigger("Disappear");
        yield return new WaitForSeconds(_UIAnimator.GetCurrentAnimatorStateInfo(0).length * 0.7f);
        _pauseMenu.SetActive(false);
        SetAIPauseTo(false);
    }

    void SetAIPauseTo(bool state)
    {
        for (int i = 0; i < _ghostsAgents.Count; i++)
        {
            AIOptions.IsPaused = state;

            if (state == true)
            {
                _currentVelocities.Add(_ghostsAgents[i].speed);
                _ghostsAgents[i].speed = 0f;
            }
            else    _ghostsAgents[i].speed = _currentVelocities[i];
        }

        if (state == false)  _currentVelocities.Clear();
    }
}
