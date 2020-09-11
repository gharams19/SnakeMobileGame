using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class Snake : MonoBehaviour {

    private enum Direction {
        Left,
        Right,
        Up,
        Down
    }
    private enum State {
        Alive,
        Dead
    }
    private State state;
    private Direction gridMoveDirection;
    private Vector3 gridMoveDirectionVector;
    private Vector3 gridPosition;
    private float gridMoveTimer;
    private float gridMoveTimerMax;
    private LevelGrid levelGrid;
    private int snakeBodySize;
    private List<SnakeMovePosition> snakeMovePositionList;
        private List<SnakeBodyPart> snakeBodyPartList;

    private Touch touch;
    private float speedModifier;
    bool snakeAteFood = false;

    private void Start() {
        speedModifier = 0.01f;
        
    }

    public void Setup(LevelGrid levelGrid) {
        this.levelGrid = levelGrid;
        InvokeRepeating("spawnFood", 2f, 2f);
        
    }

    private void Awake() {
        gridPosition = new Vector3(8,10,1);
        gridMoveTimerMax = 0.3f;
        gridMoveTimer = gridMoveTimerMax;
        gridMoveDirection = Direction.Right;
        snakeMovePositionList = new List<SnakeMovePosition>();
        snakeBodySize = 0;
        snakeBodyPartList = new List<SnakeBodyPart>();
        state = State.Alive;
    }
    private void Update() {
        switch(state) {
            case State.Alive:
                HandleInput();
                HandleGridMovement();
                break;
            case State.Dead:
                break;
        }
        UpdateSnakeBodyParts();

      
       
       
        
    }
     void spawnFood()
    {

        levelGrid.SpawnFood();
    }




    private void HandleInput() {
        //Desktop
         if(Input.GetKeyDown(KeyCode.UpArrow)) {
            if(gridMoveDirection != Direction.Down) {
                gridMoveDirection = Direction.Up;
            }
        }
        if(Input.GetKeyDown(KeyCode.DownArrow)) {
            if(gridMoveDirection != Direction.Up) {
                gridMoveDirection = Direction.Down;
            }
        }
        if(Input.GetKeyDown(KeyCode.RightArrow)) {
            if(gridMoveDirection != Direction.Left) {
                gridMoveDirection = Direction.Right;
            }
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow)) {
            if(gridMoveDirection != Direction.Right){
                gridMoveDirection = Direction.Left;
            }
        }
        //Mobile

        Vector3 prevPos = transform.position;
          if(Input.touchCount > 0) {
            touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Moved) {
               gridPosition = new Vector3 (
                    transform.position.x + touch.deltaPosition.x * speedModifier,
                    transform.position.y,
                    transform.position.z + touch.deltaPosition.y * speedModifier
                );
                gridMoveDirectionVector = new Vector3(touch.deltaPosition.x, 0, touch.deltaPosition.y);
                // transform.eulerAngles = new Vector3(90,0,GetAngleFromVector(gridMoveDirectionVector) - 90);
               

            }
            if(prevPos.x < gridPosition.x) {
                gridMoveDirection = Direction.Right;
            }
            else if(prevPos.x > gridPosition.x) {
                gridMoveDirection = Direction.Left;
            }
            else if(prevPos.z < gridPosition.z) {
                gridMoveDirection = Direction.Up;
            }
            else if(prevPos.z > gridPosition.z) {
                gridMoveDirection = Direction.Down;
            }
           
        }
            
            
            

      
        // HandleGridMovement();

        
        
        

       
    }
    private void HandleGridMovement() {
    
    

            SnakeMovePosition previousSnakeMovePosition = null;
            if(snakeMovePositionList.Count > 0) 
            {
                previousSnakeMovePosition = snakeMovePositionList[0];
            }
            SnakeMovePosition snakeMovePosition = new SnakeMovePosition(previousSnakeMovePosition, gridPosition,gridMoveDirection);
            snakeMovePositionList.Insert(0,snakeMovePosition);

           
            gridPosition = levelGrid.ValidateGridPosition(gridPosition);
            

            if(snakeAteFood) {
                snakeBodySize++;
                CreateSnakeBodyPart();
            }
            snakeAteFood = false;
            if(snakeMovePositionList.Count >= snakeBodySize + 1) {
                snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);
            }

            

            transform.position = new Vector3(gridPosition.x, gridPosition.y, gridPosition.z);
            transform.eulerAngles = new Vector3(90,0,GetAngleFromVector(gridMoveDirectionVector) - 90);

    
            



    }
    private void HandleSnakeBody() {
        snakeBodySize++;
        CreateSnakeBodyPart();
        if(snakeMovePositionList.Count >= snakeBodySize + 1) {
                snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);
            }
            // UpdateSnakeBodyParts();
    }
    private void CreateSnakeBodyPart() {
        GameObject prevBodyPart;
        if(snakeBodyPartList.Count == 0) {
            prevBodyPart = gameObject;
        }
        else {
            prevBodyPart = snakeBodyPartList[snakeBodyPartList.Count -1].GetCurrentBodyPart();
        }
        snakeBodyPartList.Add(new SnakeBodyPart(snakeBodyPartList.Count, prevBodyPart));
         UpdateSnakeBodyParts();
    }
   

    private void UpdateSnakeBodyParts() {
         for(int i = 0; i < snakeBodyPartList.Count; i++) {
                snakeBodyPartList[i].SetSnakeMovePosition(snakeMovePositionList[i]);
            }
    }


    private float GetAngleFromVector(Vector3 dir) {
        float n = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        if(n < 0) n += 360;
        return n;
    }

    public Vector3 GetGridPosition() {
        return gridPosition;
    }

    //Return full list of positions occupied by snake: Head + body
    public List<Vector3> GetFullSnakeGridPositionList() {
        List<Vector3> gridPositionList = new List<Vector3>() {gridPosition};
        foreach(SnakeMovePosition snakeMovePosition in snakeMovePositionList) {
            gridPositionList.Add(snakeMovePosition.GetGridPosition());
        }
        return gridPositionList;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.name == "Food") {
            levelGrid.snakeAteFood();
            snakeAteFood = true;
           
        }
    }
    private class SnakeBodyPart {
        
        private SnakeMovePosition snakeMovePosition;
        private Transform transform;

        private GameObject previousBodyPart;
        private GameObject currentBodyPart;
        public SnakeBodyPart(int bodyIndex, GameObject previousBodyPart) {
            GameObject snakeBodyGameObject = new GameObject("SnakeBody", typeof(SpriteRenderer));
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.snakeBodySprite;
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sortingOrder = -1 - bodyIndex;
            transform = snakeBodyGameObject.transform;
            transform.eulerAngles = new Vector3(90, transform.eulerAngles.y, transform.eulerAngles.z);
            this.previousBodyPart = previousBodyPart;
            this.currentBodyPart = snakeBodyGameObject;
            
         
            
        }
        public void SetSnakeMovePosition(SnakeMovePosition snakeMovePosition) {
            this.snakeMovePosition = snakeMovePosition;
            

            float angle;
            float offsetX, offsetZ; 
            switch(snakeMovePosition.GetDirection()) {
                default:
                case Direction.Up: //currently going up
                switch(snakeMovePosition.GetPreviousDirection()) {
                    default: angle  = 0; break;
                    case Direction.Left: angle = 0 + 45; break; //Prev was going left
                    case Direction.Right: angle = 0 - 45; break; //prev was going right

                }
                offsetZ = -1;
                offsetX = 0;
                break;
                case Direction.Down: //currently going down
                switch(snakeMovePosition.GetPreviousDirection()) {
                    default: angle  = 180; break;
                    case Direction.Left: angle = 180 + 45; break; //Prev was going left
                    case Direction.Right: angle = 180 - 45; break; //prev was going right

                } 
                offsetZ = 1;
                offsetX = 0;
                break;
                case Direction.Left: //currently going left
                switch(snakeMovePosition.GetPreviousDirection()) {
                    default: angle  = -90; break;
                    case Direction.Down: angle = -45; break; //Prev was going down
                    case Direction.Up: angle = 45; break; //prev was going up

                }
                offsetZ = 0;
                offsetX = 1;
                break;
                case Direction.Right: //Currently going right
                switch(snakeMovePosition.GetPreviousDirection()) {
                    default: angle  = 90; break;
                    case Direction.Down: angle = 45; break; //Prev was going down
                    case Direction.Up: angle = -45; break; //prev was going up

                }
                offsetZ = 0;
                offsetX = -1;
                break;
            }
            transform.position = new Vector3(previousBodyPart.transform.position.x  + offsetX, snakeMovePosition.GetGridPosition().y, previousBodyPart.transform.position.z + offsetZ);
            transform.eulerAngles = new Vector3(90,0,angle);

        }
        public Vector3 GetGridPosition() {
            return snakeMovePosition.GetGridPosition();
        }
        public GameObject GetPreviousBodyPart() {
            return previousBodyPart;
        }
        public GameObject GetCurrentBodyPart() {
            return currentBodyPart;
        }
    }
    //Handles one move position from the snake
    private class SnakeMovePosition {
        private SnakeMovePosition previousSnakeMovePosition;
        private Vector3 gridPosition;
        private Direction direction;
        public SnakeMovePosition(SnakeMovePosition previousSnakeMovePosition, Vector3 gridPosition, Direction direction) {
            this.previousSnakeMovePosition = previousSnakeMovePosition;
            this.gridPosition = gridPosition;
            this.direction = direction;

        }
        public Vector3 GetGridPosition() {
            return gridPosition;
        }
        public Direction GetDirection() {
            return direction;
        }

        public Direction GetPreviousDirection() {
            if(previousSnakeMovePosition == null) {
                return Direction.Right;
            } else {
                return previousSnakeMovePosition.direction;
            }

        }

    }
}