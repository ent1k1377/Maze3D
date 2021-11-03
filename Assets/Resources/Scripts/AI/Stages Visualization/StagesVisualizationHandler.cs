using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagesVisualizationHandler : MonoBehaviour
{
    [SerializeField] EnemyAI _AI;

    [Space(4f)]
    [SerializeField] GameObject _idleSymbols;
    [SerializeField] GameObject _searchSymbols;
    [SerializeField] GameObject _chaseSymbols;
    [SerializeField] GameObject _fearSymbols;

    List<GameObject> _symbols = new List<GameObject>();


    void Start()
    {
        _symbols.Add(_idleSymbols);
        _symbols.Add(_searchSymbols);
        _symbols.Add(_chaseSymbols);
        _symbols.Add(_fearSymbols);
    }

    public void EnableIdleSymbols()
    {
        ChangeSymbolToCurrent(_idleSymbols);
    }

    public void EnableSearchSymbols()
    {
        ChangeSymbolToCurrent(_searchSymbols);
    }

    public void EnableChaseSymbols()
    {
        ChangeSymbolToCurrent(_chaseSymbols);
    }

    public void EnableFearSymbols()
    {
        ChangeSymbolToCurrent(_fearSymbols);
    }

    void ChangeSymbolToCurrent(GameObject symbol)
    {
        foreach(GameObject _symbol in _symbols)
        {
            if (_symbol.Equals(symbol))
                _symbol.SetActive(true);
            else
                _symbol.SetActive(false);
        }
    }
}
