using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using CodeMonkey;
using CodeMonkey.Utils;

public class LevelGrid : MonoBehaviour
{
    private Vector3 foodGridPosition;
    private GameObject foodGameObject;
    private int width;
    private int height;
    private Snake snake;

    public LevelGrid(int width, int height) {
        this.width = width;
        this.height = height;

    }
    
    public void Setup(Snake snake) {
        this.snake = snake;
        SpawnFood();

    }
    public void SpawnFood() {

        do {
            foodGridPosition = new Vector3(Random.Range(0,width-1), 10, Random.Range(0,height-1));
        } while(snake.GetFullSnakeGridPositionList().IndexOf(foodGridPosition) != -1);

        foodGameObject = new GameObject("Food", typeof(SpriteRenderer));
        foodGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.foodSprite;
        foodGameObject.transform.position = new Vector3(foodGridPosition.x, 10, foodGridPosition.z);
        var rotationVector = foodGameObject.transform.rotation.eulerAngles;
        rotationVector.x = 90f;
        foodGameObject.transform.rotation = Quaternion.Euler(rotationVector);
        BoxCollider boxCollider = foodGameObject.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;

    }
    
    public void snakeAteFood() {
        Destroy(foodGameObject);
        SpawnFood();
        GameHandler.AddScore();
    }
    // public bool TrySnakeEatFood(Vector3 snakeGridPosition) {
    
    //     if(snakeGridPosition == foodGridPosition) {
    //         Destroy(foodGameObject);
    //         SpawnFood();
    //         GameHandler.AddScore();
    //         return true;
        
    //     }
    //     else {
    //         return false;
    //     }

    // }
    public Vector3 ValidateGridPosition(Vector3 gridPosition) { 
        if(gridPosition.x < 0) {
            gridPosition.x = width - 1;
        }
        if(gridPosition.x > width - 1) {
            gridPosition.x = 0;
        }
        if(gridPosition.z <= -height) {
            gridPosition.z = height - 1;
        }
        if(gridPosition.z > height - 1) {
            gridPosition.z = -(height-1);
        }
        return gridPosition;
    }
}
