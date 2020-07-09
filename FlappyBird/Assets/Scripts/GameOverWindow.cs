using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverWindow : MonoBehaviour{

    private Text scoreText;
    private Text highscoreText;

    private void Awake(){
        scoreText = transform.Find("ScoreText").GetComponent<Text>();
        highscoreText = transform.Find("HighscoreText").GetComponent<Text>();
    }

    private void Start(){
        Bird.GetInstance().OnDied += Bird_OnDied;
        Hide();
    }

    private void Bird_OnDied(object sender, System.EventArgs e){
        scoreText.text = Level.GetInstance().GetPipesPassedCount().ToString();

        if(Level.GetInstance().GetPipesPassedCount() >= Score.GetHighscore()){
            //Novo recorde
            highscoreText.text = "Novo Recorde!!";
        }else{
            highscoreText.text = "Recorde: " + Score.GetHighscore().ToString();
        }

        Show();
    }
    public void Retry(){
        SceneManager.LoadScene("GameScene");
    }

    public void LoadMainMenu(){
        SceneManager.LoadScene("MainMenu");
    }

    private void Hide(){
        gameObject.SetActive(false);
    }

    private void Show(){
        gameObject.SetActive(true);
    }
}
