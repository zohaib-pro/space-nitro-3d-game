using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGenScript : MonoBehaviour
{
    float tObs = 0, tCoin = 0, tBooster = 0, nextBoosterIn;
    private GameObject player;
    public GameObject[] obstaclePrefab, boosterPrefab;
    public GameObject coinPrefab;
    public List<GameObject> obstacles = new();
    public List<GameObject> coins =  new();
    public List<GameObject> boosters = new();

    private int maxSize = 0;
    private float noiseScale = 2f; // Adjust the noise scale to control obstacle spacing
    

    private float screenHalfWidth; // Half of the screen's width in world coordinates
    private float screenHalfHeight; // Half of the screen's height in world coordinates

    private bool isStarted = false;

    // CONSTANTS
    private const float OBSTACLE_ROTATION_PROBABILITY = 0.7f;
    private const int OSBSTACLE_POOL_SIZE = 25, 
        COIN_POOL_SIZE = 15,
        BOOSTER_POOL_SIZE = 5;

    void Start()
    {
        // Calculate the half width and half height of the screen in world coordinates
        screenHalfWidth = Camera.main.aspect * Camera.main.orthographicSize;
        screenHalfHeight = Camera.main.orthographicSize;

        nextBoosterIn = Random.Range(15, 30);

        // create a pool of obstacles
        createPool(obstaclePrefab, obstacles, OSBSTACLE_POOL_SIZE, Const.TAG_OBSTACLE);

        //create a pool of boosters
        createPool(boosterPrefab, boosters, BOOSTER_POOL_SIZE, null);

        //create a pool of coins
        createPool(new GameObject[] { coinPrefab }, coins, COIN_POOL_SIZE, Const.TAG_COIN);

    }

   
    void Update()
    {
        if (!isStarted) return;
        create();
        destroy();
    }

    private void create()
    {
        float xe = Random.Range(-screenHalfWidth * 2, screenHalfWidth * 2); // Random X position within screen width
        float ye = Random.Range(-screenHalfHeight * 2, screenHalfHeight * 2); // Random Y position within screen height
        float noiseValue = Mathf.PerlinNoise(Time.time * noiseScale, 0);
        float ze = player.transform.position.z + 100 + noiseValue * 80;
        tObs += Time.deltaTime;
        tCoin += Time.deltaTime;
        tBooster += Time.deltaTime;

        if (tObs >= 0.35f)
        {
            GameObject obstacle = GetObjectFromPool(obstacles);
            if(obstacle != null)
            {
                obstacle.transform.position = new Vector3(xe, ye, ze);
                obstacle.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                float obstacleScale = Random.Range(0.5f, 1.75f);
                obstacle.transform.localScale = new Vector3(obstacleScale, obstacleScale, obstacleScale);
                if (Random.Range(0f, 1f) <= OBSTACLE_ROTATION_PROBABILITY)
                {
                    obstacle.GetComponent<Spinner>().enabled = true;
                }
            }
            tObs = 0;
        }

        xe = Random.Range(-screenHalfWidth , screenHalfWidth ); // Random X position within screen width
        ye = Random.Range(-screenHalfHeight , screenHalfHeight ); // Random Y position within screen height
        noiseValue = Mathf.PerlinNoise(Time.time * noiseScale, 0);
        ze = player.transform.position.z + 100 + noiseValue * 80;

        if (tCoin >= .75f)
        {
            GameObject newCoin = GetObjectFromPool(coins);
            newCoin.transform.position = new Vector3(xe, player.transform.position.y, ze);
            newCoin.transform.rotation = Quaternion.Euler(90f, 0, 0);
            tCoin = 0f;
        }

        if (tBooster >= nextBoosterIn)
        {
            GameObject newBooster = GetObjectFromPool(boosters, true);
            newBooster.transform.position = new Vector3(xe, player.transform.position.y, ze);
            newBooster.transform.localScale *= 1.5f;

            nextBoosterIn = Random.Range(15, 30);
            tBooster = 0f;
        }

        if (obstacles.Count > maxSize)
        {
            maxSize = obstacles.Count;
            Debug.Log(maxSize);
        }
    }

    private GameObject GetObjectFromPool(List<GameObject> objectPool, bool isRandom = false)
    {
        List<GameObject> availableObjects = new();

        // Find an inactive object in the pool and return it.
        foreach (GameObject obj in objectPool)
        {
            if (!obj.activeInHierarchy)
            {
                if (isRandom)
                {
                    availableObjects.Add(obj);
                }
                else
                {
                    obj.SetActive(true);
                    return obj;
                }   
            }
        }

        if (isRandom && availableObjects.Count > 0)
        {
            int random = Random.Range(0, availableObjects.Count - 1);
            GameObject obj = availableObjects[random];
            obj.SetActive(true);
            return obj;
        }
        return null;
    }


    private void destroy()
    {
        returnObjectsToPool(obstacles);
        returnObjectsToPool(coins);
        returnObjectsToPool(boosters);
    }

    private GameObject getRandom(GameObject[] targetPrefabs)
    {
        int random = ((int)(Random.value * 1000)) % targetPrefabs.Length;
        return targetPrefabs[random];
    }


    private bool isOutOfScreen(GameObject g)
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(g.transform.position);
        return screenPoint.y < 0 || screenPoint.x < 0 || screenPoint.x > Screen.width;
    }

    private void createPool(GameObject[] prefabs, List<GameObject> pool, int poolSize, string tag)
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject prefab = getRandom(prefabs);
            GameObject newPoolObject = Instantiate(prefab, new Vector3(0, 0, -50), Quaternion.identity);
            if (tag != null)
                newPoolObject.tag = tag;
            newPoolObject.SetActive(false);
            pool.Add(newPoolObject);
        }
    }
    private void returnObjectsToPool(List<GameObject> objects)
    {
        foreach (GameObject obj in objects)
        {
            // Calculate the screen boundaries
            if (isOutOfScreen(obj))
            {
                // Deactivate and reset the object and return it to the pool.
                obj.SetActive(false);
                Spinner spinner = obj.GetComponent<Spinner>();
                if (spinner != null)
                    spinner.enabled = false;
                obj.transform.position = Vector3.zero; // Reset position
                obj.transform.rotation = Quaternion.identity; // Reset rotation
            }
        }
    }

    public void startGeneration(GameObject player)
    {
        this.player = player;
        isStarted = true;
    }
}
