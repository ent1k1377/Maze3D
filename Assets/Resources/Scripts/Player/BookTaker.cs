using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookTaker : MonoBehaviour
{
    [SerializeField] List<BookCollisionHandler> _booksCollisionHandlers = new List<BookCollisionHandler>();
    [SerializeField] UIBooksHandler _booksUI;

    UnitInfo _playerInfo;
    PlayerController _playerMover;


    void Start()
    {
        _playerInfo = GetComponent<UnitInfo>();
        _playerMover = GetComponent<PlayerController>();

        foreach (var handler in _booksCollisionHandlers)
        {
            handler.OnTakingBook += TakeBook;
        }
    }

    void TakeBook(BookCollisionHandler handler)
    {
        Book book = handler.Book;
        _playerInfo.Books.Add(book);
        _playerInfo.BoundBooksWithOwner();
        _booksCollisionHandlers.Remove(handler);

        if (_playerMover && _booksCollisionHandlers.Count == 0)
        {
            _playerMover.PlayerMovementSpeed = PlayerOptions.PlayerMovementSpeedWithAllBooks;
            _playerMover.RotationSpeed = PlayerOptions.PlayerRotationSpeedWithAllBooks;
        }

        if (_booksUI)
        {
            _booksUI.SetBookIcon(book.BookSprite);
        }
    }
}
