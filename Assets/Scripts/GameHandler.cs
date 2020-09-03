using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class GameHandler : MonoBehaviour
{

    [SerializeField] private Snake snake;
    private LevelGrid levelGrid;
    void Start()
    {
        levelGrid = new LevelGrid(10,10);

        snake.Setup(levelGrid);
        levelGrid.Setup(snake);
    }

}
