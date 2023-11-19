using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    public Text scoreText, highScoreText, highscoreBlinker;
    public GameObject adsBtn;
    public Sprite unAvailBtnSprite;

    public void refresh(int score, bool canContinue = false)
    {
        Debug.Log("refresh");
        scoreText.text = "" + score;
        int highScore = PrefManager.CheckAndSetHighScore(score);
        highScoreText.text = "Highscore: " + highScore;

        if (score >= highScore)
        {
            highscoreBlinker.gameObject.SetActive(true);
        }

        if (!canContinue)
        {
            adsBtn.GetComponent<Button>().interactable = false;
            adsBtn.GetComponent<Image>().sprite = unAvailBtnSprite;
        }
    }
}
