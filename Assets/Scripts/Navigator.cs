using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Navigator : MonoBehaviour
{
    public GameObject GameLoaderObject;
    public void gotoShop()
    {
        load(Const.SCENE_SHOP);
    }

    public void gotoGame()
    {
        load(Const.SCENE_MAIN_GAME);
    }

    public void gotoSettings()
    {
        load(Const.SCENE_SETTINGS);
    }

    private void load(string sceneName)
    {
        if (Time.timeScale == 0)
            Time.timeScale = 1;
        if (GameLoaderObject != null)
        {
            GameLoaderObject.SetActive(true);
            GameLoaderObject.GetComponent<GameLoader>().LoadGame(sceneName);
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
