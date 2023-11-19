using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //public members
    
    public float forceMagnitude, maxVelocity, rotationSpeed;
    
    public int scoreMultiplier = 1;

    //public Canvas controlCanvas;
    public Text msgText, coinText, coinDoubleText;
    public Camera cam;
    public GameObject shipContainer;
    public GameObject coinDestructionEffect, shipDestructionEffect, obstacleDestructionEffect;

    //tutorial
    public GameObject tutorial,
        msgContainer,
        gameStartPointer,
        tutorialSprite,
        tutorialObstacles,
        tutorialBooster,
        scoreArrow,
        tutorialSkipper;
    public Text tutorialMsgText, continueText;

    private bool tutorialStarted = false;

    private int tutorialIdx = 0;
    private delegate void OnComplete();


    //end tutorial
    
    public AudioSource coinCollectAudio, 
        boosterCollectAudio, 
        crashAudio, 
        engineAudio, 
        engineWorkingAudio, 
        nitroBoostAudio,
        bgMusic;
    public Slider slider;
    public const float minAltitude = 1f;

    

    float turnSpeed = 20.0f; // Use a float for moveSpeed
    float maxTiltAngle = 90.0f; // Maximum tilt angle for the plane

    //private members
    private Vector3 movementDirection;
    private float screenHalfWidth; // Half of the screen's width in world coordinates
    private float screenHalfHeight; // Half of the screen's height in world coordinates
    private float planeWidth;
    private float score = 0.0f;


    private bool isStable = false,
        cameraAttached = false,
        isStarted = false,
        isFinished = false,
        isTutorialCompleted = false,
        isMusic,
        isSfx;

    private int left = 0,
        right = 0;

    private string controlType;

    private Rigidbody rb;
    private GameObject ship;
    private CameraFollow cameraFollow;
    private GameObject lastCollidedWith; //need to destroy it after game resumed after playing ad
    private float turnPower = 0f;

    private int coins = 0;

    private Booster coinDoubleBooster, invincibleBooster, scoreBooster;

    private GameUiManager uiManager;

    // Start is called before the first frame update
    void Start()
    {

        isTutorialCompleted =  PrefManager.IsTutorialCompleted();
        if (!isTutorialCompleted)
        {
            gameStartPointer.SetActive(true);
        }
        //don't let the screen to sleep
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        //get the music and sfx information
        isSfx = PrefManager.IsSfxOn();
        isMusic = PrefManager.IsMusicOn();

        if (isMusic)
            bgMusic.Play();


        //instantiate the selected ship and prepare it for playing
        Transform shipContainerTransform = shipContainer.transform;
        int selectedShip = PrefManager.GetSelectedShip();
        GameObject temp = shipContainerTransform.GetChild(selectedShip).gameObject;
        ship = Instantiate(temp, new Vector3(0, 1.8f, 0), Quaternion.Euler(-90,0,0));
        //shield.transform.position = ship.transform.position;
        //shield.transform.parent = ship.transform;
        ship.tag = Const.TAG_PLAYER;
        ship.AddComponent<HitManager>();
        rb = ship.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = ship.AddComponent<Rigidbody>();    
        }
        rb.useGravity = false;
        ship.SetActive(true);

        Spinner spinner =  ship.AddComponent<Spinner>();
        spinner.setRotation(0, 0, 2);

        //some neccessary calculations
        screenHalfWidth = Camera.main.aspect * Camera.main.orthographicSize;
        screenHalfHeight = Camera.main.orthographicSize;
        cameraFollow = cam.GetComponent<CameraFollow>();

        measurePlanWidth();

        msgText.text = "Not Started!";
        coinText.text = "" + PrefManager.GetPlayerCoins();

        uiManager = GetComponent<GameUiManager>();

        

    }


    // Update is called once per frame


    
    void Update()
    {
        if (!isTutorialCompleted)
            manageTutorial();

        checkAndApplyBoosters();

        handleMusic();

        rotateToFaceVelocity();

        handlePlayer();
        
    }

    private void manageTutorial()
    {
        if (isStable && !isShowingMsg)
        {
            msgContainer.SetActive(true);
            tutorialSkipper.SetActive(true);

            if (!tutorialStarted && Math.Abs(ship.transform.position.x) > 2)
            {
                
                tutorialIdx = 1;
                tutorialStarted = true;
                tutorialSprite.SetActive(false);
                SetTutorialMsg("Good Job!\n\nYou can change control in settings", 3f, ()=>{
                    tutorialMsgText.text = "Control ship and avoid collision with asteroids!";
                    showTutorialObstacles();
                });
            }
        }
    }

    private bool isShowingMsg = false;

    private void SetTutorialMsg(string msg, float forTime, OnComplete onComplete)
    {
        StartCoroutine(GetTutorialMsgCoroutine(msg, forTime, onComplete));
    }
    private IEnumerator GetTutorialMsgCoroutine(string msg, float forTime, OnComplete onComplete)
    {
        isShowingMsg = true;
        tutorialMsgText.text = msg;

        yield return new WaitForSeconds(forTime);

        isShowingMsg = false;
        onComplete();

    }

    

    private void handlePlayer()
    {
        if (isStable && !isFinished)
        {

            handleTilt(getKeyboard());
            if (controlType == Const.CONTROL_TILT)
                handleTilt(getAccelerometerInput());
            else
                handleTilt(getButtonsInput());
            
            if (isStarted)
                updateScore();
        }
    }

    private void handleMusic()
    {
        if (!isStarted && isMusic && bgMusic.time >= bgMusic.clip.length * 0.05f)
        {
            // Restart the audio by setting the time to 0
            bgMusic.time = 0f;
        }

        if (engineWorkingAudio.time >= engineWorkingAudio.clip.length * 0.95f)
        {
            engineWorkingAudio.time = engineWorkingAudio.clip.length * 0.01f;
        }
    }

    private void checkAndApplyBoosters()
    {
        if (coinDoubleBooster != null)
        {
            coinDoubleBooster.update(Time.deltaTime);
            if (!coinDoubleBooster.isActive)
            {
                coinDoubleBooster = null; //destroy the unused booster
            }
        }

        if (invincibleBooster != null)
        {
            invincibleBooster.update(Time.deltaTime);
            if (!invincibleBooster.isActive)
            {
                setActiveShield(false);
                invincibleBooster = null; //destroy the unused booster
            }
        }

        if (scoreBooster != null)
        {
            scoreBooster.update(Time.deltaTime);
            if (!scoreBooster.isActive)
            {
                setThrusters(false);
                scoreBooster = null;
            }
        }
    }

    private bool IsApproximatelyIdentity(Quaternion quaternion, float tolerance = 0.01f)
    {
        // Calculate the angle of rotation from the quaternion
        float angle = Quaternion.Angle(quaternion, Quaternion.identity);

        // Check if the angle is within the tolerance
        return Mathf.Abs(angle) < tolerance;
    }


    private void measurePlanWidth()
    {
        // Assuming the plane has a MeshRenderer component
        MeshRenderer planeRenderer = ship.GetComponent<MeshRenderer>();
        if (planeRenderer != null)
        {
            planeWidth = planeRenderer.bounds.size.x;
        }
        else
        {
            // If the plane doesn't have a MeshRenderer, you can try using a Collider
            Collider planeCollider = GetComponent<Collider>();
            if (planeCollider != null)
            {
                planeWidth = planeCollider.bounds.size.x;
            }
            else
            {
                // If neither a MeshRenderer nor a Collider is found, you may need to adjust this part based on your specific setup.
                Debug.LogError("Plane doesn't have a MeshRenderer or Collider.");
            }
        }
    }

    private void rotateToFaceVelocity()
    {
        if (!isStarted || isFinished)
            return;

        if (!isStable )
        {
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward);
            Quaternion remainingQuaternion = Quaternion.Slerp(ship.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            ship.transform.rotation = remainingQuaternion;
            msgText.text = "Stablizing";
            //float angleMagnitude = Mathf.Abs(remainingQuaternion.eulerAngles.magnitude);
            //Debug.Log(angleMagnitude);
            if (IsApproximatelyIdentity(remainingQuaternion))
            {
                ship.transform.rotation = Quaternion.identity;

                isStable = true;

                startPlay();

                msgText.text = "stable";

                if (!cameraAttached)
                {
                    Debug.Log("attaching Camera");
                    cameraAttached = true;
                    Invoke(nameof(attachCam), 1f);
                }

            }
        }
        else
        {
            msgText.text = "Adjusting Alt";
            if (ship.transform.position.y > minAltitude)
            {
                Vector3 targetPosition = new Vector3(0, 0, 0);
                ship.transform.position = Vector3.Lerp(ship.transform.position, targetPosition, Time.deltaTime * rotationSpeed);
            }
            else
            {
                msgText.text = "Altitude Adjusted";
            }
        }
        
    }

    private Vector3 getAccelerometerInput()
    {
        return Input.acceleration;
    }

    private Vector3 getButtonsInput()
    {
        int turnDir = left + right;

        
        turnPower = Mathf.Max(turnPower + 0.008f, 0.25f);
        return new Vector3(turnDir, 0,0) * turnPower;    
    }

    Vector3 lastmouse = Vector3.zero;
    private Vector3 getMouseInput()
    {
        // Check if the left mouse button is pressed
        if (Input.GetMouseButtonDown(0))
        {
            // Get the mouse position in screen coordinates
            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.x -= screenHalfWidth;
            // Convert the screen position to world space
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

            lastmouse = mouseWorldPosition * 10 ;
            
            //Debug.Log(mouseWorldPosition);
            return lastmouse;
        }

        return lastmouse;
    }


    private Vector3 getKeyboard()
    {
        return new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * 0.5f;
    }

    void FixedUpdate()
    {
        //Debug.Log("fixed update");
        if (!isStable || !isStarted)
        {
            return;
        }
            

        Vector3 force = /*new Vector3(dx, 0, 1)*/ Vector3.forward * forceMagnitude * Time.deltaTime;
        rb.AddForce(force, ForceMode.Force);

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);
    }

    void attachCam()
    {
        cameraFollow.setTarget(ship.transform);
        uiManager.setPlayer(ship);
    }

    private void startPlay()
    {
        GetComponent<GameUiManager>().showPauseBtn(true);
        if (!isStable)
            engineWorkingAudio.Play();
        ObstacleGenScript generator = GetComponent<ObstacleGenScript>();
        if(generator != null && isTutorialCompleted)
        {
            generator.startGeneration(ship);
        }
    }

    public void handle()
    {
        float moveSpeed = 100;
        float moveHorizontal = Input.GetAxis("Horizontal");
        // Calculate movement vector
        Vector3 movement = new Vector3(moveHorizontal, 0f, 0f);

        // Normalize the movement vector and scale it by moveSpeed
        movement = movement.normalized * moveSpeed * Time.deltaTime;

        // Move the player
        ship.transform.Translate(movement);
    }

    private void handleTilt(Vector3 input)
    {
        float horizontalInput = input.x;
        float verticalInput = input.y = 0;

        //Debug.Log(accelerometerInput);

        // Calculate the target tilt angle based on accelerometer input
        float targetTiltAngle = Mathf.Clamp(horizontalInput * maxTiltAngle, -maxTiltAngle, maxTiltAngle);

        // Apply the tilt to the plane's rotation around the Z-axis
        ship.transform.rotation = Quaternion.Slerp(ship.transform.rotation, Quaternion.Euler(0.0f, 0.0f, -targetTiltAngle), Time.deltaTime * rotationSpeed);

        // Calculate the desired horizontal movement
        float horizontalMovement = horizontalInput * turnSpeed * Time.deltaTime;
        float verticalMovement = verticalInput * (turnSpeed/2) * Time.deltaTime;


        // Calculate the plane's new position
        Vector3 newPosition = ship.transform.position + new Vector3(horizontalMovement, verticalMovement, 0.0f);

        // Calculate the screen boundaries in world coordinates
        float halfPlaneWidth = planeWidth / 2.0f;
        float leftBoundary = -screenHalfWidth + halfPlaneWidth;
        float rightBoundary = screenHalfWidth - halfPlaneWidth;

        // Clamp the plane's position within the screen boundaries
        newPosition.x = Mathf.Clamp(newPosition.x, leftBoundary, rightBoundary);

        // Apply the new position
        ship.transform.position = newPosition;

        //msgText.text = "move: " + accelerometerInput;
    }

    private void updateScore()
    {
        if (!isStarted)
            return;
        score +=  Time.deltaTime * scoreMultiplier;
        msgText.text = "score: " + ((int) score);
    }


    public void startEngine()
    {
        gameStartPointer.SetActive(false);
        bgMusic.time = 0f;
        //set the control type
        controlType = PrefManager.GetControlType();
        if (controlType == Const.CONTROL_BUTTONS)
        {
            uiManager.showControlBtns(true);
        }

        coinText.text = "" + coins; 

        Spinner spinner = ship.GetComponent<Spinner>();
        if (spinner != null)
            Destroy(spinner);

        isStarted = true;


        uiManager.showGameLauncher(false);
        if (isSfx)
        {
            engineAudio.Play();
        }

    }

    public void onLeftPressed()
    {
        turnPower = 0f;
        left = -1;
    }

    public void onRightPressed()
    {
        turnPower = 0f;
        right = 1;
    }

    public void onLeftUp()
    {
        left = 0;
    }

    public void onRightUp()
    {
        right = 0;
    }

   /* public void stopMoving()
    {
        
        if (!left && !right)
        {
            turnDir = 0;
            turnPower = 0f;
        }
        
    }*/

    public void collectCoin(GameObject gameObject)
    {
        gameObject.SetActive(false);
        coins++;
        Instantiate(coinDestructionEffect, gameObject.transform.position, Quaternion.identity);
        if (isSfx)
            coinCollectAudio.Play();

        if (coinDoubleBooster != null)
        {
            coinDoubleText.GetComponent<Animation>().Play();
            coins++;
        }

        coinText.text = "" + coins;
        //this effect will be destroyed automatically after some time, because this prefab already has a script on it
    }

    public void collectBooster(GameObject gameObject)
    {
        switch (gameObject.tag)
        {
            case Const.TAG_BOOSTER_COIN_DOUBLE:
                coinDoubleBooster = new Booster(20000f, slider);
                break;
            case Const.TAG_BOOSTER_INVINCIBLE:
                invincibleBooster = new Booster(15000f, slider);
                setActiveShield(true);
                break;
            case Const.TAG_BOOSTER_SCORE:
                scoreBooster = new Booster(7000f, slider);
                uiManager.boosterTakeShow(true);
                nitroBoostAudio.Play();
                Invoke(nameof(stopShowingBoosterCatch), 2f);
                setThrusters(true);
                tutorialIdx = 3;
                SetTutorialMsg("Your ship's speed and score are kept boosted until the timer ends!", 4f, () => {
                    if (tutorialIdx == 3)  //if ship isn't crashed to the asteroid after taking booster
                    {
                        SetTutorialMsg("Collect other boosters and coins!\n\nYou Rock!", 3f, () =>
                        {
                            SkipTutorial();
                        });
                    }
                    
                });
                break;
        }
        if (isSfx)
            boosterCollectAudio.Play();
        gameObject.SetActive(false); // will be destroyed by its generator script

    }

    private void stopShowingBoosterCatch()
    {
        uiManager.boosterTakeShow(false);
    }

    private void onUserEarnedReward()
    {
        continueText.text = "continue?";
        if (lastCollidedWith != null)
            lastCollidedWith.SetActive(false);
        isFinished = false;
        isStarted = true;
        uiManager.hideResults();
        uiManager.showPauseBtn(true);
        cameraAttached = true;

        GetComponent<ObstacleGenScript>().enabled = true;
        if (isTutorialCompleted)
        {
            invincibleBooster = new Booster(7000f, slider);
            setActiveShield(true);
        }

        //continue the sounds, if is on
        if (isSfx)
            engineWorkingAudio.Play();

        if (isMusic)
            bgMusic.Play();
    }

    private void setThrusters(bool isActive)
    {
        scoreMultiplier = isActive ? 5 : 1;
        forceMagnitude += isActive ?  16500: -16500;
        //rb.velocity *= 1000;
        Debug.Log("Force Mag: " + forceMagnitude);
        Transform thrusters = ship.transform.Find(Const.OBJECT_TRHUSTERS);
        if (thrusters != null)
        {
            thrusters.gameObject.SetActive(isActive);
        }
        else
        {
            Debug.Log(Const.OBJECT_TRHUSTERS+" not found");
        }
    }

    private void setActiveShield(bool isActive)
    {
        Transform transform = ship.transform.Find(Const.OBJECT_SHIELD);
        if (transform != null)
            transform.gameObject.SetActive(isActive);
    }

    public void collided(GameObject gameObject)
    {
        if (!isStarted)
            return;
        // if invincilbe booster active, then destroy the collided object, not the ship
        if (invincibleBooster != null)
        {
            ship.SetActive(true);
            Instantiate(obstacleDestructionEffect, ship.transform.position, Quaternion.identity);
            //Destroy(gameObject); //don't destroy because it is already being destroyed by obstacle generation script
            //instead set it invisible
            gameObject.SetActive(false);
        }
        else
        {
            lastCollidedWith = gameObject;
            msgText.text = "Game Over";
            uiManager.showPauseBtn(false);
            engineWorkingAudio.Stop();
            //if no invincile booster active then, destroy the ship
            isStarted = false;
            //cam.transform.parent = null;

            float x = UnityEngine.Random.Range(0f, 1f);
            float y = UnityEngine.Random.Range(0f, 1f);
            GetComponent<ObstacleGenScript>().enabled = false;
            PrefManager.AddPlayerCoins(coins);
            ship.GetComponent<Rigidbody>().velocity = Vector3.zero;
            if (!isFinished)
            {
                if (isSfx)
                    crashAudio.Play();
                Instantiate(shipDestructionEffect, ship.transform.position, Quaternion.identity);
            }
            Vector3 pos = cam.transform.position;
            isFinished = true;
            if (isTutorialCompleted)
            {
                uiManager.showResults((int)score, 1.5f);
            }
            else
            {
                SetTutorialMsg("oops your ship crashed\n Let's try again", 2f, () =>
                {
                    tutorialObstacles.SetActive(false);
                    onUserEarnedReward();
                    Invoke(nameof(showTutorialObstacles), 2f);
                });
            }
                
        }

    }

    private void showTutorialBooster()
    {
        showTutorialObstacles();
        tutorialBooster.SetActive(true);
    }

    private void showTutorialObstacles()
    {
        int childCount = tutorialObstacles.transform.childCount;
        for (int i = 0; i < childCount-1; i++)
        {
            tutorialObstacles.transform.GetChild(i).gameObject.SetActive(true);
        }
        tutorialObstacles.SetActive(true);
        tutorialObstacles.transform.position = new Vector3(0, 0, ship.transform.position.z + 100);
    }

    public void checkpoint()
    {
        if (tutorialIdx == 1)
            SetTutorialMsg("Good Job!", 2f, () => {
                scoreArrow.SetActive(true);
                SetTutorialMsg("Your score increased!\nThe longer you survive the more you score", 5f, ()=>{ 
                    tutorialMsgText.text = "Let's give your ship a boost\nCollect the comming Booster";
                    scoreArrow.SetActive(false);
                    showTutorialBooster();
                    tutorialIdx = 2;
                }
                );
            });
        else if (tutorialIdx == 2)
        {
            SetTutorialMsg("Oops! you missed the booster\nLet's try again!", 2f, () => {
                tutorialMsgText.text = "Let's give your ship a boost\nCollect the comming Booster";
                scoreArrow.SetActive(false);
                showTutorialObstacles();
                tutorialBooster.SetActive(true);
                tutorialIdx = 2;
            });
        }
    }

    public void SkipTutorial()
    {
        isTutorialCompleted = true;
        tutorial.SetActive(false);
        PrefManager.SetTutorialCompleted();
        startPlay();
    }

    public void onUserPressedWatchAd()
    {
        continueText.text = "Loading Ad...";
        //load the ad for the game
        AdManager adManager = GetComponent<AdManager>();
        adManager.LoadRewardedAd((isSuccess) => {
            if (isSuccess){
                adManager.ShowRewardedAd(() =>
                {
                    onUserEarnedReward();
                });
            }else{
                continueText.text = "Failed to load Ad! Try later";
            }
            
        });
        
        bgMusic.Pause();
    }

    public void PauseGame()
    {
        Time.timeScale = 0;

        bgMusic.Pause();

        engineWorkingAudio.Pause();

        uiManager.showPauseMenu(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        uiManager.showPauseMenu(false);

        engineWorkingAudio.Play();
        bgMusic.Play();
    }


}
