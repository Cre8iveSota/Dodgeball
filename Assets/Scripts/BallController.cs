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
    GameObject ThrowMan;
    GameObject Reciever;
    PhotonView photonView;
    public bool isBallReady;


    Vector3 defeinedSpeed;



    // Start is called before the first frame update
    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        gammeManagerObj = GameObject.FindGameObjectWithTag("GameManager");
        gameManager = gammeManagerObj.GetComponent<GameManager>();
        rb = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        SetDestination(ballDestination);
        // if (IsReceiverCatchSuccess && photonView.IsMine) //エラー出た
        if (IsReceiverCatchSuccess && PhotonNetwork.IsMasterClient)//狩り
        {
            isMovingBall = false;
            photonView.RPC("DestroyTmpBallHolder", RpcTarget.All, Reciever.GetPhotonView().ViewID);
            Destroy(tmpBallHolder);
            IsReceiverCatchSuccess = false;
        }
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (gameManager.CheckHaveBallAsChildren(passerGameObject))
            {
                ThrowMan = passerGameObject;
                Reciever = recieverGameObject;
                ballDestination = recieverGameObject.transform.position;
                isMovingBall = true;
                tmpBallHolder = Instantiate(tmpBallPosition, passerGameObject.transform.position, Quaternion.identity);
                transform.SetParent(tmpBallHolder.transform, false);
                if (ThrowMan == gameManager.mainCharaInstance) transform.localPosition = new Vector3(0f, 1f, 0.4f);
                if (ThrowMan == gameManager.subCharaInstance) transform.localPosition = new Vector3(0f, 1f, -0.4f);
                defeinedSpeed = new Vector3((ballDestination.x - transform.position.x) * ballSpeed, 0f, (ballDestination.z - transform.position.z) * ballSpeed);
                yield return new WaitForSeconds(1f);
            }
            Debug.Log("presssed space");
            yield return new WaitForSeconds(1f);
        }
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
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Ballllll");
    }

    public bool IsPlayerThrowing()
    {
        if (tmpBallHolder == null)
        {
            return false;
        }
        Debug.Log("Player throw ball; the player name is" + ThrowMan);
        if (tmpBallHolder.transform.position.z == -6 || tmpBallHolder.transform.position.z == 16)
        {
            return true;
        }
        return false;
    }
}
