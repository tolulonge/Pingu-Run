using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Facebook.Unity;

public class TutorialManager : MonoBehaviour
{
    public GameObject[] popUps;
    private int popUpIndex;
    float waitTime = 1f;
    public Animator tutorialAnim, playerAnim;
    private PlayerMotor playerMotor;
    public Text mainText, subText;
    private void Start()
    {
        playerMotor = FindObjectOfType<PlayerMotor>();
    }
    private void Awake()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(() =>
            {
                if (FB.IsInitialized)
                    FB.ActivateApp();
                else
                    Debug.LogError("Couldn't Initialize");
            },
            isGameShown =>
            {
                if (!isGameShown)
                    Time.timeScale = 0;
                else
                    Time.timeScale = 1;
            }
            );
        }
        else
            FB.ActivateApp();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.isGameStarted && MobileInput.Instance.Tap)
        {
            GameManager.Instance.shouldStart = true;

        }

        if (waitTime <= 0)
        {
            if(popUpIndex == 0)
            {
                tutorialAnim.SetTrigger("Enter");
                FindObjectOfType<GlacierSpawner>().IsScrolling = false;
                FindObjectOfType<PlayerMotor>().isRunning = false;
                playerAnim.SetTrigger("TutorialPause");
                popUpIndex++;
            }
  

            waitTime = 0;

        }
        else 
        {
            if(GameManager.Instance.isGameStarted)
            waitTime -= Time.deltaTime;
        }
   

        if(popUpIndex == 1)
        {
            if(MobileInput.Instance.SwipeLeft)
            {
                tutorialAnim.SetTrigger("Exit");
                playerMotor.isRunning = true;
                playerMotor.desiredLane = 0;
                playerAnim.SetTrigger("StartRunning");
                FindObjectOfType<GlacierSpawner>().IsScrolling = true;
                popUpIndex++;
                StartCoroutine(Countdown());

            }
           
        }

        if(popUpIndex == 3)
        {
            if (MobileInput.Instance.SwipeRight)
            {
                tutorialAnim.SetTrigger("Exit");
                playerMotor.isRunning = true;
                playerMotor.desiredLane = 1;
                playerAnim.SetTrigger("StartRunning");
                waitTime = 2;
                FindObjectOfType<GlacierSpawner>().IsScrolling = true;
                popUpIndex++;
               // StartCoroutine(NextCountdown());
            }
        }

        if (popUpIndex == 4 && waitTime <= 0)
        {
            mainText.text = "Swipe down to avoid obstacle";
            tutorialAnim.SetTrigger("Enter");
            FindObjectOfType<GlacierSpawner>().IsScrolling = false;
            FindObjectOfType<PlayerMotor>().isRunning = false;
            playerAnim.SetTrigger("TutorialPause");
            popUpIndex++;
            StartCoroutine(Countdown());
        }

        if (popUpIndex == 5)
        {
            if (MobileInput.Instance.SwipeDown)
            {
                playerMotor.handleTutorialSlide();
                tutorialAnim.SetTrigger("Exit");
                playerMotor.isRunning = true;
                playerMotor.desiredLane = 1;
                playerAnim.SetTrigger("StartRunning");
                waitTime = 2;
                FindObjectOfType<GlacierSpawner>().IsScrolling = true;
                popUpIndex++;
            }
        }


        if (popUpIndex == 6 && waitTime <= 0)
        {
            mainText.text = "Swipe up to jump up";
            tutorialAnim.SetTrigger("Enter");
            FindObjectOfType<GlacierSpawner>().IsScrolling = false;
            FindObjectOfType<PlayerMotor>().isRunning = false;
            playerAnim.SetTrigger("TutorialPause");
            popUpIndex++;
            StartCoroutine(Countdown());
        }

        if (popUpIndex == 7)
        {
            if (MobileInput.Instance.SwipeUp)
            {
                playerMotor.handleTutorialJump();
                tutorialAnim.SetTrigger("Exit");
                playerMotor.isRunning = true;
                playerMotor.desiredLane = 1;
                waitTime = 2;
                FindObjectOfType<GlacierSpawner>().IsScrolling = true;
                popUpIndex++;
            }
        }

        if (popUpIndex == 8 && waitTime <= 0)
        {
            mainText.text = "Pickup shields and score boosters in form of jetpack along the way";
            subText.text = "Swipe anywhere to continue";
            tutorialAnim.SetTrigger("Enter");
            FindObjectOfType<GlacierSpawner>().IsScrolling = false;
            FindObjectOfType<PlayerMotor>().isRunning = false;
            playerAnim.SetTrigger("TutorialPause");
            popUpIndex++;
            StartCoroutine(Countdown());
        }

        if (popUpIndex == 9)
        {
            if (MobileInput.Instance.SwipeUp || MobileInput.Instance.SwipeDown || MobileInput.Instance.SwipeLeft || MobileInput.Instance.SwipeRight)
            {
                tutorialAnim.SetTrigger("Exit");
                playerMotor.isRunning = true;
                playerMotor.desiredLane = 1;
                playerAnim.SetTrigger("StartRunning");
                waitTime = 2;
                FindObjectOfType<GlacierSpawner>().IsScrolling = true;
                popUpIndex++;
            }
        }

        if(popUpIndex == 10 && waitTime <= 0)
        {
            playerMotor.desiredLane = 1;
            mainText.text = "Excellent, You have completed the tutorial";
            subText.text = "Loading...";
            tutorialAnim.SetTrigger("Enter");
            waitTime = 5;
            popUpIndex++;
        }

        if (popUpIndex == 11 && waitTime <= 0)
        {
            tutorialAnim.SetTrigger("Exit");
            SceneManager.LoadScene("Game");
        }
           
            

        if(popUpIndex > 3)
        {
            playerMotor.desiredLane = 1;
        }




    }

    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(2);
        CallNextTutorial();
    }

    IEnumerator NextCountdown()
    {
        yield return new WaitForSeconds(2);
        CallNextTutorial();
    }

    private void CallNextTutorial()
    {

        if(popUpIndex == 2)
        {
            mainText.text = "Swipe right to move to the right";
            tutorialAnim.SetTrigger("Enter");
            FindObjectOfType<GlacierSpawner>().IsScrolling = false;
            FindObjectOfType<PlayerMotor>().isRunning = false;
            playerAnim.SetTrigger("TutorialPause");
            popUpIndex++;
            StartCoroutine(Countdown());
        }


        
    }
}
