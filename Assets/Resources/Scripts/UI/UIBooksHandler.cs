using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBooksHandler : MonoBehaviour
{
    [SerializeField] List<Image> _bookSlots = new List<Image>();


    public void SetBookIcon(Sprite sprite)
    {
        List<Image> _tempBookSlots = new List<Image>(_bookSlots);
        foreach (Image img in _tempBookSlots)
        {
            img.sprite = sprite;
            _bookSlots.Remove(img);
            break;
        }
    }
}
