using System.Collections;
using System.Collection.Collection.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class Snake : MonoBehaviour {
    private Vector3 gridMoveDirection;
    private Vector3 gridPosition;
    private float gridMoveTimer;
    private float gridMoveTimerMax;
    private LevelGrid levelGrid;
    private int snakeBodySize;
    private List<Vector3> snakeMovePositionList;
    private List<SnakeBodyPart> snakeBodyPartList;


    public void Setup(LevelGrid levelGrid) {
        this.levelGrid = levelGrid;
        
    }

    private void Awake() {
        gridPosition = new Vector3(10,10,10);
        gridMoveTimerMax = 1f;
        gridMoveTimer = gridMoveTimerMax;
        gridMoveDirection = new Vector3(1,10,0);

        snakeMovePositionList = new List<Vector3>();
        snakeBodySize = 0;
        snakeBodyPartList = new List<SnakeBodyPart>();
    }
    private void Update() {
        HandleInput();
        HandleGridMovement();
    }
    private void HandleInput() {
         if(Input.GetKeyDown(KeyCode.UpArrow)) {
            if(gridMoveDirection.z != -1) {
                gridMoveDirection.x = 0;
                gridMoveDirection.z = +1;
            }
        }
        if(Input.GetKeyDown(KeyCode.DownArrow)) {
            if(gridMoveDirection.z != +1) {
                gridMoveDirection.x = 0;
                gridMoveDirection.z = -1;
            }
        }
        if(Input.GetKeyDown(KeyCode.RightArrow)) {
            if(gridMoveDirection.x != -1) {
                gridMoveDirection.x = +1;
                gridMoveDirection.z = 0;
            }
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow)) {
            if(gridMoveDirection.x != +1){
                gridMoveDirection.x = -1;
                gridMoveDirection.z = 0;
            }
        }

       
    }
    private HandleGridMovement() {
         gridMoveTimer += Time.deltaTime;
        if(gridMoveTimer >= gridMoveTimerMax) {
            gridMoveTimer -= gridMoveTimerMax;

            snakeMovePositionList.Insert(0,gridPosition);

            gridPosition += gridMoveDirection;
            
            bool snakeAteFood = levelGrid.TrySnakeEatFood(gridPosition);

            if(snakeAteFood) {
                snakeBodySize++;
                CreateSnakeBodyPart();
            }
            if(snakeMovePositionList.Count >= snakeBodySize + 1) {
                snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);
            }

            transform.position = new Vector3(gridPosition.x, gridPosition.y, gridPosition.z);
            transform.eulerAngles = new Vector3(90,0,GetAngleFromVector(gridMoveDirection) - 90);

            UpdateSnakeBodyParts();
        }
    }
    private void CreateSnakeBodyPart() {
        snakeBodyPartList.Add(new SnakeBodyPart(snakeBodyPartList.Count));
    }

    private void UpdateSnakeBodyParts() {
        for(int i = 0; i < snakeBodyPartList.Count; i++) {
                snakeBodyPartList[i].SetGridPosition(snakeMovePositionList[i]);
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
        gridPositionList.AddRange(snakeMovePositionList);
        return gridPositionList;
    }
    private class SnakeBodyPart {

        private Vector3 gridPosition;
        private Transform transform;
        public SnakeBodyPart(int bodyIndex) {
            GameObject snakeBodyGameObject = new GameObject("SnakeBody", typeof(SpriteRenderer));
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.snaekBodySprite;
            snakeBodyTransformList.Add(snakeBodyGameObject.transform);
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sortingOrder = -bodyIndex;
        }
        public void SetGridPosition(Vector3 gridPosition) {
            this.gridPosition = gridPosition;
            transform.position = new Vector3(gridPosition.x, gridPosition.y, gridPosition.z);

        }
    }
}