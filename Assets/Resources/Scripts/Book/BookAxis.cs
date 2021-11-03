using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookAxis : MonoBehaviour
{
    [HideInInspector] public bool BookIsClosed = false;
    [HideInInspector] public OrbitalBookStages CurrentStage;
    [HideInInspector] public int IndexOfAscend = -1;

    public float AngleInOrbit;
    public Transform BookContainer;
    public Transform BoundGhost;
    public Animator BookAnimator;

    [SerializeField] Animator _containerAnimator;


    void Awake()
    {
        CurrentStage = OrbitalBookStages.InOrbit;
    }

    public void BookDisappear()
    {
        _containerAnimator.SetTrigger("Disappear");
    }

    public void BookAppear()
    {
        _containerAnimator.SetTrigger("Appear");
    }
}
