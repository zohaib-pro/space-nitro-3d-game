using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameUiManager : MonoBehaviour
{

    
    public GameObject controlBtns, pauseBtn, resumeBtn, gameLauncher;
    public GameObject results;
    public GameObject pauseMenu, boosterShow;
    public GameObject ps1, ps2;
    public Text highScoreText, debugText;

    private string debugStr= "";

    private GameObject player;
    private float dist = 0.0f;

    private int resultShowCount = 0;

    

    // Start is called before the first frame update
    void Start()
    {
        highScoreText.text = String.Format("High Score\n\n{0}", PrefManager.GetHighScore());
        dist = Math.Abs(ps1.transform.position.z - ps2.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
            return;

        pushStarPoolForward(ps1);
        pushStarPoolForward(ps2);
    }

    public void setPlayer(GameObject player)
    {
        this.player = player;
    }

    public void showPauseBtn(bool isShow)
    {
        pauseBtn.SetActive(isShow);
    }

    public void showControlBtns(bool isShow)
    {
        controlBtns.SetActive(isShow);
    }

    public void showGameLauncher(bool isShow)
    {
        gameLauncher.SetActive(isShow);
    }

    public void boosterTakeShow(bool isShow)
    {
        boosterShow.SetActive(isShow);
    }

    public void showResults(int score, float afterTime)
    {

        StartCoroutine(MyCoroutine(score, afterTime));
    }

    public void hideResults()
    {
        results.SetActive(false);
    }

    public void showPauseMenu(bool isShow)
    {
        pauseMenu.SetActive(isShow);
        pauseBtn.SetActive(!isShow);
        resumeBtn.SetActive(isShow);
    }

    public void Debug(string str)
    {
        debugStr += "\n" + str;
        debugText.text = debugStr;
    }


    //private functions
    private IEnumerator MyCoroutine(int score, float afterTime)
    {
        yield return new WaitForSeconds(afterTime);

        results.GetComponent<ResultManager>().refresh((int)score, resultShowCount++ == 0);
        results.SetActive(true);
    }
   
    private void pushStarPoolForward(GameObject ps)
    {
        if (player.transform.position.z > ps.transform.position.z + dist)
        {
            Vector3 v = ps.transform.position;
            ps.transform.position = new Vector3(v.x, v.y, v.z + dist * 2);
        }
    }


}
