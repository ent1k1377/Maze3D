using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryMenu : MonoBehaviour
{
    public delegate void Victory();
    public event Victory OnVictory;

    [SerializeField] List<UnitInfo> _ghosts = new List<UnitInfo>();
    [SerializeField] PlayerController _playerController;
    [SerializeField] GameObject _victoryMenu;
    [SerializeField] Text _coinsCount;

    CoinsHandler _playerCoins;
    Animator _playerAnimator;
    Animator _UIAnimator;


    void Start()
    {
        foreach (UnitInfo ghost in _ghosts)
        {
            ghost.OnDying += EnableVictoryMenu;
        }

        _playerCoins = _playerController.GetComponent<CoinsHandler>();
        _playerAnimator = _playerController.GetComponent<Animator>();
        _UIAnimator = _victoryMenu.GetComponent<Animator>();
    }

    public void EnableVictoryMenu(UnitInfo unit)
    {
        _ghosts.Remove(unit);

        if (_ghosts.Count == 0)
        {
            OnVictory?.Invoke();
            StartCoroutine(CallMenuWithDelay(0.5f));
        }
    }

    IEnumerator CallMenuWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _victoryMenu.SetActive(true);
        _coinsCount.text = _playerCoins.TempCoinsAmount.ToString();
        _playerCoins.TempCoinsAmount = 0;
        _playerController.enabled = false;
        _playerAnimator.SetBool("Move", false);

        _UIAnimator.SetTrigger("Appear");
        yield return new WaitForSeconds(_UIAnimator.GetCurrentAnimatorStateInfo(0).length);

        SavePrefs.SeedNumber++;
        SavePrefs.SaveCurrentLevel();
    }
}
