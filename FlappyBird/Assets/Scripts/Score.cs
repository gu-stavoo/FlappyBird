using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Score{

    public static void Start(){
        Bird.GetInstance().OnDied += Bird_OnDied;
        //ResetHighscore();
    }

    public static void Bird_OnDied(object sender, System.EventArgs e){
        TrySetNewHighscore(Level.GetInstance().GetPipesPassedCount());
    }
    public static int GetHighscore(){
        return PlayerPrefs.GetInt("highscore");
    }

    //Para settar um novo recorde
    public static bool TrySetNewHighscore(int score){
        int currentHighscore = GetHighscore();
        if(score > currentHighscore){
            //Setta novo recorde
            PlayerPrefs.SetInt("highscore", score);
            PlayerPrefs.Save();

            //Debug.Log(PlayerPrefs.GetInt("highscore"));
            return true;
        }else{
            return false;
        }
    }
    public static void ResetHighscore(){
            //Resetta a pontuação recorde
            PlayerPrefs.SetInt("highscore", 0);
            PlayerPrefs.Save();
    }
}
