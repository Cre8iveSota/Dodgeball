using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float playerMovement;
    bool isRight = false;
    bool isLeft = false;
    bool isWorking = false;
    private bool isBallholder = true;
    [SerializeField] float waitTime = 5f;
    GameObject ball;
    GameObject[] playerTeam;
    // Start is called before the first frame update
    void Start()
    {
        ball = GameObject.FindGameObjectWithTag("Ball");
        playerTeam = GameObject.FindGameObjectsWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        CheckAmIballholder();
        Debug.Log("chrkball" + isBallholder);
        playerMovement = Input.GetAxisRaw("Horizontal");
        if (!isWorking && this.isBallholder)
        {
            isWorking = true;
            StartCoroutine(MoveMainPlayer());
        }
        else if (!this.isBallholder)
        {
            MoveBallHolderTeamSubPlayer();
        }
    }

    private IEnumerator MoveMainPlayer()
    {
        yield return new WaitForSeconds(0.1f * Time.deltaTime * waitTime);
        if (playerMovement > 0)
        {
            isRight = true;
        }
        if (playerMovement < 0)
        {
            isLeft = true;
        }
        if (isRight && transform.position.x < 15)
        {
            transform.position = new Vector3(transform.position.x + 10, transform.position.y, transform.position.z);
            isRight = false;
        }
        if (isRight && transform.position.x > 15)
        {
            playerMovement = 0;
        }
        if (isLeft && transform.position.x > -15)
        {
            transform.position = new Vector3(transform.position.x - 10, transform.position.y, transform.position.z);
            isLeft = false;
        }
        if (isLeft && transform.position.x < -15)
        {
            playerMovement = 0;
        }
        yield return new WaitForSeconds(0.05f * Time.deltaTime * waitTime);
        isWorking = false;
    }

    private void CheckAmIballholder()
    {
        Debug.Log($"{this.name}  {Vector3.Distance(this.transform.position, ball.transform.position)}");
        if (Vector3.Distance(this.transform.position, ball.transform.position) < 6f)
        {
            isBallholder = true;
            Debug.Log("come");
        }
        else { isBallholder = false; }
    }

    void MoveBallHolderTeamSubPlayer()
    {
        if (GetMainTeamMember().transform.position.x <= 10 && GetMainTeamMember().transform.position.x >= -10)
        {
            transform.position = new Vector3(GetMainTeamMember().transform.position.x, this.transform.position.y, this.transform.position.z);
        }
    }


    // 現状ではボールを持っているチームの味方しか取得できないため、修正必要
    GameObject GetMainTeamMember()
    {
        return playerTeam.FirstOrDefault(player => player.GetComponent<PlayerController>().isBallholder);
    }
}
