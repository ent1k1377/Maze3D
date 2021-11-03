using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarScaler : MonoBehaviour
{
    List<Transform> _pillars = new List<Transform>();


    void Start()
    {
        foreach (Transform child in transform)
        {
            RandomizePillarScale(child.GetComponent<Pillar>());
        }
    }

    void RandomizePillarScale(Pillar pillar)
    {
        Transform pillarTransform = pillar.transform;
        float defaultHeight = pillarTransform.localScale.y;
        float differenceToChangePosition;
        Vector3 scale = pillarTransform.localScale;
        Vector3 position = pillarTransform.localPosition;

        scale.y = Random.Range(pillar.MinHeight, pillar.MaxHeight);
        if (scale.y > 25f)
        {
            scale.y = 50f;
        }

        differenceToChangePosition = (scale.y - defaultHeight) / 2;
        position.y += differenceToChangePosition;

        pillarTransform.localScale = scale;
        pillarTransform.localPosition = position;
    }
}
