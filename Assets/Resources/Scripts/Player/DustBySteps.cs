using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustBySteps : MonoBehaviour
{
    [SerializeField] Transform _effectsPool;

    List<GameObject> _dustEffects = new List<GameObject>();
    

    void Start()
    {
        foreach (Transform child in _effectsPool)
        {
            _dustEffects.Add(child.gameObject);
        }
    }

    void CreateStepEffect()
    {
        foreach (GameObject dust in _dustEffects)
        {
            if (!dust.activeSelf)
            {
                Vector3 position = new Vector3(transform.position.x, dust.transform.position.y, transform.position.z);
                Quaternion rotation = Quaternion.Euler(0, transform.eulerAngles.y + 180f, 0);
                dust.transform.position = position;
                dust.transform.rotation = rotation;
                dust.SetActive(true);
                break;
            }
        }
    }
}
