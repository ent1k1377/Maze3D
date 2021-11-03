using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMoveHandler : MonoBehaviour
{
    [SerializeField] PauseMenu _pauseMenu;
    [SerializeField] PauseMenuButton _pauseButton;
    [SerializeField] ResumeButton _resumeButton;
    [SerializeField] MainMenuButton _mainMenuButton;
    [SerializeField] SubstrateToActivateGameplay _gameplaySubstrate;
    [SerializeField] DefeatMenu _defeatMenu;
    [SerializeField] VictoryMenu _victoryMenu;
    [SerializeField] List<LoadLevelButton> _loadLevelButtons = new List<LoadLevelButton>();

    [SerializeField] RectTransform _booksUI;

    RectTransform _pauseButtonRT;

    //string _prefName = "HiddenUI";

    Vector2 _defaultPauseButtonPosition;
    Vector2 _positionToSlidePauseButton;
    Vector2 _defaultBooksPosition;
    Vector2 _positionToSlideBooks;

    Coroutine _hidingPauseButton;
    Coroutine _showingPauseButton;
    Coroutine _hidingBookUI;
    Coroutine _showingBookUI;



    void Start()
    {
        _pauseButton.OnActivatingPause += ShowPauseMenu;
        _resumeButton.OnDeactivatingPause += HidePauseMenu;
        _mainMenuButton.OnActivatingMainMenu += TurnToMainMenu;
        _gameplaySubstrate.OnActivatingGameplay += TurnToGameplay;
        _victoryMenu.OnVictory += HidePauseButton;
        _defeatMenu.OnDefeat += HidePauseButton;

        if (_loadLevelButtons.Count > 0)
        {
            foreach (LoadLevelButton button in _loadLevelButtons)
            {
                button.OnLoadLevel += HideBookUI;
            }
        }

        _pauseButtonRT = _pauseButton.GetComponent<RectTransform>();
        
        _defaultPauseButtonPosition = _pauseButtonRT.anchoredPosition;
        _positionToSlidePauseButton = new Vector2(-60, _defaultPauseButtonPosition.y);
    
        _defaultBooksPosition = _booksUI.anchoredPosition;
        _positionToSlideBooks = new Vector2(_defaultBooksPosition.x, 30);  
        
        //if (PlayerPrefs.HasKey(_prefName) && PlayerPrefs.GetInt(_prefName) == 1)
        //{
        //    _booksUI.anchoredPosition = _positionToSlideBooks;
        //    _pauseButtonRT.anchoredPosition = _positionToSlidePauseButton;
        //    _gameplaySubstrate.gameObject.SetActive(true);
        //}

        _booksUI.anchoredPosition = _positionToSlideBooks;
        _pauseButtonRT.anchoredPosition = _positionToSlidePauseButton;
    }

    void TurnToGameplay()
    {
        ShowPauseButton();
        ShowBookUI();
        //PlayerPrefs.SetInt(_prefName, 0);
    }

    void TurnToMainMenu()
    {
        HidePauseButton();
        HideBookUI();
        _pauseMenu.DisablePauseMenu();
        StartCoroutine(WaitAndReloadLevel(0.5f));
        //PlayerPrefs.SetInt(_prefName, 1);
    }

    void HidePauseMenu()
    {
        _gameplaySubstrate.gameObject.SetActive(true);
        ShowPauseButton();
        _pauseMenu.DisablePauseMenu();
    }

    void ShowPauseMenu()
    {
        _gameplaySubstrate.gameObject.SetActive(false);
        HidePauseButton();
        _pauseMenu.CallPauseMenu();
    }

    void HideBookUI()
    {
        if (_hidingBookUI == null)
        {
            if (_showingBookUI != null)
            {
                StopCoroutine(_showingBookUI);
                _showingBookUI = null;
            }
            _hidingBookUI = StartCoroutine(SlideToPosition(_booksUI, _booksUI.anchoredPosition, _positionToSlideBooks, 4f, _hidingBookUI));
        }
    }

    void ShowBookUI()
    {
        if (_showingBookUI == null)
        {
            if (_hidingBookUI != null)
            {
                StopCoroutine(_hidingBookUI);
                _hidingBookUI = null;
            }
            _showingBookUI = StartCoroutine(SlideToPosition(_booksUI, _booksUI.anchoredPosition, _defaultBooksPosition, 4.5f, _showingBookUI));
        }
    }

    void HidePauseButton()
    {
        if (_hidingPauseButton == null)
        {
            if (_showingPauseButton != null)
            {
                StopCoroutine(_showingPauseButton);
                _showingPauseButton = null;
            }
            _hidingPauseButton = StartCoroutine(SlideToPosition(_pauseButtonRT, _pauseButtonRT.anchoredPosition, _positionToSlidePauseButton, 8f, _hidingPauseButton));
        }
    }

    void ShowPauseButton()
    {
        if (_showingPauseButton == null)
        {
            if (_hidingPauseButton != null)
            {
                StopCoroutine(_hidingPauseButton);
                _hidingPauseButton = null;
            }
            _showingPauseButton = StartCoroutine(SlideToPosition(_pauseButtonRT, _pauseButtonRT.anchoredPosition, _defaultPauseButtonPosition, 4f, _showingPauseButton));
        }
    }

    IEnumerator WaitAndReloadLevel(float duration)
    {
        yield return new WaitForSeconds(duration);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator SlideToPosition(RectTransform UIObject, Vector2 from, Vector2 to, float speed, Coroutine coroutine)
    {
        float step = 0f;
        while (step < 1f)
        {
            UIObject.anchoredPosition = Vector2.Lerp(from, to, step);
            step += speed * Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        coroutine = null;
    }
}
