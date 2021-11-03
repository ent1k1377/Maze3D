using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsHandler : MonoBehaviour
{
    public int CoinsAmount
    {
        get
        {
            return _coins;
        }
    }
    public int TempCoinsAmount
    {
        get
        {
            return _tempCoins;
        }
        set
        {
            _tempCoins = value;
        }
    }

    [SerializeField] CoinsUI _UI;

    int _coins;
    int _tempCoins;


    void Start()
    {
        _coins = SavePrefs.LoadCoins();
        _UI.ChangeDisplayValue(_coins.ToString());
    }

    public void AddCoin()
    {
        _coins++;
        _tempCoins++;
        _UI.ChangeDisplayValue(_coins.ToString());

        SavePrefs.SaveCoins(_coins);
    }
}
