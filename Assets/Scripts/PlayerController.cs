using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed = 1f;
    [HideInInspector]
    public float baseSpeed;
    private Rigidbody rb;
    private int pickUpCount;
    GameObject resetPoint;
    bool resetting = false;
    bool grounded = true;
    Color originalColour;
    private bool gameOver = false;

    //Controllers
    CameraController cameraController;
    GameController gameController;
    SoundController soundController;
    Timer timer;

    [Header("UI")]
    public GameObject gameOverScreen;
    public TMP_Text pickUpText;
    public TMP_Text timerText;
    public TMP_Text winTimeText;
    public GameObject inGamePanel;
    public Image pickUpSlider;

    void Start()
    {
        baseSpeed = speed;
        Time.timeScale = 1;
        //Turn on our in game panel
        inGamePanel.SetActive(true);
        rb = GetComponent<Rigidbody>();
        //Get the number of pick ups in our scene
        pickUpCount = GameObject.FindGameObjectsWithTag("Pick Up").Length;
        //Run the Check Pickups Function
        CheckPickUps();
        //Game Over related
        gameOverScreen.SetActive(false);
        //Get the timer object and start the timer
        timer = FindObjectOfType<Timer>();
        //Reset Point stuff
        resetPoint = GameObject.Find("Reset Point");
        originalColour = GetComponent<Renderer>().material.color;
        
        gameController = FindObjectOfType<GameController>();
        cameraController = FindObjectOfType<CameraController>();
        soundController = FindObjectOfType<SoundController>();


        timer = FindObjectOfType<Timer>();
        if (gameController.gameType == GameType.SpeedRun)
            StartCoroutine(timer.StartCountdown());
    }
    
    void FixedUpdate()
    {
        if (gameController.gameType == GameType.SpeedRun && !timer.IsTiming())
            return;

        if (gameController.controlType == ControlType.WorldTilt)
            return;

        if (resetting)
            return;

        if (grounded)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
            
            if(cameraController.cameraStyle == CameraStyle.Free)
            {
                //rotates the player to the direction of the camera
                transform.eulerAngles = Camera.main.transform.eulerAngles;
                //Translates the input vectors into coordinates
                movement = transform.TransformDirection(movement);
            }

            rb.AddForce(movement * speed);
        }

        if (gameOver == true)
            return;



    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Pick Up")
        {
            //Particle related
            other.GetComponent<Particles>().CreateParticles();
            //Destroy the collided object
            Destroy(other.gameObject);
            //Decrement the pick up count
            pickUpCount--;
            //Run the Check Pick Ups function
            CheckPickUps();
            //Sound related
            soundController.PlayPickupSound();
        }

        if(other.gameObject.CompareTag("Powerup"))
        {
            other.GetComponent<Powerup>().UsePowerup();
            other.gameObject.transform.position = Vector3.down * 1000;
            //apparently this is wwhere you should add sound script
        }
    }

    void CheckPickUps()
    {
        pickUpText.text = "Pick Ups Left: " + pickUpCount;
        float inv = Mathf.InverseLerp(0f, 3f, pickUpCount);
        pickUpSlider.fillAmount = inv;
        if (pickUpCount == 0)
        {
            WinGame();
        }
    }

    void WinGame()
    {
        //Set our game over to true
        gameOverScreen.SetActive(true);
        //Turn off our in game panel
        inGamePanel.SetActive(false);
        //Stop the timer
        timer.StopTimer();
        soundController.PlayWinSound();

        //Stop the ball from moving
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Respawn"))
        {
            StartCoroutine(ResetPlayer());
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            soundController.PlayCollisionSound(collision.gameObject);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.collider.CompareTag("Ground"))
            grounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
            grounded = false;
    }

    public IEnumerator ResetPlayer()
    {
        resetting = true;
        GetComponent<Renderer>().material.color = Color.black;
        rb.velocity = Vector3.zero;
        Vector3 startPos = transform.position;
        float resetSpeed = 2f;
        var i = 0.0f;
        var rate = 1.0f / resetSpeed;
        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;
            transform.position = Vector3.Lerp(startPos, resetPoint.transform.position, i);
            yield return null;
        }
        GetComponent<Renderer>().material.color = originalColour;
        resetting = false;
    }


}