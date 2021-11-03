using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LoadLevelButton : MonoBehaviour, IPointerClickHandler
{
    public delegate void LoadLevel();
    public event LoadLevel OnLoadLevel;

    [SerializeField] GameObject _parentMenu;

    Animator _parentAnimator;


    void Start()
    {
        _parentAnimator = _parentMenu.GetComponent<Animator>();
    }

    public void OnPointerClick(PointerEventData pointerData)
    {
        OnLoadLevel?.Invoke();
        StartCoroutine(Disappear());
    }

    IEnumerator Disappear()
    {
        _parentAnimator.SetTrigger("Disappear");
        yield return new WaitForSeconds(_parentAnimator.GetCurrentAnimatorStateInfo(0).length * 2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
