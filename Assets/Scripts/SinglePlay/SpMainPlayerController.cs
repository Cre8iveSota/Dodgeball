using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpMainPlayerController : MonoBehaviour
{
    SpManager singlePlayManager;
    SpBallController spBallController;
    public Animator animator;
    public bool iAmThrowing;



    // Start is called before the first frame update
    void Start()
    {
        singlePlayManager = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<SpManager>();
        if (singlePlayManager.realBallInstance != null) spBallController = singlePlayManager.realBallInstance.GetComponent<SpBallController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        singlePlayManager.CountingTimeOfHoldingShiftKey();
        if (spBallController == null) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {

            if (singlePlayManager.CheckHaveBallAsChildren(this.gameObject))
            {
                iAmThrowing = true;
                StartCoroutine(spBallController.NormalPass(singlePlayManager.mainCharaInstance, singlePlayManager.subCharaInstance));
            }
        }
        if (!iAmThrowing)
        {
            MoveMainPlayer();
            TurnMainPlayer();
        }

        // 自分がボールを持っていたら相手の方向を向く
        if (singlePlayManager.GetBallHolderTeamPlayer(true) == singlePlayManager.mainCharaInstance)
        {
            this.gameObject.transform.rotation = singlePlayManager.normalRotation;
            this.gameObject.transform.position = new Vector3(transform.position.x, 0, -6f); //new Vector3(transform.position.x, 0, 6f);
        }

    }
    private void MoveMainPlayer()
    {
        if (singlePlayManager.duration > singlePlayManager.Threshold) return;

        if (Input.GetKeyDown(KeyCode.RightArrow) && transform.position.x < 10)
        {
            transform.position = new Vector3(transform.position.x + 10, transform.position.y, transform.position.z);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && transform.position.x > -10)
        {
            transform.position = new Vector3(transform.position.x - 10, transform.position.y, transform.position.z);
        }
    }

    private void TurnMainPlayer()
    {
        if (!Input.GetKeyDown(KeyCode.Space)) return;
        // ball持っていない時
        if (singlePlayManager.Threshold < singlePlayManager.duration && !singlePlayManager.hasPlayer1TeamBall)
        {
            ClockWiseTurn();
        }
        else if (singlePlayManager.Threshold > singlePlayManager.duration && !singlePlayManager.hasPlayer1TeamBall)
        {
            AntiClockWiseTurn();
        }
    }

    private void ClockWiseTurn()
    {
        if (this.gameObject.transform.localRotation == singlePlayManager.normalRotation)
        {
            this.gameObject.transform.localRotation = singlePlayManager.normalLeftRotation;
        }
        else if (this.gameObject.transform.localRotation == singlePlayManager.normalLeftRotation)
        {
            this.gameObject.transform.localRotation = singlePlayManager.inverseRightRotation;
            this.gameObject.transform.position = new Vector3(transform.position.x, 0f, transform.position.z + 2f);
        }
        else if (this.gameObject.transform.localRotation == singlePlayManager.inverseRightRotation)
        {
            this.gameObject.transform.localRotation = singlePlayManager.inverseRotation;
        }
        else if (this.gameObject.transform.localRotation == singlePlayManager.inverseRotation)
        {
            this.gameObject.transform.localRotation = singlePlayManager.inverseLeftRotation;
        }
        else if (this.gameObject.transform.localRotation == singlePlayManager.inverseLeftRotation)
        {
            this.gameObject.transform.localRotation = singlePlayManager.normalRightRotation;
            this.gameObject.transform.position = new Vector3(transform.position.x, 0f, transform.position.z - 2f);
        }
        else if (this.gameObject.transform.localRotation == singlePlayManager.normalRightRotation)
        {
            this.gameObject.transform.localRotation = singlePlayManager.normalRotation;
        }
    }

    private void AntiClockWiseTurn()
    {
        if (this.gameObject.transform.localRotation == singlePlayManager.normalRotation)
        {
            this.gameObject.transform.localRotation = singlePlayManager.normalRightRotation;
        }
        else if (this.gameObject.transform.localRotation == singlePlayManager.normalRightRotation)
        {
            this.gameObject.transform.localRotation = singlePlayManager.inverseLeftRotation;
            this.gameObject.transform.position = new Vector3(transform.position.x, 0f, transform.position.z + 2f);
        }
        else if (this.gameObject.transform.localRotation == singlePlayManager.inverseLeftRotation)
        {
            this.gameObject.transform.localRotation = singlePlayManager.inverseRotation;
        }
        else if (this.gameObject.transform.localRotation == singlePlayManager.inverseRotation)
        {
            this.gameObject.transform.localRotation = singlePlayManager.inverseRightRotation;
        }
        else if (this.gameObject.transform.localRotation == singlePlayManager.inverseRightRotation)
        {
            this.gameObject.transform.localRotation = singlePlayManager.normalLeftRotation;
            this.gameObject.transform.position = new Vector3(transform.position.x, 0f, transform.position.z - 2f);

        }
        else if (this.gameObject.transform.localRotation == singlePlayManager.normalLeftRotation)
        {
            this.gameObject.transform.localRotation = singlePlayManager.normalRotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (singlePlayManager == null || singlePlayManager.GetBallHolderTeamPlayer(true) == singlePlayManager.empty) return;
        if (singlePlayManager.GetBallHolderTeamPlayer(true) == this.gameObject) return;
        if (singlePlayManager.hasPlayer1TeamBall) { spBallController.isReceiverCatchSuccess = true; return; }
        if (spBallController.enableCatchBall)
        {
            spBallController.enableBallInterupt = true;
        }
        else
        {
            ProcedureOfHit();
        }
    }

    private void ProcedureOfHit()
    {
        animator.SetBool("isHit", true);
    }

    private void StandUpChara()
    {
        animator.SetBool("isHit", false);
        spBallController.ChangeBallOwnerToPlayer();
    }
}
