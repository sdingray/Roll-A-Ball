using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed = 1f;
    private Rigidbody rb;
    private int pickUpCount;
    GameObject resetPoint;
    bool resetting = false;
    Color originalColour;
    private bool gameOver = false;

    //Controllers
    CameraController cameraController;
    GameController gameController;
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
        timer.StartTimer();
        //Reset Point stuff
        resetPoint = GameObject.Find("Reset Point");
        originalColour = GetComponent<Renderer>().material.color;

        cameraController = FindObjectOfType<CameraController>();

        gameController = FindObjectOfType<GameController>();
        timer = FindObjectOfType<Timer>();
        if (gameController.gameType == GameType.SpeedRun)
            StartCoroutine(timer.StartCountdown());
    }
    
    void FixedUpdate()
    {
        if (gameController.gameType == GameType.SpeedRun && !timer.IsTiming())
            return;

        if (resetting)
            return;

        if (gameOver == true)
            return;


        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3 (moveHorizontal, 0, moveVertical);

        if(cameraController.cameraStyle == CameraStyle.Free)
        {
            //rotates the player to the direction of the camera
            transform.eulerAngles = Camera.main.transform.eulerAngles;
            //Translates the input vectors into coordinates
            movement = transform.TransformDirection(movement);
        }

        rb.AddForce(movement * speed);
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