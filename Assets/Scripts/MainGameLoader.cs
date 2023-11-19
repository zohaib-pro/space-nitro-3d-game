using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameLoader : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject gameLoader;
    void Start()
    {
        gameLoader.GetComponent<GameLoader>().LoadGame(Const.SCENE_MAIN_GAME);
    }
}
