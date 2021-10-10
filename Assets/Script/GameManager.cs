using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
   public static GameManager Instance { set; get; }

    public bool isGameStarted = false;
    public bool IsDead { set; get; }
    private PlayerMotor motor;

    private PlayGames playGames;
    public AudioSource mainAudio;
    public AudioClip mainGameAudio;

    public bool shouldStart;
    public GameObject fb;


    // Death Menu
    public Animator deathMenuAnim;
    public Text deadScoreText, deadCoinText;

    private const int COIN_SCORE_AMOUNT = 5;

    // UI and the UI fields
    private float score, coinScore, modifierScore;
    
    public Text scoreText, coinText, modifierText, hiScoreText, totalCoinAmountText, notEnoughtTxt;
    public Animator gameCanvas, menuAnim, diamondAnim, settingsAnim;

    private int lastScore;

    private void Awake()
    {
        
        int gameScene = PlayerPrefs.GetInt("Tutorial", 0);

        if(gameScene == 0)
        {
            SceneManager.LoadScene("TutorialScene");
            PlayerPrefs.SetInt("Tutorial", 1);
        }
        else
        {
            fb.SetActive(true);
        }


        playGames = FindObjectOfType<PlayGames>();
        Instance = this;
        modifierScore = 1;
        modifierText.text = "x" + modifierScore.ToString("0.0");
        coinText.text = coinScore.ToString("0");
        scoreText.text = score.ToString("0");
        motor = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>();
        hiScoreText.text = PlayerPrefs.GetFloat("HiScore").ToString("0");
      
    }

    private void Update()
    {
       
        if(!isGameStarted && shouldStart)
        {
            mainAudio.Stop();
            mainAudio.clip = mainGameAudio;
            mainAudio.Play();
            isGameStarted = true;
            motor.StartRunning();
            FindObjectOfType<GlacierSpawner>().IsScrolling = true;
            FindObjectOfType<CameraMotor>().IsMoving = true;
            FindObjectOfType<SnowMotor>().IsMoving = true;
            gameCanvas.SetTrigger("Show");
            menuAnim.SetTrigger("Hide");

        }

        if (isGameStarted && !IsDead)
        {
            // Bump the score up
            lastScore = (int)score;
            score += (Time.deltaTime * modifierScore);
            if(lastScore != (int)score)
            {
                lastScore = (int)score;
                scoreText.text = score.ToString("0");
            }
            
        }
    }

    public void LaunchSettings()
    {
        settingsAnim.SetTrigger("Settings_Enter");
    }

    public void HideSettings()
    {
        settingsAnim.SetTrigger("Settings_Exit");
    }

    public void StartGame()
    {
        shouldStart = true;
    }
  


    public void UpdateModifier(float modifierAmount)
    {
        modifierScore = 1.0f + modifierAmount;
        modifierText.text = "x" + modifierScore.ToString("0.0");
   
    }

    public void GetCoin()
    {
        coinScore++;
        coinText.text = coinScore.ToString("0");
        score += COIN_SCORE_AMOUNT;
        scoreText.text = score.ToString("0");
        diamondAnim.SetTrigger("Collect");

    }

    public void OnPlayButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        playGames.OpenSave(false);

    }

    public void OnDeath()
    {
        IsDead = true;
        FindObjectOfType<GlacierSpawner>().IsScrolling = false;
        deadScoreText.text = score.ToString("0");
        deadCoinText.text = coinScore.ToString("0");
        deathMenuAnim.SetTrigger("Dead");
        gameCanvas.SetTrigger("Hide");
        playGames.totalCoins += (int) coinScore;



        playGames.OpenSave(true);



   



        // Check if this is a highScore
        if(score > PlayerPrefs.GetFloat("HiScore"))
        {
            float s = score;
          
            PlayerPrefs.SetFloat("HiScore", s);
        }
    }

    public void Revive()
    {
     
        if(playGames.totalCoins >= 200)
        {
            motor.ResetPosition();
            IsDead = false;
            isGameStarted = true;
            motor.StartRunning();
            FindObjectOfType<GlacierSpawner>().IsScrolling = true;
            FindObjectOfType<CameraMotor>().IsMoving = true;
            FindObjectOfType<SnowMotor>().IsMoving = true;
            gameCanvas.SetTrigger("Show");
            deathMenuAnim.SetTrigger("Hide");
            gameCanvas.SetTrigger("Show");
            playGames.totalCoins -= 200;
            
        }
        else
        {
            notEnoughtTxt.gameObject.SetActive(true);
            notEnoughtTxt.text = "You do not have enough coins, " + (200 - playGames.totalCoins) + " left";
        }
        
      
        
    }
}
