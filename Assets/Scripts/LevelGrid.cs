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
    private void SpawnFood() {

        do {
            foodGridPosition = new Vector3(Random.Range(0,width), 10, Random.Range(0,height));
        } while(snake.GetFullSnakeGridPositionList().IndexOf(foodGridPosition) != -1);

        foodGameObject = new GameObject("Food", typeof(SpriteRenderer));
        foodGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.instance.foodSprite;
        foodGameObject.transform.position = new Vector3(foodGridPosition.x, 10, foodGridPosition.z);
        var rotationVector = foodGameObject.transform.rotation.eulerAngles;
        rotationVector.x = 90f;
        foodGameObject.transform.rotation = Quaternion.Euler(rotationVector);


    }
    
    public bool TrySnakeEatFood(Vector3 snakeGridPosition) {
        if(snakeGridPosition == foodGridPosition) {
            Destroy(foodGameObject);
            SpawnFood();
            return true;
        
        }
        else {
            return false;
        }

    }
}
