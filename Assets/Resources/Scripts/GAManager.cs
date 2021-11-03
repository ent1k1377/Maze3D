using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;

public class GAManager : MonoBehaviour
{
    void Awake ()
    {
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        GameAnalytics.Initialize();
    }
}
