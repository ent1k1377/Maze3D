using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinsUI : MonoBehaviour
{
    [SerializeField] Text _counter;
  


    public void ChangeDisplayValue(string value)
    {
        _counter.text = value;
    }
}
