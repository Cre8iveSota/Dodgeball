using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpEnemyController : MonoBehaviour
{
    SinglePlayManager singlePlayManager;
    SPballController sPballController;
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
    // Start is called before the first frame update
    void Start()
    {
        singlePlayManager = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<SinglePlayManager>();
        if (singlePlayManager.realBallInstance != null) sPballController = singlePlayManager.realBallInstance.GetComponent<SPballController>();
        animator = GetComponent<Animator>();
        GameObject ground = GameObject.FindGameObjectWithTag("Ground");
        if (ground != null) spGroundController = ground.GetComponent<SpGroundController>();
        spSubEnemyController = singlePlayManager.subEnemyInstance.GetComponent<SpSubEnemyController>();
    }

    // Update is called once per frame
    void Update()
    {
        Time.fixedDeltaTime = singlePlayManager.actionSpeedEnemy;

        enemyPoistionX = singlePlayManager.enemyInstance.transform.position.x;
        enemySubPoistionX = singlePlayManager.subEnemyInstance.transform.position.x;
        playerSubPositionX = singlePlayManager.subCharaInstance.transform.position.x;
        playerPositionX = singlePlayManager.mainCharaInstance.transform.position.x;
        ballHolderPositionX = singlePlayManager.GetBallHolderTeamPlayer(true).transform.position.x;
        fellowOfBallHolderPositionX = singlePlayManager.GetBallHolderTeamPlayer(false).transform.position.x;

        // 自分がボールを持っていたら相手の方向を向く
        if (singlePlayManager.GetBallHolderTeamPlayer(true) == singlePlayManager.enemyInstance)
        {
            this.gameObject.transform.rotation = singlePlayManager.inverseRotation;
            this.gameObject.transform.position = new Vector3(transform.position.x, 0, 6f); //new Vector3(transform.position.x, 0, 6f);
        }
    }

    void FixedUpdate()
    {
        int randomNum = Random.Range(1, 36);
        // Enemy Team has ball
        if (!singlePlayManager.hasPlayer1TeamBall)
        {
            // Enemy main has ball
            if (singlePlayManager.CheckHaveBallAsChildren(singlePlayManager.enemyInstance))
            {
                // if Enemy main is still throwing ball, you cannot do any action
                if (iAmThrowing) return;
                // Sometimes enemy main thow ball randomely.
                if (randomNum % 9 == 0)
                {
                    pseudoPressSpase = true;
                    EnemyThrow();
                }
                // if player in the midlle position between enemy and enemySub also player do not prepare for defence, enemy throw ball
                else if (IsPositionBTWballHolderTeamFromTargetView(singlePlayManager.mainCharaInstance) && spGroundController.ballposition != spGroundController.defenciblePosition)
                {
                    pseudoPressSpase = true;
                    EnemyThrow();
                }
                // if player in the midlle position between enemy and enemySub also player prepared for defence, enemy play at random
                else if (IsPositionBTWballHolderTeamFromTargetView(singlePlayManager.mainCharaInstance) && spGroundController.ballposition == spGroundController.defenciblePosition)
                {
                    if (randomNum % 4 == 0) MoveEnemyToLeft();
                    if (randomNum % 4 == 1) MoveEnemyToRight();
                    if (randomNum % 4 == 2) spSubEnemyController.MoveEnemySubToRight();
                    if (randomNum % 4 == 3) spSubEnemyController.MoveEnemySubToLeft();
                }
                // if enemy position equals player position also it has difference between enemySub and enemy position, make move enemy or enemySub 
                else if (enemyPoistionX == playerPositionX && enemyPoistionX < enemySubPoistionX)
                {
                    if (randomNum % 2 == 0) MoveEnemyToRight();
                    if (randomNum % 2 == 1) spSubEnemyController.MoveEnemySubToRight();
                    return;
                }
                else if (enemyPoistionX == playerPositionX && enemySubPoistionX < enemyPoistionX)
                {
                    MoveEnemyToLeft();
                    spSubEnemyController.MoveEnemySubToLeft();
                    return;
                }
                // if enemy position not equal player position, make move enemy or enemySub 
                else if (enemyPoistionX < playerPositionX)
                {
                    if (randomNum % 2 == 0) MoveEnemyToLeft();
                    if (randomNum % 2 == 1) spSubEnemyController.MoveEnemySubToLeft();
                    return;
                }
                else if (playerPositionX < enemyPoistionX)
                {
                    if (randomNum % 2 == 0) MoveEnemyToRight();
                    if (randomNum % 2 == 1) spSubEnemyController.MoveEnemySubToRight();
                    return;
                }
            }
            // if enemySub has ball
            else if (singlePlayManager.CheckHaveBallAsChildren(singlePlayManager.subEnemyInstance))
            {
                // sometimes enemySub throw ball at random
                if (randomNum % 9 == 0) { spSubEnemyController.EnemySubThrow(); }
                // if player is in the middle of enemy and enemySub also player do not prepare for defence, throw ball
                else if (IsPositionBTWballHolderTeamFromTargetView(singlePlayManager.mainCharaInstance) && spGroundController.ballposition != spGroundController.defenciblePosition)
                {
                    spSubEnemyController.EnemySubThrow();
                }
                // if player is in the middle of enemy and enemySub also player prepared for defence, move at random
                else if (IsPositionBTWballHolderTeamFromTargetView(singlePlayManager.mainCharaInstance) && spGroundController.ballposition == spGroundController.defenciblePosition)
                {
                    if (randomNum % 4 == 0) MoveEnemyToLeft();
                    if (randomNum % 4 == 1) MoveEnemyToRight();
                    if (randomNum % 4 == 2) spSubEnemyController.MoveEnemySubToRight();
                    if (randomNum % 4 == 3) spSubEnemyController.MoveEnemySubToLeft();
                }
                // if enemySub position is as same as player, however enemy position is in left side from enemySub, make move enemy or enemySub
                else if (enemySubPoistionX == playerPositionX && enemyPoistionX < enemySubPoistionX)
                {
                    if (randomNum % 2 == 0) MoveEnemyToLeft();
                    if (randomNum % 2 == 1) spSubEnemyController.MoveEnemySubToLeft();
                    return;
                }
                else if (enemySubPoistionX == playerPositionX && enemySubPoistionX < enemyPoistionX)
                {
                    if (randomNum % 2 == 0) MoveEnemyToRight();
                    if (randomNum % 2 == 1) spSubEnemyController.MoveEnemySubToRight();
                    return;
                }
                // if enemySub position is not equal player, make move enemy or enemySub
                else if (enemySubPoistionX < playerPositionX)
                {
                    Debug.Log("hew");
                    if (randomNum % 2 == 0) MoveEnemyToLeft();
                    if (randomNum % 2 == 1) spSubEnemyController.MoveEnemySubToLeft();
                    return;
                }
                else if (playerPositionX < enemySubPoistionX)
                {
                    Debug.Log("hew2");
                    if (randomNum % 2 == 0) MoveEnemyToRight();
                    if (randomNum % 2 == 1) spSubEnemyController.MoveEnemySubToRight();
                    return;
                }
            }
        }
        // if enemy team do not have a ball
        if (singlePlayManager.hasPlayer1TeamBall)
        {
            DodgeOrTakeUpDefensiveRotation(randomNum);
        }
    }

    private void DodgeOrTakeUpDefensiveRotation(int randomNum)
    {
        if (spGroundController.ballposition == spGroundController.defenciblePosition) return;
        if (CanMoveRightFromTargetView(this.gameObject) && CanMoveLeftFromTargetView(this.gameObject))
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

    private bool IsPositionBTWballHolderTeamFromTargetView(GameObject targetInstance)
    {
        float targetPositionX = targetInstance.transform.position.x;
        Debug.Log("targetPositionX" + targetPositionX);
        Debug.Log("ballHolderPositionX" + ballHolderPositionX);
        Debug.Log("fellowOfBallHolderPositionX" + fellowOfBallHolderPositionX);

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
        if (targetInstance == singlePlayManager.enemyInstance)
        {
            if (enemyPoistionX > -10) { return true; }
            return false;
        }
        else if (targetInstance == singlePlayManager.subEnemyInstance)
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
        if (targetInstance == singlePlayManager.enemyInstance)
        {
            if (enemyPoistionX < 10) { return true; }
            return false;
        }
        else if (targetInstance == singlePlayManager.subEnemyInstance)
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
            if (singlePlayManager.CheckHaveBallAsChildren(this.gameObject))
            {
                iAmThrowing = true;
                StartCoroutine(sPballController.NormalPass(singlePlayManager.enemyInstance, singlePlayManager.subEnemyInstance));
            }
        }
    }

    private void MoveEnemyToRight()
    {
        if (pseudoPressLeft && transform.position.x > -10)
        {
            transform.position = new Vector3(transform.position.x - 10, transform.position.y, transform.position.z);
            pseudoPressLeft = false;
        }
    }

    private void MoveEnemyToLeft()
    {
        if (pseudoPressRight && transform.position.x < 10)
        {
            transform.position = new Vector3(transform.position.x + 10, transform.position.y, transform.position.z);
            pseudoPressRight = false;
        }
    }

    private void TurnEnemyToBallHolder(int randomNum)
    {
        if (!singlePlayManager.hasPlayer1TeamBall) return;
        if (singlePlayManager.GetBallHolderTeamPlayer(true) == singlePlayManager.mainCharaInstance)
        {
            if (playerPositionX < enemyPoistionX) { AntiClockWiseTurn(); return; }
            if (enemyPoistionX < playerPositionX) { ClockWiseTurn(); return; }
            if (enemyPoistionX == playerPositionX && randomNum % 2 == 0) { ClockWiseTurn(); return; }
            if (enemyPoistionX == playerPositionX && randomNum % 2 == 1) { AntiClockWiseTurn(); return; }

        }
        else if (singlePlayManager.GetBallHolderTeamPlayer(true) == singlePlayManager.subCharaInstance)
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
        if (!pseudoPressSpase) return;
        pseudoPressSpase = false;
        if (this.gameObject.transform.localRotation == singlePlayManager.normalRotation)
        {
            this.gameObject.transform.localRotation = singlePlayManager.normalRightRotation;
            return;
        }
        else if (this.gameObject.transform.localRotation == singlePlayManager.normalRightRotation)
        {
            this.gameObject.transform.localRotation = singlePlayManager.inverseLeftRotation;
            this.gameObject.transform.position = new Vector3(transform.position.x, 0f, transform.position.z + 2f);
            return;

        }
        else if (this.gameObject.transform.localRotation == singlePlayManager.inverseLeftRotation)
        {
            this.gameObject.transform.localRotation = singlePlayManager.inverseRotation;
            return;

        }
        else if (this.gameObject.transform.localRotation == singlePlayManager.inverseRotation)
        {
            this.gameObject.transform.localRotation = singlePlayManager.inverseRightRotation;
            return;

        }
        else if (this.gameObject.transform.localRotation == singlePlayManager.inverseRightRotation)
        {
            this.gameObject.transform.localRotation = singlePlayManager.normalLeftRotation;
            this.gameObject.transform.position = new Vector3(transform.position.x, 0f, transform.position.z - 2f);
            return;

        }
        else if (this.gameObject.transform.localRotation == singlePlayManager.normalLeftRotation)
        {
            this.gameObject.transform.localRotation = singlePlayManager.normalRotation;
            return;

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (singlePlayManager == null || singlePlayManager.GetBallHolderTeamPlayer(true) == singlePlayManager.empty) return;
        if (singlePlayManager.GetBallHolderTeamPlayer(true) == this.gameObject) return;
        if (!singlePlayManager.hasPlayer1TeamBall) { sPballController.isReceiverCatchSuccess = true; return; }
        if (sPballController.enableCatchBall)
        {
            sPballController.enableBallInterupt = true;
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
        sPballController.ChangeBallOwnerToEnemy();
    }
}
