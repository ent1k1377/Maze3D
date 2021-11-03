using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : MonoBehaviour
{
    public UnitInfo Owner;
    public UnitInfo DefaultOwner;
    public GameObject BookModel;
    public Sprite BookSprite;

    [HideInInspector] public UnitInfo Keeper;
    [HideInInspector] public Chunk OriginChunk;
}
