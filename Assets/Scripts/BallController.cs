using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public bool isMovingBall;
    public float ballSpeed = 1f;
    public Vector3 ballDestination = Vector3.zero;
    GameObject[] players;


    // Start is called before the first frame update
    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        SetDestination(ballDestination);
    }

    public void SetDestination(Vector3 destination)
    {
        Debug.Log("target distance" + Vector3.Distance(destination, this.transform.position));
        if (Vector3.Distance(destination, this.transform.position) < 6f)
        {
            isMovingBall = false;
        }
        else if (isMovingBall)
        {
            // transform.position += new Vector3(destination.x * ballSpeed * Time.deltaTime, 0f, destination.z * ballSpeed * Time.deltaTime);
            transform.position = new Vector3(destination.x, transform.position.y, destination.z);
        }
    }
}
