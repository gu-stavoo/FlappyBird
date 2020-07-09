﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuWindow : MonoBehaviour{

    public void PlayGame(){
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame(){
        Debug.Log("Quit game.");
        Application.Quit();
    }

}
