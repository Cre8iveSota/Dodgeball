using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpMainPlayerController : MonoBehaviour
{
    SpManager spManager;
    SpBallController spBallController;
    public Animator animator;
    public bool iAmThrowing;
    GameObject caution;
    SpGroundController spGroundController;
    SpEnemyController spEnemyController;


    // Start is called before the first frame update
    void Start()
    {
        spManager = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<SpManager>();
        caution = spManager.mainCharaInstance.transform.Find("Caution").gameObject;
        if (spManager.realBallInstance != null) spBallController = spManager.realBallInstance.GetComponent<SpBallController>();
        animator = GetComponent<Animator>();
        GameObject ground = GameObject.FindGameObjectWithTag("Ground");
        if (ground != null) spGroundController = ground.GetComponent<SpGroundController>();
        spEnemyController = spManager.enemyInstance.GetComponent<SpEnemyController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!iAmThrowing && !spManager.subCharaInstance.GetComponent<SpsubController>().iAmThrowing) spEnemyController.EnableDisplayCaution(false);

        spManager.CountingTimeOfHoldingShiftKey();
        if (spBallController == null) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {

            if (spManager.CheckHaveBallAsChildren(this.gameObject))
            {
                if (spGroundController.ballposition != spGroundController.defenciblePosition && !spBallController.enableCatchBall)
                {
                    spEnemyController.EnableDisplayCaution(true);
                }

                iAmThrowing = true;
                StartCoroutine(spBallController.NormalPass(spManager.mainCharaInstance, spManager.subCharaInstance));
            }
        }
        if (!iAmThrowing)
        {
            MoveMainPlayer();
            TurnMainPlayer();
        }

        // 自分がボールを持っていたら相手の方向を向く
        if (spManager.GetBallHolderTeamPlayer(true) == spManager.mainCharaInstance)
        {
            this.gameObject.transform.rotation = spManager.normalRotation;
            this.gameObject.transform.position = new Vector3(transform.position.x, 0, -6f); //new Vector3(transform.position.x, 0, 6f);
        }
    }
    private void MoveMainPlayer()
    {
        if (spManager.duration > spManager.Threshold) return;

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
        if (spManager.Threshold < spManager.duration && !spManager.hasPlayer1TeamBall)
        {
            ClockWiseTurn();
        }
        else if (spManager.Threshold > spManager.duration && !spManager.hasPlayer1TeamBall)
        {
            AntiClockWiseTurn();
        }
    }

    private void ClockWiseTurn()
    {
        if (this.gameObject.transform.localRotation == spManager.normalRotation)
        {
            this.gameObject.transform.localRotation = spManager.normalLeftRotation;

        }
        else if (this.gameObject.transform.localRotation == spManager.normalLeftRotation)
        {
            this.gameObject.transform.localRotation = spManager.inverseRightRotation;
            this.gameObject.transform.position = new Vector3(transform.position.x, 0f, transform.position.z + 2f);
        }
        else if (this.gameObject.transform.localRotation == spManager.inverseRightRotation)
        {
            this.gameObject.transform.localRotation = spManager.inverseRotation;
        }
        else if (this.gameObject.transform.localRotation == spManager.inverseRotation)
        {
            this.gameObject.transform.localRotation = spManager.inverseLeftRotation;
        }
        else if (this.gameObject.transform.localRotation == spManager.inverseLeftRotation)
        {
            this.gameObject.transform.localRotation = spManager.normalRightRotation;
            this.gameObject.transform.position = new Vector3(transform.position.x, 0f, transform.position.z - 2f);
        }
        else if (this.gameObject.transform.localRotation == spManager.normalRightRotation)
        {
            this.gameObject.transform.localRotation = spManager.normalRotation;
        }
    }

    private void AntiClockWiseTurn()
    {
        if (this.gameObject.transform.localRotation == spManager.normalRotation)
        {
            this.gameObject.transform.localRotation = spManager.normalRightRotation;
        }
        else if (this.gameObject.transform.localRotation == spManager.normalRightRotation)
        {
            this.gameObject.transform.localRotation = spManager.inverseLeftRotation;
            this.gameObject.transform.position = new Vector3(transform.position.x, 0f, transform.position.z + 2f);
        }
        else if (this.gameObject.transform.localRotation == spManager.inverseLeftRotation)
        {
            this.gameObject.transform.localRotation = spManager.inverseRotation;
        }
        else if (this.gameObject.transform.localRotation == spManager.inverseRotation)
        {
            this.gameObject.transform.localRotation = spManager.inverseRightRotation;
        }
        else if (this.gameObject.transform.localRotation == spManager.inverseRightRotation)
        {
            this.gameObject.transform.localRotation = spManager.normalLeftRotation;
            this.gameObject.transform.position = new Vector3(transform.position.x, 0f, transform.position.z - 2f);

        }
        else if (this.gameObject.transform.localRotation == spManager.normalLeftRotation)
        {
            this.gameObject.transform.localRotation = spManager.normalRotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (spManager == null || spManager.GetBallHolderTeamPlayer(true) == spManager.empty) return;
        if (spManager.GetBallHolderTeamPlayer(true) == this.gameObject) return;
        if (spManager.hasPlayer1TeamBall) { spBallController.isReceiverCatchSuccess = true; return; }
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

    public void EnableDisplayCaution(bool isActivate)
    {
        caution.gameObject.SetActive(isActivate);
    }
}
