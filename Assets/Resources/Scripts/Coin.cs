using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public Transform EffectsPool
    {
        set
        {
            _effectsPool = value;
        }
    }

    [SerializeField] string _playerTag;

    Transform _effectsPool;

    List<GameObject> _pickUpEffects = new List<GameObject>();


    void Start()
    {
        foreach (Transform child in _effectsPool)
        {
            _pickUpEffects.Add(child.gameObject);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == _playerTag)
        {
            if (!collider.GetComponent<CoinsHandler>())
            {
                return;
            }

            CoinsHandler playerCoins = collider.GetComponent<CoinsHandler>();
            playerCoins.AddCoin();

            foreach (GameObject explosion in _pickUpEffects)
            {
                if (!explosion.activeSelf)
                {
                    explosion.transform.position = transform.position;
                    explosion.SetActive(true);
                    break;
                }
            }

            gameObject.SetActive(false);
        }
    }
}
