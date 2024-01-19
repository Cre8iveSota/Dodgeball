using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] private GameObject tmpBallPosition;
    public bool isMovingBall;
    public float ballSpeed = 0.01f;
    public Vector3 ballDestination = Vector3.zero;
    GameObject[] players;
    GameObject gammeManagerObj;
    GameManager gameManager;
    public Rigidbody rb;
    public Vector3 ballForce;
    public bool IsReceiverCatchSuccess;
    public GameObject tmpBallHolder;
    public GameObject ThrowMan;
    GameObject Reciever;
    PhotonView photonView;
    public bool isBallReady;

    Vector3 defeinedSpeed;
    MainPlayerController mainCharaController, mainChara2Controller;
    SubPlayerController subCharaController, subChara2Controller;
    GroundController groundController;
    public bool enableCatchBall, enableBallInterupt;// 相手のボーるをキャッチしたことを知らせるフラグと、パスをインタラプトしたときに知らせるフラグ

    int cnt = 0;
    /*
    Dotweenでballを動かす
    ballのポジションの動悸するシステム
    */

    // Start is called before the first frame update
    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        gammeManagerObj = GameObject.FindGameObjectWithTag("GameManager");
        gameManager = gammeManagerObj.GetComponent<GameManager>();
        mainCharaController = gameManager.mainCharaInstance.GetComponent<MainPlayerController>();
        subCharaController = gameManager.subCharaInstance.GetComponent<SubPlayerController>();
        rb = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.z > 80f)
        {
            //subchara
            Reciever = gameManager.subCharaInstance;
            FixBallPosition();
        }
        else if (transform.position.z < -80f)
        {
            //aub2
            Reciever = gameManager.subChara2Instance;
            FixBallPosition();
        }

        bool isExecuted = false;
        if (gameManager.mainChara2Instance != null && gameManager.subChara2Instance != null && !isExecuted)
        {
            mainChara2Controller = gameManager.mainChara2Instance.GetComponent<MainPlayerController>();
            subChara2Controller = gameManager.subChara2Instance.GetComponent<SubPlayerController>();
            isExecuted = true;
        }

        // sita
        // if (this.gameObject.GetPhotonView() != null && PhotonNetwork.IsMasterClient) photonView.RPC("SyncronizeBallPosition", RpcTarget.All, this.gameObject.GetPhotonView().ViewID);

        SetDestination(ballDestination);
        // if (IsReceiverCatchSuccess && photonView.IsMine) //エラー出た
        // if (IsReceiverCatchSuccess && PhotonNetwork.IsMasterClient)//狩り
        if (enableBallInterupt)
        {
            if (ThrowMan == gameManager.mainCharaInstance || ThrowMan == gameManager.subCharaInstance) Reciever = gameManager.mainChara2Instance;
            if (ThrowMan == gameManager.mainChara2Instance || ThrowMan == gameManager.subChara2Instance) Reciever = gameManager.mainCharaInstance;
            Debug.Log("ThrowMan" + ThrowMan);
            Debug.Log("changed reciever: " + Reciever);
            FixBallPosition();
            enableBallInterupt = false;
        }
        else if (IsReceiverCatchSuccess)//狩り
        {
            FixBallPosition();
        }

        if (cnt % 6 == 0) photonView.RPC("SyncronizeBallPosition", RpcTarget.All);
        cnt++;
    }

    private void FixBallPosition()
    {
        isMovingBall = false;
        if (mainCharaController != null && mainCharaController.iAmThrowing == true) mainCharaController.iAmThrowing = false;
        if (subCharaController != null && subCharaController.iAmThrowing == true) subCharaController.iAmThrowing = false;
        if (mainChara2Controller != null && mainChara2Controller.iAmThrowing == true) mainChara2Controller.iAmThrowing = false;
        if (subChara2Controller != null && subChara2Controller.iAmThrowing == true) subChara2Controller.iAmThrowing = false;
        photonView.RPC("DestroyTmpBallHolder", RpcTarget.All, Reciever.GetPhotonView().ViewID);
        IsReceiverCatchSuccess = false;
        enableCatchBall = false;
        photonView.RPC("SyncronizeBallPosition", RpcTarget.All);
        ThrowMan = null;
    }

    public void SetDestination(Vector3 destination)
    {
        if (isMovingBall)
        {
            transform.position += defeinedSpeed;
        }
    }

    public IEnumerator NormalPass(GameObject passerGameObject, GameObject recieverGameObject)
    {
        cnt = 0;
        ThrowMan = passerGameObject;

        GameObject ground = GameObject.FindGameObjectWithTag("Ground");
        // if (ground != null)
        if (ground != null)
        {
            groundController = ground.GetComponent<GroundController>();
            Debug.Log("groundController.defenciblePosition ball : " + groundController.defenciblePosition);
            Debug.Log("groundController.defenciblePosition bal : " + groundController.ballposition);

            if ((groundController.defenciblePosition.transform.position.z == -5f || groundController.defenciblePosition.transform.position.z == 15f)
            && groundController.defenciblePosition == groundController.ballposition)
            {
                Debug.Log("come on");
                // メインプレイヤーの攻撃時、メインのプレイヤーのzの位置の平行ラインにdefencive pointがあり かつ defensive pint とボールの位置が被っている
                enableCatchBall = true;
            }
            else if ((groundController.defenciblePosition.transform.position.z == 5f || groundController.defenciblePosition.transform.position.z == -15f)
            && groundController.defenciblePosition == groundController.ballposition)
            {
                Debug.Log("come on2");
                // メイン2プレイヤーの攻撃時、メインのプレイヤーのzの位置の平行ラインにdefencive pointがあり かつ defensive pint とボールの位置が被っている
                enableCatchBall = true;
            }
        }

        Reciever = recieverGameObject;
        ballDestination = recieverGameObject.transform.position;
        isMovingBall = true;
        tmpBallHolder = Instantiate(tmpBallPosition, passerGameObject.transform.position, Quaternion.identity);

        photonView.RPC("CreateTmpBallHolder", RpcTarget.All, tmpBallHolder.GetPhotonView().ViewID);

        transform.localPosition = new Vector3(0f, 1f, 0.4f);
        defeinedSpeed = new Vector3((ballDestination.x - transform.position.x) * ballSpeed * Time.deltaTime, 0f, (ballDestination.z - transform.position.z) * ballSpeed * Time.deltaTime);
        yield return new WaitForSeconds(1f);
        Debug.Log("presssed space");
    }

    [PunRPC]
    private void DestroyTmpBallHolder(int viewID)
    {
        PhotonView recieverView = PhotonView.Find(viewID);
        if (recieverView != null)
        {
            GameObject tmpReciever = recieverView.gameObject;
            transform.SetParent(tmpReciever.transform, false);
            transform.localPosition = new Vector3(0f, 1f, 0.4f);

            Destroy(tmpBallHolder);
        }
    }


    [PunRPC]
    private void CreateTmpBallHolder(int viewID)
    {
        PhotonView tmpBallHolder = PhotonView.Find(viewID);
        if (tmpBallHolder != null)
        {
            transform.SetParent(tmpBallHolder.gameObject.transform, false);
            // transform.localPosition = new Vector3(0f, 1f, 0.4f);
            Debug.Log("gameManager.GetBallHolderPlayer(true)" + gameManager.GetBallHolderTeamPlayer(true));
            if (tmpBallHolder.transform.position.z == -6 && tmpBallHolder.transform.position.z == -16) transform.localPosition = new Vector3(0f, 1f, 0.4f);
            if (tmpBallHolder.transform.position.z == 16 && tmpBallHolder.transform.position.z == 6) transform.localPosition = new Vector3(0f, 1f, -0.4f);
        }
    }


    [PunRPC]
    private void SyncronizeBallPosition()
    {
        PhotonView ballPhotonView = PhotonView.Find(this.gameObject.GetPhotonView().ViewID);
        if (ballPhotonView != null)
        {
            gameManager.realBallInstance = ballPhotonView.gameObject;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Ballllll");
    }

    public bool IsSomeoneThrowing()
    {
        if (tmpBallHolder == null)
        {
            return false;
        }
        Debug.Log("Player throw ball; the player name is" + ThrowMan);
        if (tmpBallHolder != null)
        {
            return true;
        }
        return false;
    }
}
