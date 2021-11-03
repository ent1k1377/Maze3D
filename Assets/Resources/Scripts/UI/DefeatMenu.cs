using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefeatMenu : MonoBehaviour
{
    public delegate void Defeat();
    public event Defeat OnDefeat;

    [SerializeField] UnitInfo _player;

    [SerializeField] GameObject _defeatMenu;
    [SerializeField] Text _coinsCount;

    CoinsHandler _playerCoins;
    Animator _UIAnimator;


    void Start()
    {
        _player.OnDying += EnableDefeatMenu;

        _playerCoins = _player.GetComponent<CoinsHandler>();
        _UIAnimator = _defeatMenu.GetComponent<Animator>();
    }

    void EnableDefeatMenu(UnitInfo unit)
    {
        OnDefeat?.Invoke();
        StartCoroutine(CallMenuWithPause(0.5f));
    }

    IEnumerator CallMenuWithPause(float delay)
    {
        yield return new WaitForSeconds(delay);
        _defeatMenu.SetActive(true);
        _coinsCount.text = _playerCoins.TempCoinsAmount.ToString();

        _UIAnimator.SetTrigger("Appear");
        yield return new WaitForSeconds(_UIAnimator.GetCurrentAnimatorStateInfo(0).length);
    }
}
