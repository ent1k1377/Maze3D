using UnityEngine;
using UnityEngine.UI;

public class LevelNumber : MonoBehaviour
{
    Text _levelText;

    void Start()
    {
        _levelText = GetComponent<Text>();
        
        _levelText.text = "LEVEL " + SavePrefs.SeedNumber;
    }
}
