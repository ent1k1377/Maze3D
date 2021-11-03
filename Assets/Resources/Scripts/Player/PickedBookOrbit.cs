using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickedBookOrbit : MonoBehaviour
{
    [SerializeField] Transform _objToFollow;
    [SerializeField] List<BookAxis> _bookAxes;

    [SerializeField] Transform _bookOrbit;

    [SerializeField] List<Transform> _bookSlots;

    [SerializeField] List<BookDeactivating> _spawnedBooks = new List<BookDeactivating>(); //Все книги на карте. Нужно чисто для подписки на ивент подбора игроком

    List<GameObject> _pickedBooks = new List<GameObject>();
    List<Transform> _ascendingBooks = new List<Transform>();

    UnitInfo _objInfo;

    float _rotationSpeed = 8f;

    



    void Start()
    {
        if (_objToFollow.GetComponent<UnitInfo>())
        {
            _objInfo = _objToFollow.GetComponent<UnitInfo>();
            _objInfo.OnDying += DeactivateOrbit;
        }

        if (_objToFollow.GetComponent<PlayerFOW>())
        {
            _objToFollow.GetComponent<PlayerFOW>().OnSeeingTargets += TakeBookFromOrbit;
            _objToFollow.GetComponent<PlayerFOW>().OnNotSeeingTargets += ReturnBookToOrbit;
        }

        foreach(BookDeactivating deactivator in _spawnedBooks)
        {
            deactivator.OnBookDissapearing += AddBookToOrbit;
        }
    }

    void FixedUpdate()
    {
        if (_objToFollow != null)
        {
            Vector3 position = new Vector3(_objToFollow.position.x, transform.position.y, _objToFollow.position.z);
            transform.position = position;
        }
        
        _bookOrbit.rotation *= Quaternion.AngleAxis(2f, Vector3.up);

        foreach (BookAxis axis in _bookAxes)
        {
            StagesHandler(axis);
        }
    }

    void StagesHandler(BookAxis axis)
    {
        switch (axis.CurrentStage)
        {
            //Начало возвращения книги на орбиту
            case OrbitalBookStages.StartReturning :
            {
                axis.transform.SetParent(_bookOrbit);

                if (axis.BookIsClosed == false)
                {
                    axis.BookAnimator.SetTrigger("Deactivate");
                    axis.BookAnimator.SetFloat("ClosingSpeedMult", 1);
                    axis.BookIsClosed = true;
                }
                _ascendingBooks.Remove(axis.transform); //возможно понадобится предварительная проверка на наличие
                axis.IndexOfAscend = -1;
                axis.CurrentStage = OrbitalBookStages.IsReturning;
                break;
            }
            //Процесс возвращения книги на орбиту
            case OrbitalBookStages.IsReturning :
            {
                float localSpeed = 2.5f;
                Quaternion rotation = Quaternion.Euler(0f, axis.AngleInOrbit, 0f);
                Vector3 bookOffset = axis.BookContainer.localPosition;
                bookOffset = new Vector3(bookOffset.x, bookOffset.y, _bookSlots[0].localPosition.z);

                axis.transform.localRotation = Quaternion.Slerp(axis.transform.localRotation, rotation, localSpeed * Time.deltaTime);
                axis.transform.localPosition = Vector3.Lerp(axis.transform.localPosition, Vector3.zero, localSpeed * Time.deltaTime);
                axis.BookContainer.localPosition = Vector3.Lerp(axis.BookContainer.localPosition, bookOffset, _rotationSpeed * Time.deltaTime);
                TiltBookByLocalX(axis.BookContainer, 0f);
                
                if (Quaternion.Angle(axis.transform.localRotation, rotation) < 1f)
                {
                    axis.transform.localPosition = Vector3.zero;
                    axis.BookContainer.localPosition = bookOffset;
                    axis.transform.localRotation = Quaternion.Euler(0f, axis.AngleInOrbit, 0f);

                    axis.CurrentStage = OrbitalBookStages.InOrbit;
                }
                break;
            }

            //Холостая стадия, обозначающая, что книга на орбите
            case OrbitalBookStages.InOrbit :
            {
                break;
            }

            //Начало перемещения к позиции перед игроком
            case OrbitalBookStages.StartAscending :
            {
                axis.transform.SetParent(transform);
                _ascendingBooks.Add(axis.transform);
                axis.CurrentStage = OrbitalBookStages.IsAscending;
                break;
            }
            //Процесс перемещения к позиции перед игроком
            case OrbitalBookStages.IsAscending :
            {
                Quaternion rotation = GetRotationToTarget(axis.transform, axis.BoundGhost);
                Vector3 bookOffset;
                Vector3 finishAxisPosition = GetPositionToMove(axis, out bookOffset);
                axis.transform.rotation = Quaternion.Slerp(axis.transform.rotation, rotation, _rotationSpeed * Time.deltaTime);
                axis.transform.position = Vector3.Lerp(axis.transform.position, finishAxisPosition, _rotationSpeed * Time.deltaTime);
                axis.BookContainer.localPosition = Vector3.Lerp(axis.BookContainer.localPosition, bookOffset, _rotationSpeed * Time.deltaTime);
                TiltBookByLocalX(axis.BookContainer, 25f);

                if (axis.BookIsClosed == true && Quaternion.Angle(axis.transform.rotation, rotation) < 40f)
                {
                    axis.BookAnimator.SetTrigger("Activate");
                    axis.BookAnimator.SetFloat("ClosingSpeedMult", 2);
                    axis.BookIsClosed = false;
                }

                if (Quaternion.Angle(axis.transform.rotation, rotation) < 1f)
                {
                    axis.transform.position = finishAxisPosition;
                    axis.BookContainer.localPosition = bookOffset;
                    axis.transform.SetParent(_objToFollow);
                    axis.transform.localRotation = new Quaternion();

                    axis.IndexOfAscend = _ascendingBooks.IndexOf(axis.transform);
                    axis.CurrentStage = OrbitalBookStages.WasAscended;
                }

                break;
            }

            //Книга на позиции перед игроком
            case OrbitalBookStages.WasAscended :
            {
                if (axis.IndexOfAscend != _ascendingBooks.IndexOf(axis.transform)) //Проверка на порядок размещения книг перед игроком
                {
                    float speed = 2f;
                    Vector3 bookOffset;
                    Vector3 finishAxisPosition = GetPositionToMove(axis, out bookOffset);
                    axis.transform.position = Vector3.Lerp(axis.transform.position, finishAxisPosition, speed * Time.deltaTime);
                    axis.BookContainer.localPosition = Vector3.Lerp(axis.BookContainer.localPosition, bookOffset, speed * Time.deltaTime);

                    if (Vector3.Distance(axis.transform.position, finishAxisPosition) < 0.01f)
                    {
                        axis.transform.position = finishAxisPosition;
                        axis.BookContainer.localPosition = bookOffset;

                        axis.IndexOfAscend = _ascendingBooks.IndexOf(axis.transform);
                    }
                }

                axis.transform.rotation = GetRotationToTarget(axis.transform, axis.BoundGhost);
                break;
            }
        }
    }

    void ReturnBookToOrbit(List<Transform> ghosts)
    {
        if (_pickedBooks.Count == 0)
            return;

        foreach (Transform ghost in ghosts)
        {
            foreach (BookAxis axis in _bookAxes)
            {
                if (axis.BookAnimator && axis.BoundGhost != null && axis.BoundGhost == ghost)
                {
                    if (axis.CurrentStage > OrbitalBookStages.InOrbit)
                    {
                        axis.CurrentStage = OrbitalBookStages.StartReturning;
                    }
                }
                else if (axis.BookAnimator && axis.BoundGhost == null)
                {
                    if (axis.CurrentStage > OrbitalBookStages.InOrbit)
                    {
                        axis.CurrentStage = OrbitalBookStages.StartReturning;
                    }
                }
            }
        }
    }

    void TakeBookFromOrbit(List<Transform> ghosts)
    {
        if (_pickedBooks.Count == 0)
            return;

        foreach (Transform ghost in ghosts)
        {
            foreach (BookAxis axis in _bookAxes)
            {
                if (axis.BookAnimator && axis.BoundGhost != null && axis.BoundGhost == ghost)
                {
                    if (axis.CurrentStage < OrbitalBookStages.StartAscending)
                        axis.CurrentStage = OrbitalBookStages.StartAscending;
                }
            }
        }
    }

    void AddBookToOrbit(BookDeactivating bookDeactivator)
    {
        foreach (BookAxis bookAxis in _bookAxes)
        {
            if (bookAxis.BookContainer.transform.childCount == 0)
            {
                Quaternion bookRotation = Quaternion.Euler(-20f, 180f, 0);
                Vector3 bookPosition = new Vector3(0f, 0f, 0f);
                Book book = bookDeactivator.GetComponent<Book>();
                GameObject bookModel = Instantiate(book.BookModel, bookAxis.BookContainer.position, bookRotation, bookAxis.BookContainer);
                bookModel.GetComponent<IdleRotater>().enabled = false;
                bookModel.SetActive(true);
                bookAxis.BookAnimator = bookModel.GetComponent<Animator>();
                bookAxis.BoundGhost = book.DefaultOwner.transform;
                bookModel.transform.localPosition = bookPosition;
                bookModel.transform.localRotation = bookRotation;
                bookAxis.BookAnimator.SetTrigger("Deactivate");
                bookAxis.BookIsClosed = true;

                _pickedBooks.Add(bookModel);

                if (_pickedBooks.Count > 2)
                {
                    SetThreeBookAxes(_bookAxes[1], bookAxis);
                }
                else
                {
                    bookAxis.BookAppear();
                }        

                break;
            }
        }
    }

    void SetThreeBookAxes(BookAxis axisToRotate, BookAxis axisToAppear)
    {
        StartCoroutine(RotateAxis(axisToRotate.transform, Quaternion.Euler(0, 120f, 0)));
        axisToRotate.AngleInOrbit = 120f;
        axisToAppear.BookAppear();

        //1.y = 0, 2.y = 120, 3.y = 240
    }

    IEnumerator RotateAxis(Transform axis, Quaternion angle)
    {
        while(Quaternion.Angle(axis.localRotation, angle) > 2f)
        {
            axis.localRotation = Quaternion.Slerp(axis.localRotation, angle, 1f * Time.deltaTime);
            yield return null;
        }

        axis.localRotation = angle;
    }

    Vector3 GetPositionToMove(BookAxis axis, out Vector3 bookOffset)
    {
        Vector3 axisPosition = axis.transform.position;
        bookOffset = axis.BookContainer.localPosition;

        switch (_ascendingBooks.Count)
        {
            //Если взводятся две книги одновременно
            case 2 :
            {
                switch (_ascendingBooks.IndexOf(axis.transform))
                {
                    //Если эта книга начала взведение первой (первой попала в список)
                    case 0 :
                    {
                        int index = 0;
                        bookOffset = new Vector3(bookOffset.x, bookOffset.y, _bookSlots[index].localPosition.z);
                        return new Vector3(axisPosition.x, _bookSlots[index].position.y, axisPosition.z);
                    }
                    case 1 :
                    {
                        int index = 1;
                        bookOffset = new Vector3(bookOffset.x, bookOffset.y, _bookSlots[index].localPosition.z);
                        return new Vector3(axisPosition.x, _bookSlots[index].position.y, axisPosition.z);
                    }
                }
                break;
            }
            //Если взводятся три книги одновременно
            case 3 :
            {
                switch (_ascendingBooks.IndexOf(axis.transform))
                {
                    case 0 :
                    {
                        int index = 0;
                        bookOffset = new Vector3(bookOffset.x, bookOffset.y, _bookSlots[index].localPosition.z);
                        return new Vector3(axisPosition.x, _bookSlots[index].position.y, axisPosition.z);
                    }
                    case 1 :
                    {
                        int index = 1;
                        bookOffset = new Vector3(bookOffset.x, bookOffset.y, _bookSlots[index].localPosition.z);
                        return new Vector3(axisPosition.x, _bookSlots[index].position.y, axisPosition.z);
                    }
                    case 2 :
                    {
                        int index = 2;
                        bookOffset = new Vector3(bookOffset.x, bookOffset.y, _bookSlots[index].localPosition.z);
                        return new Vector3(axisPosition.x, _bookSlots[index].position.y, axisPosition.z);
                    }
                }
                break;
            }
        }
        bookOffset = new Vector3(bookOffset.x, bookOffset.y, _bookSlots[0].localPosition.z);
        return new Vector3(axisPosition.x, _bookSlots[0].position.y, axisPosition.z);
    } 

    void TiltBookByLocalX(Transform book, float angle)
    {
        float rotationSpeed = 15f;
        Quaternion rotation = Quaternion.Euler(angle, book.localEulerAngles.y, book.localEulerAngles.z);
        book.localRotation = Quaternion.Slerp(book.localRotation, rotation, rotationSpeed * Time.deltaTime);

        if (Quaternion.Angle(book.localRotation, rotation) < 1f)
        {
            book.localRotation = rotation;
        }
    }

    Quaternion GetRotationToTarget(Transform self, Transform target)
    {
        Vector3 targetPosition = new Vector3(target.position.x, 0f, target.position.z);
        Vector3 selfPosition = new Vector3(self.position.x, 0f, self.position.z);
        Vector3 direction = (targetPosition - selfPosition).normalized;
        return Quaternion.LookRotation(direction);
    }

    //bool 

    void DeactivateOrbit(UnitInfo info)
    {
        foreach(BookAxis axis in _bookAxes)
        {
            axis.BookDisappear();
        }

        this.enabled = false;
    }
}

public enum OrbitalBookStages
{
    StartReturning,
    IsReturning,
    InOrbit,
    StartAscending,
    IsAscending,
    WasAscended
}
