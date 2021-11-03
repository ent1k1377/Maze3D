using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarsTemplateManager : MonoBehaviour
{
    [SerializeField] List<GameObject> _templates = new List<GameObject>();


    void Start()
    {
        GameObject randomTemplate = _templates[Random.Range(0, _templates.Count)];
        randomTemplate.SetActive(true);
    }
}
