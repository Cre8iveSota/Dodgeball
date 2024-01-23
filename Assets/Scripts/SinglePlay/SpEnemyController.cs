using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpEnemyController : MonoBehaviour
{
    SpManager spManager;
    SpBallController spBallController;
    public Animator animator;
    public bool iAmThrowing;
    private float elapsedTime;

    private bool pseudoPressSpase, pseudoPressRight, pseudoPressLeft = false;
    public bool canAttack;
    SpGroundController spGroundController;
    float enemyPoistionX, enemySubPoistionX, playerSubPositionX, playerPositionX;
    bool isExecute;
    float ballHolderPositionX, fellowOfBallHolderPositionX;
    SpSubEnemyController spSubEnemyController;
    GameObject caution;
    SpMainPlayerController spMainPlayerController;
    // Start is called before the first frame update
    void Start()
    {
        spManager = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<SpManager>();
        caution = spManager.enemyInstance.transform.Find("Caution").gameObject;
        if (spManager.realBallInstance != null) spBallController = spManager.realBallInstance.GetComponent<SpBallController>();
        animator = GetComponent<Animator>();
        GameObject ground = GameObject.FindGameObjectWithTag("Ground");
        if (ground != null) spGroundController = ground.GetComponent<SpGroundController>();
        spSubEnemyController = spManager.subEnemyInstance.GetComponent<SpSubEnemyController>();
        spMainPlayerController = spManager.mainCharaInstance.GetComponent<SpMainPlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        // Time.fixedDeltaTime = spManager.actionSpeedEnemy;

        if (spBallController.throwMan == null) spMainPlayerController.EnableDisplayCaution(false);

        enemyPoistionX = spManager.enemyInstance.transform.position.x;
        enemySubPoistionX = spManager.subEnemyInstance.transform.position.x;
        playerSubPositionX = spManager.subCharaInstance.transform.position.x;
        playerPositionX = spManager.mainCharaInstance.transform.position.x;
        ballHolderPositionX = spManager.GetBallHolderTeamPlayer(true).transform.position.x;
        fellowOfBallHolderPositionX = spManager.GetBallHolderTeamPlayer(false).transform.position.x;

        // 自分がボールを持っていたら相手の方向を向く
        if (spManager.GetBallHolderTeamPlayer(true) == spManager.enemyInstance)
        {
            this.gameObject.transform.rotation = spManager.inverseRotation;
            this.gameObject.transform.position = new Vector3(transform.position.x, 0, 6f); //new Vector3(transform.position.x, 0, 6f);
        }

        if (iAmThrowing || spSubEnemyController.iAmThrowing)
        {
            if (!spManager.hasPlayer1TeamBall && !spBallController.enableCatchBall)
            {
                spMainPlayerController.EnableDisplayCaution(true);
            }
        }

        if (elapsedTime > spManager.actionSpeedEnemy)
        {
            EnemyAction();
            elapsedTime = 0;
        }
    }

    // void FixedUpdate()
    // {
    //     EnemyAction();
    // }

    private void EnemyAction()
    {
        int randomNum = Random.Range(1, 36);
        // Enemy Team has ball
        if (!spManager.hasPlayer1TeamBall)
        {
            // Enemy main has ball
            if (spManager.CheckHaveBallAsChildren(spManager.enemyInstance))
            {
                // if Enemy main is still throwing ball, you cannot do any action
                if (iAmThrowing) return;
                // Sometimes enemy main thow ball randomely.
                if (randomNum % 36 == 0)
                {
                    pseudoPressSpase = true;
                    EnemyThrow();
                }
                else if (spGroundController.ballposition == spGroundController.defenciblePosition)
                {
                    if (randomNum % 9 == 0) { pseudoPressSpase = true; EnemyThrow(); }
                    else if (randomNum % 10 == 0) spSubEnemyController.MoveEnemySubToRight();
                    else if (randomNum % 11 == 0) spSubEnemyController.MoveEnemySubToLeft();
                    else { DodgeOrTakeUpDefensiveRotation(randomNum, false); }
                }
                // if player in the midlle position between enemy and enemySub also player do not prepare for defence, enemy throw ball
                else if (IsPositionBTWballHolderTeamFromTargetView(spManager.mainCharaInstance) && spGroundController.ballposition != spGroundController.defenciblePosition)
                {
                    pseudoPressSpase = true;
                    EnemyThrow();
                }
                // if enemy position equals player position also it has difference between enemySub and enemy position, make move enemy or enemySub 
                else if (enemyPoistionX == playerPositionX && enemyPoistionX < enemySubPoistionX)
                {
                    if (randomNum % 2 == 0) { pseudoPressRight = true; MoveEnemyToRight(); }
                    if (randomNum % 2 == 1) spSubEnemyController.MoveEnemySubToRight();
                    return;
                }
                else if (enemyPoistionX == playerPositionX && enemySubPoistionX < enemyPoistionX)
                {
                    { pseudoPressLeft = true; MoveEnemyToLeft(); }
                    spSubEnemyController.MoveEnemySubToLeft();
                    return;
                }
                // if enemy position not equal player position, make move enemy or enemySub 
                else if (enemyPoistionX < playerPositionX)
                {
                    if (randomNum % 2 == 0) { pseudoPressLeft = true; MoveEnemyToLeft(); }
                    if (randomNum % 2 == 1) spSubEnemyController.MoveEnemySubToLeft();
                    return;
                }
                else if (playerPositionX < enemyPoistionX)
                {
                    if (randomNum % 2 == 0) { pseudoPressRight = true; MoveEnemyToRight(); }
                    if (randomNum % 2 == 1) spSubEnemyController.MoveEnemySubToRight();
                    return;
                }
            }
            // if enemySub has ball
            else if (spManager.CheckHaveBallAsChildren(spManager.subEnemyInstance))
            {
                // sometimes enemySub throw ball at random
                if (randomNum % 9 == 0) { spSubEnemyController.EnemySubThrow(); }
                // if player is in the middle of enemy and enemySub also player do not prepare for defence, throw ball
                else if (IsPositionBTWballHolderTeamFromTargetView(spManager.mainCharaInstance) && spGroundController.ballposition != spGroundController.defenciblePosition)
                {
                    spSubEnemyController.EnemySubThrow();
                }
                // if player is in the middle of enemy and enemySub also player prepared for defence, move at random
                else if (IsPositionBTWballHolderTeamFromTargetView(spManager.mainCharaInstance) && spGroundController.ballposition == spGroundController.defenciblePosition)
                {
                    if (randomNum % 4 == 0) { pseudoPressLeft = true; MoveEnemyToLeft(); }
                    if (randomNum % 4 == 1) { pseudoPressRight = true; MoveEnemyToRight(); }
                    if (randomNum % 4 == 2) spSubEnemyController.MoveEnemySubToRight();
                    if (randomNum % 4 == 3) spSubEnemyController.MoveEnemySubToLeft();
                }
                // if enemySub position is as same as player, however enemy position is in left side from enemySub, make move enemy or enemySub
                else if (enemySubPoistionX == playerPositionX && enemyPoistionX < enemySubPoistionX)
                {
                    if (randomNum % 2 == 0) { pseudoPressLeft = true; MoveEnemyToLeft(); }
                    if (randomNum % 2 == 1) spSubEnemyController.MoveEnemySubToLeft();
                    return;
                }
                else if (enemySubPoistionX == playerPositionX && enemySubPoistionX < enemyPoistionX)
                {
                    if (randomNum % 2 == 0) { pseudoPressRight = true; MoveEnemyToRight(); }
                    if (randomNum % 2 == 1) spSubEnemyController.MoveEnemySubToRight();
                    return;
                }
                // if enemySub position is not equal player, make move enemy or enemySub
                else if (enemySubPoistionX < playerPositionX)
                {
                    Debug.Log("hew");
                    if (randomNum % 2 == 0) { pseudoPressLeft = true; MoveEnemyToLeft(); }
                    if (randomNum % 2 == 1) spSubEnemyController.MoveEnemySubToLeft();
                    return;
                }
                else if (playerPositionX < enemySubPoistionX)
                {
                    Debug.Log("hew2");
                    if (randomNum % 2 == 0) { pseudoPressRight = true; MoveEnemyToRight(); }
                    if (randomNum % 2 == 1) spSubEnemyController.MoveEnemySubToRight();
                    return;
                }
            }
        }
        // if enemy team do not have a ball
        if (spManager.hasPlayer1TeamBall)
        {
            if (spGroundController.ballposition == spGroundController.defenciblePosition) return;
            DodgeOrTakeUpDefensiveRotation(randomNum, true);
        }
    }

    private void DodgeOrTakeUpDefensiveRotation(int randomNum, bool isInDefence)
    {
        if (isInDefence)
        {
            if (ballHolderPositionX > enemyPoistionX + 10 && spBallController.throwMan == null) { pseudoPressLeft = true; MoveEnemyToLeft(); }
            else if (ballHolderPositionX < enemyPoistionX - 10 && spBallController.throwMan == null) { pseudoPressRight = true; MoveEnemyToRight(); }
            else if (CanMoveRightFromTargetView(this.gameObject) && CanMoveLeftFromTargetView(this.gameObject))
            {
                if (randomNum % 3 == 0) { pseudoPressRight = true; MoveEnemyToRight(); }
                else if (randomNum % 3 == 1) { pseudoPressLeft = true; MoveEnemyToLeft(); }
                else if (randomNum % 3 == 2) { pseudoPressSpase = true; TurnEnemyToBallHolder(randomNum); }
            }
            else if (CanMoveRightFromTargetView(this.gameObject) && !CanMoveLeftFromTargetView(this.gameObject))
            {
                if (randomNum % 2 == 0) { pseudoPressRight = true; MoveEnemyToRight(); }
                else if (randomNum % 2 == 1) { pseudoPressSpase = true; TurnEnemyToBallHolder(randomNum); }
            }
            else if (!CanMoveRightFromTargetView(this.gameObject) && CanMoveLeftFromTargetView(this.gameObject))
            {
                if (randomNum % 2 == 0) { pseudoPressLeft = true; MoveEnemyToLeft(); }
                else if (randomNum % 2 == 1) { pseudoPressSpase = true; TurnEnemyToBallHolder(randomNum); }
            }
        }
        else
        {
            if (CanMoveRightFromTargetView(this.gameObject) && CanMoveLeftFromTargetView(this.gameObject))
            {
                if (randomNum % 2 == 0) { pseudoPressRight = true; MoveEnemyToRight(); }
                else if (randomNum % 2 == 1) { pseudoPressLeft = true; MoveEnemyToLeft(); }
            }
            else if (CanMoveRightFromTargetView(this.gameObject) && !CanMoveLeftFromTargetView(this.gameObject))
            {
                pseudoPressRight = true; MoveEnemyToRight();
            }
            else if (!CanMoveRightFromTargetView(this.gameObject) && CanMoveLeftFromTargetView(this.gameObject))
            {
                pseudoPressLeft = true; MoveEnemyToLeft();
            }
        }
    }

    private bool IsPositionBTWballHolderTeamFromTargetView(GameObject targetInstance)
    {
        float targetPositionX = targetInstance.transform.position.x;

        if ((ballHolderPositionX < targetPositionX && targetPositionX < fellowOfBallHolderPositionX)
        || (fellowOfBallHolderPositionX < targetPositionX && targetPositionX < ballHolderPositionX))
        {
            return true;
        }
        else if (targetPositionX == ballHolderPositionX && targetPositionX == fellowOfBallHolderPositionX)
        {
            return true;
        }
        return false;
    }

    private bool CanMoveRightFromTargetView(GameObject targetInstance)
    {
        // enemy1初期の ↓ 向きで見て右 
        if (targetInstance == spManager.enemyInstance)
        {
            if (enemyPoistionX > -10) { return true; }
            return false;
        }
        else if (targetInstance == spManager.subEnemyInstance)
        {
            if (enemyPoistionX > -20) { return true; }
            return false;
        }
        Debug.LogWarning("CanMoveRight check Doesn't work");
        return false;
    }

    private bool CanMoveLeftFromTargetView(GameObject targetInstance)
    {
        // enemy1初期の ↓ 向きで見て左
        if (targetInstance == spManager.enemyInstance)
        {
            if (enemyPoistionX < 10) { return true; }
            return false;
        }
        else if (targetInstance == spManager.subEnemyInstance)
        {
            if (enemyPoistionX < 20) { return true; }
            return false;
        }
        Debug.LogWarning("CanMoveRight check Doesn't work");
        return false;
    }

    private void EnemyThrow()
    {
        if (pseudoPressSpase)
        {
            pseudoPressSpase = false;
            if (spManager.CheckHaveBallAsChildren(this.gameObject))
            {
                iAmThrowing = true;
                StartCoroutine(spBallController.NormalPass(spManager.enemyInstance, spManager.subEnemyInstance));
            }
        }
    }

    private void MoveEnemyToRight()
    {
        if (!pseudoPressRight) return;
        if (pseudoPressRight && transform.position.x > -10)
        {
            SoundManager.instance.PlaySE(3);
            transform.position = new Vector3(transform.position.x - 10, transform.position.y, transform.position.z);
            pseudoPressRight = false;

        }
    }

    private void MoveEnemyToLeft()
    {
        if (!pseudoPressLeft) return;
        if (pseudoPressLeft && transform.position.x < 10)
        {
            SoundManager.instance.PlaySE(3);
            transform.position = new Vector3(transform.position.x + 10, transform.position.y, transform.position.z);
            pseudoPressLeft = false;
        }
    }

    private void TurnEnemyToBallHolder(int randomNum)
    {
        if (!spManager.hasPlayer1TeamBall) return;
        if (spManager.GetBallHolderTeamPlayer(true) == spManager.mainCharaInstance)
        {
            if (playerPositionX < enemyPoistionX) { AntiClockWiseTurn(); return; }
            if (enemyPoistionX < playerPositionX) { ClockWiseTurn(); return; }
            if (enemyPoistionX == playerPositionX && randomNum % 2 == 0) { ClockWiseTurn(); return; }
            if (enemyPoistionX == playerPositionX && randomNum % 2 == 1) { AntiClockWiseTurn(); return; }

        }
        else if (spManager.GetBallHolderTeamPlayer(true) == spManager.subCharaInstance)
        {
            if (playerSubPositionX < enemyPoistionX) { ClockWiseTurn(); return; }
            if (enemyPoistionX < playerSubPositionX) { AntiClockWiseTurn(); return; }
            if (enemyPoistionX == playerPositionX && randomNum % 2 == 0) { ClockWiseTurn(); return; }
            if (enemyPoistionX == playerPositionX && randomNum % 2 == 1) { AntiClockWiseTurn(); return; }
        }
    }

    private void ClockWiseTurn()
    {
        if (!pseudoPressSpase) return;
        pseudoPressSpase = false;
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
        if (!pseudoPressSpase) return;
        pseudoPressSpase = false;
        if (this.gameObject.transform.localRotation == spManager.normalRotation)
        {
            this.gameObject.transform.localRotation = spManager.normalRightRotation;
            return;
        }
        else if (this.gameObject.transform.localRotation == spManager.normalRightRotation)
        {
            this.gameObject.transform.localRotation = spManager.inverseLeftRotation;
            this.gameObject.transform.position = new Vector3(transform.position.x, 0f, transform.position.z + 2f);
            return;

        }
        else if (this.gameObject.transform.localRotation == spManager.inverseLeftRotation)
        {
            this.gameObject.transform.localRotation = spManager.inverseRotation;
            return;

        }
        else if (this.gameObject.transform.localRotation == spManager.inverseRotation)
        {
            this.gameObject.transform.localRotation = spManager.inverseRightRotation;
            return;

        }
        else if (this.gameObject.transform.localRotation == spManager.inverseRightRotation)
        {
            this.gameObject.transform.localRotation = spManager.normalLeftRotation;
            this.gameObject.transform.position = new Vector3(transform.position.x, 0f, transform.position.z - 2f);
            return;

        }
        else if (this.gameObject.transform.localRotation == spManager.normalLeftRotation)
        {
            this.gameObject.transform.localRotation = spManager.normalRotation;
            return;

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (spManager == null || spManager.GetBallHolderTeamPlayer(true) == spManager.empty) return;
        if (spManager.GetBallHolderTeamPlayer(true) == this.gameObject) return;
        if (!spManager.hasPlayer1TeamBall) { spBallController.isReceiverCatchSuccess = true; return; }
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
        SoundManager.instance.PlaySE(2);
        animator.SetBool("isHit", true);
    }

    private void StandUpChara()
    {
        animator.SetBool("isHit", false);
        spBallController.ChangeBallOwnerToEnemy();
    }
    public void EnableDisplayCaution(bool isActivate)
    {
        if (isActivate) SoundManager.instance.PlayOnlyThisSE(0);
        caution.gameObject.SetActive(isActivate);
    }
}
