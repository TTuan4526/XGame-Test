using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager ins;

    private void Awake()
    {
        ins = this;
    }

    public GameObject InGameScreen;
    public GameObject GameOverScreen;
    public GameObject PauseScreen;

    public Text scoreTxt, highScoreTxt;

    [Header("Ingame Btn")]
    public Button pauseBtn;
    public Button rotateBtn;
    public Button fxBtn;
    public Button musicBtn;

    [Header("Pause Btn")]
    public Button restartBtn;
    public Button resumeBtn;
    public Button pauseMenuBtn;

    [Header("GameOver Btn")]
    public Button replayBtn;
    public Button gameOverMenuBtn;

    // Start is called before the first frame update
    void Start()
    {
        if (!InGameScreen.activeSelf)
        {
            InGameScreen.SetActive(true);
        }

        if (GameOverScreen.activeSelf)
        {
            GameOverScreen.SetActive(false);
        }

        if (PauseScreen.activeSelf)
        {
            PauseScreen.SetActive(false);
        }

        //InGame Screen
        pauseBtn.onClick.AddListener(OnClickPauseBtn);
        rotateBtn.onClick.AddListener(OnClickRotateBtn);
        fxBtn.onClick.AddListener(OnClickFxBtn);
        musicBtn.onClick.AddListener(OnClickMusicBtn);

        //Pause Screen
        restartBtn.onClick.AddListener(OnClickRestartBtn);
        resumeBtn.onClick.AddListener(OnClickResumeBtn);
        pauseMenuBtn.onClick.AddListener(OnClickPauseMenuBtn);

        //GameOver Screen
        replayBtn.onClick.AddListener(OnClickReplayBtn);
        gameOverMenuBtn.onClick.AddListener(OnClickGameOverMenuBtn);

        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        string highScoreString = highScore.ToString();
        highScoreTxt.text = highScoreString;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //InGame Screen Function
    public void OnClickPauseBtn()
    {
        if (!PauseScreen.activeSelf)
        {
            PauseScreen.SetActive(true);

            Time.timeScale = 0;
        }
    }

    public void OnClickRotateBtn()
    {
        GameManager.ins.ToggleRotationDirection();
    }

    public void OnClickFxBtn()
    {
        SoundManager.ins.ToggleFx();
    }

    public void OnClickMusicBtn()
    {
        SoundManager.ins.ToggleMusic();
    }

    //Pause Screen Function
    public void OnClickRestartBtn()
    {
        GameManager.ins.Restart();
    }

    public void OnClickResumeBtn()
    {
        if (PauseScreen.activeSelf)
        {
            PauseScreen.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void OnClickPauseMenuBtn()
    {
        if (PauseScreen.activeSelf)
        {
            PauseScreen.SetActive(false);
            Time.timeScale = 1;
        }

        if (InGameScreen.activeSelf)
        {
            InGameScreen.SetActive(false);
        }

        SceneManager.LoadScene("Main Menu");
    }

    public void OnClickReplayBtn()
    {
        GameManager.ins.Restart();
    }

    public void OnClickGameOverMenuBtn()
    {
        if (GameOverScreen.activeSelf)
        {
            GameOverScreen.SetActive(false);
            Time.timeScale = 1;
        }

        if (InGameScreen.activeSelf)
        {
            InGameScreen.SetActive(false);
        }

        SceneManager.LoadScene("Main Menu");
    }
}
