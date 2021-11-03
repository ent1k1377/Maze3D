using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInfo : MonoBehaviour
{
    public delegate void Dying(UnitInfo info);
    public event Dying OnDying;

    public bool isPlayer;

    public List<Book> Books = new List<Book>();
    public bool haveBook
    {
        get
        {
            if (Books.Count > 0)
                return true;
            else
                return false;
        }
    }

    [SerializeField] Material _deathParticlesMaterial;
    [SerializeField] Transform _deathEffectsPool;

    List<GameObject> _deathEffects = new List<GameObject>();


    void Start()
    {
        BoundBooksWithOwner();

        foreach (Transform child in _deathEffectsPool)
        {
            _deathEffects.Add(child.gameObject);
        }
    }

    public void BoundBooksWithOwner()
    {
        foreach(Book book in Books)
        {
            if (book.Owner != null && book.Owner != this)
                book.Owner.Books.Remove(book);

            book.Owner = this;
        }
    }

    public void Death()
    {
        OnDying?.Invoke(this);

        foreach (GameObject death in _deathEffects)
        {
            if (!death.activeSelf)
            {
                ParticleSystemRenderer particles = death.GetComponent<ParticleSystemRenderer>();
                particles.material = _deathParticlesMaterial;
                death.transform.position = transform.position;
                death.SetActive(true);
                break;
            }
        }
        
        gameObject.SetActive(false);
    }
}
