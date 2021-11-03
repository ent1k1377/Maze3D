using UnityEngine;

public static class SavePrefs
{
    public static int SeedNumber;

    private static bool _isTestLaunch;

    private static string _levelPrefsName = "CurrentLevel";
    private static string _coinsPrefsName = "PlayerCoins";


    public static void SetLevelSeed()
    {
        if (SeedNumber < 0)
        {
            SeedNumber = 1;

            PlayerPrefs.SetInt(_levelPrefsName, SeedNumber);
            PlayerPrefs.SetInt(_coinsPrefsName, 0);

            Random.InitState(SeedNumber);
            _isTestLaunch = false;
        }
        else if (SeedNumber == 0)
        {
            if (PlayerPrefs.HasKey(_levelPrefsName) && PlayerPrefs.GetInt(_levelPrefsName) > 0)
            {
                SeedNumber = PlayerPrefs.GetInt(_levelPrefsName);
            }
            else
            {
                SeedNumber = 1;
            }
            Random.InitState(SeedNumber);
            _isTestLaunch = false;
        }
        else if (SeedNumber > 0)
        {
            //Random.InitState(SeedNumber);
            _isTestLaunch = true;
        }
    }

    public static void SaveCurrentLevel()
    {
        if (SeedNumber > 0 && _isTestLaunch == false)
            PlayerPrefs.SetInt(_levelPrefsName, SeedNumber);
    }
    
    public static void SaveCoins(int coins)
    {
        if (_isTestLaunch == false)
            PlayerPrefs.SetInt(_coinsPrefsName, coins);
    }

    public static int LoadCoins()
    {
        if (PlayerPrefs.HasKey(_coinsPrefsName))
            return PlayerPrefs.GetInt(_coinsPrefsName);
        else
            return 0;
    }
}
