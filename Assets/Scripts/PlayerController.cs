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
    BallController ballController;
    GameManager gameManager;
    //長押しと判定するフレーム数を管理
    const int _threshold = 30;
    //キーを押しているフレーム数を記録
    int _duration = 0;

    SubPlayerController subPlayerController;
    // Start is called before the first frame update
    void Start()
    {
        ball = GameObject.FindGameObjectWithTag("Ball");
        ballController = ball.GetComponent<BallController>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<GameManager>();
        subPlayerController = gameManager.subChara.GetComponent<SubPlayerController>();
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
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            Debug.Log("長押し");
            _duration = _duration + 1;
            Debug.Log("_duration" + _duration);
        }
        else
        {
            subPlayerController.isPositionAuto = true;
        }
        if (_duration > _threshold && Input.GetKeyDown(KeyCode.RightArrow))
        {
            _duration = 0;
            if (gameManager.subChara.transform.position.x < 10 && Input.GetKeyDown(KeyCode.RightArrow))
            {
                gameManager.subChara.transform.position = new Vector3(gameManager.subChara.transform.position.x + 10, gameManager.subChara.transform.position.y, gameManager.subChara.transform.position.z);
                subPlayerController.isPositionAuto = false;
            }
            return;
        }
        else if (_duration > _threshold && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _duration = 0;
            if (gameManager.subChara.transform.position.x > -10 && Input.GetKeyDown(KeyCode.LeftArrow))
            {
                gameManager.subChara.transform.position = new Vector3(gameManager.subChara.transform.position.x - 10, gameManager.subChara.transform.position.y, gameManager.subChara.transform.position.z);
                subPlayerController.isPositionAuto = false;
            }
            return;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) && transform.position.x < 15 && transform.position.z < -10)
        {
            transform.position = new Vector3(transform.position.x + 10, transform.position.y, transform.position.z);
            subPlayerController.isPositionAuto = true;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && transform.position.x < 10 && 4 < transform.position.z && transform.position.z < 6)
        {
            transform.position = new Vector3(transform.position.x + 10, transform.position.y, transform.position.z);
            subPlayerController.isPositionAuto = true;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && transform.position.x > -15 && transform.position.z < -10)
        {
            transform.position = new Vector3(transform.position.x - 10, transform.position.y, transform.position.z);
            subPlayerController.isPositionAuto = true;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && transform.position.x > -10 && 4 < transform.position.z && transform.position.z < 6)
        {
            transform.position = new Vector3(transform.position.x - 10, transform.position.y, transform.position.z);
            subPlayerController.isPositionAuto = true;
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
