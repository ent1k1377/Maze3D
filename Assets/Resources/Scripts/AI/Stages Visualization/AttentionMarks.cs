using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttentionMarks : MonoBehaviour
{
    [SerializeField] List<Animator> _animators;

    Coroutine _playingAnims;


    void OnEnable()
    {
        _playingAnims = StartCoroutine(PlayAnimationsRandomly());
    }

    void OnDisable()
    {
        StopCoroutine(_playingAnims);
    }

    IEnumerator PlayAnimationsRandomly()
    {
        while (true)
        {
            int index = Random.Range(0, _animators.Count);
            _animators[index].SetTrigger("Enable");
            yield return new WaitForSeconds(_animators[index].GetCurrentAnimatorStateInfo(0).length * 0.4f);
        }
    }
}
