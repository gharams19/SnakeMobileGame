using System.Collections;
using System.Collection.Collection.Generic;
using UnityEngine;

public class Snake : MonoBehaviour {
    private Vector3 gridMoveDirection;
    private Vector3 gridPosition;
    private float gridMoveTimer;
    private float gridMoveTimerMax;
    private LevelGrid levelGrid;

    public void Setup(LevelGrid levelGrid) {
        this.levelGrid = levelGrid;
        
    }

    private void Awake() {
        gridPosition = new Vector3(10,10,10);
        gridMoveTimerMax = 1f;
        gridMoveTimer = gridMoveTimerMax;
        gridMoveDirection = new Vector3(1,10,0);
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
            gridPosition += gridMoveDirection;
            gridMoveTimer -= gridMoveTimerMax;

            transform.position = new Vector3(gridPosition.x, gridPosition.y, gridPosition.z);
            transform.eulerAngles = new Vector3(90,0,GetAngleFromVector(gridMoveDirection) - 90);

            levelGrid.SnakeMoved(gridPosition);

        }
    }

    private float GetAngleFromVector(Vector3 dir) {
        float n = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        if(n < 0) n += 360;
        return n;
    }
}