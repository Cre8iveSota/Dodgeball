using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SubPlayerController : MonoBehaviour
{
    GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveBallHolderTeamSubPlayer();
    }

    void MoveBallHolderTeamSubPlayer()
    {
        Debug.Log("gameManager.mainChara" + gameManager.mainChara);
        Debug.Log("gameManager.mainChara name" + gameManager.mainChara.transform.name);
        Debug.Log("gameManager.mainChara position" + gameManager.mainChara.transform.position);


        if (gameManager.mainChara != null && gameManager.mainChara.transform.position.x <= 10 && gameManager.mainChara.transform.position.x >= -10)
        {
            Debug.Log("So farso good" + gameManager.mainChara.transform.position.x);
            transform.position = new Vector3(gameManager.mainChara.transform.position.x, this.transform.position.y, this.transform.position.z);
            Debug.Log("transform.position" + transform.position);
            Debug.Log("transform.position" + new Vector3(gameManager.mainChara.transform.position.x, this.transform.position.y, this.transform.position.z));
        }
    }
}
