using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour{

    public event EventHandler OnDied;
    public event EventHandler OnStartedPlaying;
    private static Bird instance;

    public static Bird GetInstance(){
        return instance;
    }

    private const float JUMP_AMOUNT = 120f;
    private Rigidbody2D birdRigidbody2D;

    private State state;
    private enum State{
        WaitingToStart,
        Playing,
        Dead,
    }

    private void Awake(){
        instance = this;
        birdRigidbody2D = GetComponent<Rigidbody2D>();
        birdRigidbody2D.bodyType = RigidbodyType2D.Static;
        state = State.WaitingToStart;
    }
    private void Update(){

        switch(state){
            default:
            case State.WaitingToStart:
                //Aguardando o pulo inicial para começar o jogo
                if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || (Input.touchCount > 0)){
                    state = State.Playing;
                    birdRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
                    Jump();
                    if(OnStartedPlaying != null) OnStartedPlaying(this, EventArgs.Empty);
                }
                break;
            case State.Playing:
                //Reconhece o input referente ao pulo
                //ATENCAO: verificar se o touch funciona corretamente
                if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || (Input.touchCount > 0)){
                    Jump();
                }
                //Muda o angulo do sprite
                transform.eulerAngles = new Vector3(0, 0, birdRigidbody2D.velocity.y * 0.2f);

                break;
            case State.Dead:
                break; 
        }
    }

    //Funcao de pulo
    private void Jump(){
        birdRigidbody2D.velocity = Vector2.up * JUMP_AMOUNT;
        //SoundManager.PlaySound(SoundManager.Sound.BirdJump);
    }

    //Detectar colisoes
    private void OnTriggerEnter2D(Collider2D collider){

        birdRigidbody2D.bodyType = RigidbodyType2D.Static;
        SoundManager.PlaySound(SoundManager.Sound.Lose);
        if(OnDied != null) OnDied(this, EventArgs.Empty);
    }
}
