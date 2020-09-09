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
    bool snakeAteFood;

    private void Start() {
        speedModifier = 0.01f;
    }

    public void Setup(LevelGrid levelGrid) {
        this.levelGrid = levelGrid;
        
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
                break;
            case State.Dead:
                break;
        }

      
       
       
        
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
               transform.position = new Vector3 (
                    transform.position.x + touch.deltaPosition.x * speedModifier,
                    transform.position.y,
                    transform.position.z + touch.deltaPosition.y * speedModifier
                );
                gridMoveDirectionVector = new Vector3(touch.deltaPosition.x, 0, touch.deltaPosition.y);
                transform.eulerAngles = new Vector3(90,0,GetAngleFromVector(gridMoveDirectionVector) - 90);
                
            }
           
        }
            
            if(prevPos.x < transform.position.x) {
                gridMoveDirection = Direction.Right;
            }
            if(prevPos.x > transform.position.x) {
                gridMoveDirection = Direction.Left;
            }
            if(prevPos.z < transform.position.z) {
                gridMoveDirection = Direction.Up;
            }
            if(prevPos.z > transform.position.z) {
                gridMoveDirection = Direction.Down;
            }
            

        
         HandleGridMovement();

        
        
        

       
    }
    private void HandleGridMovement() {
        gridMoveTimer += Time.deltaTime;
        if(gridMoveTimer >= gridMoveTimerMax) {
            gridMoveTimer -= gridMoveTimerMax;

            SnakeMovePosition previousSnakeMovePosition = null;
            if(snakeMovePositionList.Count > 0) 
            {
                previousSnakeMovePosition = snakeMovePositionList[0];
            }
            SnakeMovePosition snakeMovePosition = new SnakeMovePosition(previousSnakeMovePosition, gridPosition,gridMoveDirection);
            snakeMovePositionList.Insert(0,snakeMovePosition);

            // switch(gridMoveDirection) {
            //     default:
            //     case Direction.Right:   gridMoveDirectionVector = new Vector3(+1,0,0);break;
            //     case Direction.Left:    gridMoveDirectionVector = new Vector3(-1,0,0);break;
            //     case Direction.Up:      gridMoveDirectionVector = new Vector3(0,0,+1);break;
            //     case Direction.Down:    gridMoveDirectionVector = new Vector3(0,0,-1);break;
            // }

           


            gridPosition = transform.position;

            gridPosition = levelGrid.ValidateGridPosition(gridPosition);
            
            // bool snakeAteFood = levelGrid.TrySnakeEatFood(gridPosition);

            // if(snakeAteFood) {
            //     snakeBodySize++;
            //     CreateSnakeBodyPart();
            // }
            if(snakeMovePositionList.Count >= snakeBodySize + 1) {
                snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);
            }
            UpdateSnakeBodyParts();

            foreach(SnakeBodyPart snakeBodyPart in snakeBodyPartList) {
                 if(snakeBodyPart.GetGridPosition() ==  gridPosition) {
                    //Game over
                    state = State.Dead;
                    GameHandler.SnakeDied();
                }
            }
               

        }
    }
    private void CreateSnakeBodyPart() {
        snakeBodyPartList.Add(new SnakeBodyPart(snakeBodyPartList.Count));
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
            snakeBodySize++;
            CreateSnakeBodyPart();
        }
    }
    private class SnakeBodyPart {

        private SnakeMovePosition snakeMovePosition;
        private Transform transform;
        public SnakeBodyPart(int bodyIndex) {
            GameObject snakeBodyGameObject = new GameObject("SnakeBody", typeof(SpriteRenderer));
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.snakeBodySprite;
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sortingOrder = -1 - bodyIndex;
            transform = snakeBodyGameObject.transform;
            transform.eulerAngles = new Vector3(90, transform.eulerAngles.y, transform.eulerAngles.z);
         
            
        }
        public void SetSnakeMovePosition(SnakeMovePosition snakeMovePosition) {
            this.snakeMovePosition = snakeMovePosition;
            transform.position = new Vector3(snakeMovePosition.GetGridPosition().x, snakeMovePosition.GetGridPosition().y, snakeMovePosition.GetGridPosition().z);

            float angle;
            switch(snakeMovePosition.GetDirection()) {
                default:
                case Direction.Up: //currently going up
                switch(snakeMovePosition.GetPreviousDirection()) {
                    default: angle  = 0; break;
                    case Direction.Left: angle = 0 + 45; break; //Prev was going left
                    case Direction.Right: angle = 0 - 45; break; //prev was going right

                }
                break;
                case Direction.Down: //currently going down
                switch(snakeMovePosition.GetPreviousDirection()) {
                    default: angle  = 180; break;
                    case Direction.Left: angle = 180 + 45; break; //Prev was going left
                    case Direction.Right: angle = 180 - 45; break; //prev was going right

                } 
                break;
                case Direction.Left: //currently going left
                switch(snakeMovePosition.GetPreviousDirection()) {
                    default: angle  = -90; break;
                    case Direction.Down: angle = -45; break; //Prev was going down
                    case Direction.Up: angle = 45; break; //prev was going up

                }
                break;
                case Direction.Right: //Currently going right
                switch(snakeMovePosition.GetPreviousDirection()) {
                    default: angle  = 90; break;
                    case Direction.Down: angle = 45; break; //Prev was going down
                    case Direction.Up: angle = -45; break; //prev was going up

                }
                break;
            }
            transform.eulerAngles = new Vector3(90,0,angle);

        }
        public Vector3 GetGridPosition() {
            return snakeMovePosition.GetGridPosition();
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