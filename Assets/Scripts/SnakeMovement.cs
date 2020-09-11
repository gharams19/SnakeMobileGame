using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMovement : MonoBehaviour
{

    private List<Transform> BodyParts = new List<Transform>();

    
    public float mindistance = 0.25f;

    public float speed = 1;
    public float rotationSpeed = 50;

    public GameObject bodyprefab;
    private float distance;
    private Transform curBodyPart;
    private Transform PrevBodyPart;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Move() {
       

    }
    public void AddBodyPart() {
        Transform newpart = (Instantiate(bodyprefab, BodyParts[BodyParts.Count - 1].position, BodyParts[BodyParts.Count - 1].rotation) as GameObject).transform;
        newpart.SetParent(transform);
        BodyParts.Add(newpart);

    }
}
