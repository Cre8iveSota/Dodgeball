using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool isActiveCharacter = true;
    private GameObject ball;
    GameObject[] playerTeam;
    PlayerController mem0, mem1;
    PassMountainsBall passMountainsBall;
    BallController ballController;
    GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        ball = GameObject.FindGameObjectWithTag("Ball");
        playerTeam = GameObject.FindGameObjectsWithTag("Player");
        passMountainsBall = GetComponent<PassMountainsBall>();
        ballController = ball.GetComponent<BallController>();
        mem0 = playerTeam[0].GetComponent<PlayerController>();
        mem1 = playerTeam[1].GetComponent<PlayerController>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        JudgeActivePlayer();
        if (isActiveCharacter)
        {
            // move and throw is allowed
            MoveMainPlayer();
            NormalPass();
        }
        else
        {
            // exchange components within team
        }
    }

    private void NormalPass()
    {
        if (!Input.GetKeyDown(KeyCode.Space)) return;
        if (ballController != null)
        {
            ballController.ballDestination = gameManager.subChara.transform.position;
            ballController.isMovingBall = true;
            ball.transform.SetParent(gameManager.subChara.transform, false);
        }
    }

    private void MoveMainPlayer()
    {
        if (!this.isActiveCharacter) return;
        if (Input.GetKeyDown(KeyCode.RightArrow) && transform.position.x < 15)
        {
            transform.position = new Vector3(transform.position.x + 10, transform.position.y, transform.position.z);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && transform.position.x > -15)
        {
            transform.position = new Vector3(transform.position.x - 10, transform.position.y, transform.position.z);
        }
    }

    private void JudgeActivePlayer()
    {
        Debug.Log($"{this.name}  {Vector3.Distance(this.transform.position, ball.transform.position)}");
        if (Vector3.Distance(this.transform.position, ball.transform.position) < 6f)
        {
            isActiveCharacter = true;
        }
        else
        {
            isActiveCharacter = false;
            gameManager.isChangeActiveCharacter = true;
        }
    }
}
