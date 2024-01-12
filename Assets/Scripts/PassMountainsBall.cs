using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassMountainsBall : MonoBehaviour
{
    private GameObject ball;
    PlayerController playerController;
    BallController ballController;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        ball = GameObject.FindGameObjectWithTag("Ball");
        // GameObject ball = transform.Find("Ball").gameObject;
        ballController = GetComponent<BallController>();
    }
    private void Update()
    {
        Debug.Log($"{this.gameObject.name}  {playerController?.isActiveCharacter}");
    }

    public void PassMountainousBall(GameObject fellow)
    {
        ballController.ballDestination = new Vector3(fellow.transform.position.x - ball.transform.position.x, 0f, fellow.transform.position.z - ball.transform.position.z);
        ballController.isMovingBall = true;
        // if (playerController.isBallholder) ball = transform.Find("Ball").gameObject;
        // if (ball != null)
        // {
        //     ball.transform.SetParent(fellow.transform, false);
        // }
    }
}
