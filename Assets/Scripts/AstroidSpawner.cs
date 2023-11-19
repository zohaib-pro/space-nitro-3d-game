using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstroidSpawner : MonoBehaviour
{

    public GameObject[] asteroidsPrefabs;
    public float secondsBetweenPrefabs = 1.5f;
    public Vector2 forceRange;
    public Camera cam;
    float timer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime; 
        if (timer < 0)
        {
            spawnAsteroid();
            timer += secondsBetweenPrefabs;
        }
    }

    private void spawnAsteroid()
    {
        
    }

    private void doNothing()
    {
        throw new NotImplementedException();
    }
}
