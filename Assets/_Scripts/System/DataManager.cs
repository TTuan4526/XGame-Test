using UnityEngine;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
    public static DataManager ins;

    private int score = 0;

    private void Awake()
    {
        ins = this;
    }

    private void Start() => Reset();

    private void UpdateUIText()
    {
        if (UIManager.ins.scoreTxt)
        {
            UIManager.ins.scoreTxt.text = PadZero(score, 5);
        }
    }

    private string PadZero(int n, int padDigits)
    {
        string nStr = n.ToString();

        while (nStr.Length < padDigits)
        {
            nStr = "0" + nStr;
        }

        return nStr;
    }

    public void ScoreLines(int n)
    {
        switch (n)
        {
            case 1:
                score += 40;
                break;
            case 2:
                score += 100;
                break;
            case 3:
                score += 300;
                break;
            case 4:
                score += 1200;
                break;
        }
        UpdateUIText(); 
    }

    public void Reset()
    {
        score = 0;

        UpdateUIText();
    }

    public void SaveScore()
    {
        int currentScore = score;
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        
        if(currentScore > highScore)
        {
            PlayerPrefs.SetInt("HighScore", currentScore);
            PlayerPrefs.Save();
        }
    }


}
