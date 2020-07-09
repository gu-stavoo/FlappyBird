using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour{

    private void Start(){
        Debug.Log("GameHandler.Start");//Verifica se inicia corretamente
        Score.Start();
    }
}
