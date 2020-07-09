using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour{

    private const float CAMERA_ORTHO_SIZE = 128f;
    //Posicao que deve ser atingida pelo cano para retirar o mesmo da cena
    private const float PIPE_DESTROY_X_POSITION = -200f;
    private const float PIPE_SPAWN_X_POSITION = +200f;
    private const float PIPE_WIDTH = 26f;
    private const float PIPE_HEAD_HEIGHT = 12f;

    private const float BIRD_X_POSITION = 0f;

    private const float PIPE_MOVE_SPEED = 40f;

    private float pipeSpawnTimer;
    private float pipeSpawnTimerMax;
    private float gapSize;
    private int pipesSpawned;
    private int pipesPassedCount;
    private List<Pipe> pipeList;

    private static Level instance;
    public static Level GetInstance(){
        return instance;
    }

    private State state;

    public enum Difficulty{
        Easy,
        Medium,
        Hard,
        Brutal,
    }

    private enum State{
        WaitingToStart,
        Playing,
        BirdDead,
    }

    private void Awake(){
        instance = this;
        pipeList = new List<Pipe>();
        pipeSpawnTimerMax = 2f;
        SetDifficulty(Difficulty.Easy);
        state = State.WaitingToStart;
    }

    private void Start(){
        Bird.GetInstance().OnDied += Bird_OnDied;
        Bird.GetInstance().OnStartedPlaying += Bird_OnStartedPlaying;
    }

    private void Bird_OnStartedPlaying(object sender, System.EventArgs e){
        state = State.Playing;
    }

    private void Bird_OnDied(object sender, System.EventArgs e){
        state = State.BirdDead;
        //Invoke("ReloadScene", 1f);
    }
/*
    private void ReloadScene(){
        SceneManager.LoadScene("GameScene");
    }
*/
    private void Update(){
        if(state == State.Playing){
            HandlePipeMovement();
            HandlePipeSpawning();
        }
    }

    //Spawna os canos na cena
    private void HandlePipeSpawning(){
        pipeSpawnTimer -= Time.deltaTime;
        if(pipeSpawnTimer < 0){
            //Momento de spawnar outro cano
            pipeSpawnTimer = pipeSpawnTimerMax;

            float heightEdgeLimit = 13f;
            float minHeight = gapSize * 0.5f + heightEdgeLimit;
            float totalHeight = CAMERA_ORTHO_SIZE * 2f;
            float maxHeight = totalHeight - gapSize * 0.5f - heightEdgeLimit;

            float height = Random.Range(minHeight, maxHeight);
            
            CreateGapPipes(height, gapSize, PIPE_SPAWN_X_POSITION);
        }
    }

    //Move os canos pelo nivel da direita para a esquerda
    private void HandlePipeMovement(){

        for(int i = 0; i < pipeList.Count; i++){
            Pipe pipe = pipeList[i];
            bool isToTheRight = pipe.GetXPosition() > BIRD_X_POSITION;
            pipe.Move();
            if(isToTheRight && (pipe.GetXPosition() <= BIRD_X_POSITION) && pipe.IsAtBottom()){
                //O cano passou pelo birb
                pipesPassedCount++;
                SoundManager.PlaySound(SoundManager.Sound.Score);
            }
            //Para remover o cano da cena
            if(pipe.GetXPosition() < PIPE_DESTROY_X_POSITION){
                //Remover o cano da cena e da lista respectivamente
                pipe.DestroySelf();
                pipeList.Remove(pipe);
                i--;
            }
        }
    }

    private void SetDifficulty(Difficulty difficulty){
        switch(difficulty){
            case Difficulty.Easy:
            gapSize = 80f;
            break;
            case Difficulty.Medium:
            gapSize = 64f;
            break;
            case Difficulty.Hard:
            gapSize = 56f;
            break;
            case Difficulty.Brutal:
            gapSize = 48f;
            break;
        }
    }
    private Difficulty GetDifficulty(){
        if(pipesSpawned >= 30) return Difficulty.Brutal;
        if(pipesSpawned >= 20) return Difficulty.Hard;
        if(pipesSpawned >= 10) return Difficulty.Medium;
        return Difficulty.Easy;

    }

    //Cria pares de canos com um espaço entre eles
    //gapY é a posição desse espaço no eixo y
    //gapSize é o tamanho desse espaço
    //xPosition é a posição no eixo X desse par de canos
    private void CreateGapPipes(float gapY, float gapSize, float xPosition){

        CreatePipe(gapY - (gapSize * 0.5f), xPosition, true);
        CreatePipe((CAMERA_ORTHO_SIZE * 2f) - gapY - (gapSize * 0.5f), xPosition, false);
        
        pipesSpawned++;
        SetDifficulty(GetDifficulty());
    }

    //Funcao para criar canos
    private void CreatePipe(float height, float xPosition, bool createBottom){
        //Posiciona o pipeHead
        Transform pipeHead = Instantiate(GameAssets.GetInstance().pfPipeHead);
        //Posiciona corretamente o pipeHead mesmo que seja um cano na parte superior da tela ou inferior
        float pipeHeadYPosition;
        if(createBottom){
            pipeHeadYPosition = -CAMERA_ORTHO_SIZE + height - (PIPE_HEAD_HEIGHT * 0.5f);
        }else{
            pipeHeadYPosition = +CAMERA_ORTHO_SIZE - height + (PIPE_HEAD_HEIGHT * 0.5f);
        }
        pipeHead.position = new Vector3(xPosition, pipeHeadYPosition);
        
        //Posiciona o pipeBody
        Transform pipeBody = Instantiate(GameAssets.GetInstance().pfPipeBody);
        //Posiciona corretamente o pipeHeadBody mesmo que seja um cano na parte superior da tela ou inferior
        float pipeBodyYPosition;
        if(createBottom){
            pipeBodyYPosition = -CAMERA_ORTHO_SIZE;
        }else{
            pipeBodyYPosition = +CAMERA_ORTHO_SIZE;
            pipeBody.localScale = new Vector3(1, -1, 0);
        }
        pipeBody.position = new Vector3(xPosition,  pipeBodyYPosition);

        //Ajusta a altura do pipeBody
        SpriteRenderer pipeBodySpriteRenderer = pipeBody.GetComponent<SpriteRenderer>();
        pipeBodySpriteRenderer.size = new Vector2(PIPE_WIDTH, height);
        //Ajusta a altura do collider do pipeBody
        BoxCollider2D pipeBodyBoxCollider = pipeBody.GetComponent<BoxCollider2D>();
        pipeBodyBoxCollider.size = new Vector2(PIPE_WIDTH, height);
        pipeBodyBoxCollider.offset = new Vector2(0f, height * 0.5f);

        Pipe pipe = new Pipe(pipeHead, pipeBody, createBottom);
        pipeList.Add(pipe);
    }

    public int GetPipesSpawned(){
        return pipesSpawned;
    }

    public int GetPipesPassedCount(){
        return pipesPassedCount;
    }

    //Representa um cano
    private class Pipe{

        private bool createBottom;
        private Transform pipeHeadTransform;
        private Transform pipeBodyTransform;

        public Pipe(Transform pipeHeadTransform, Transform pipeBodyTransform, bool createBottom){
            this.pipeHeadTransform = pipeHeadTransform;
            this.pipeBodyTransform = pipeBodyTransform;
            this.createBottom = createBottom;
        }

        //Move os canos pelo nivel da direita para a esquerda
        public void Move(){
            pipeHeadTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
            pipeBodyTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;

        }

        //Funcao para repassar a posicao do cano no eixo x
        public float GetXPosition(){
            return pipeHeadTransform.position.x;
        }
       
         public bool IsAtBottom(){
            return createBottom;
        }

        //Remove o cano da cena
        public void DestroySelf(){
            Destroy(pipeHeadTransform.gameObject);
            Destroy(pipeBodyTransform.gameObject);
        }
    }
}
