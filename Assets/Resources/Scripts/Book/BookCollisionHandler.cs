using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookCollisionHandler : MonoBehaviour
{
    public delegate void TakeBook(BookCollisionHandler collision);
    public event TakeBook OnTakingBook;

    public Book Book
    {
        get
        {
            return _book;
        }
    }

    [SerializeField] Book _book;
    [SerializeField] string _playerTag;


    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == _playerTag)
        {
            OnTakingBook?.Invoke(this);
            gameObject.SetActive(false);
        }
    }
}
