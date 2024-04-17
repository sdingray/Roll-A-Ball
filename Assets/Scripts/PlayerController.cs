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
    private Timer timer;
    private bool gameOver = false;

    [Header("UI")]
    public GameObject gameOverScreen;
    public TMP_Text pickUpText;
    public TMP_Text timerText;
    public TMP_Text winTimeText;
    public GameObject winPanel;
    public GameObject inGamePanel;
    public Image pickUpSlider;

    void Start()
    {
        //Turn on our in game panel
        inGamePanel.SetActive(true);
        //Turn off our win panel
        winPanel.SetActive(false);
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
    }

    private void Update()
    {
        timerText.text = "Time: " + timer.GetTime().ToString("F2");
    }

    void FixedUpdate()
    {
        if (resetting)
            return;

        if (gameOver == true)
            return;


        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
         
        Vector3 movement = new Vector3 (moveHorizontal, 0, moveVertical);
        rb.AddForce(movement * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Pick Up")
        {
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
        //Turn on our win panel
        winPanel.SetActive(true);
        //Stop the timer
        timer.StopTimer();
        //Display our time to the win time text
        winTimeText.text = "Your time was: " + timer.GetTime().ToString("F2");

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
        Vector3 startPost = transform.position;
        float resetSpeed = 2f;
        var 1 = 0.0f;
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