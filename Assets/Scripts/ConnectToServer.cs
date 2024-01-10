using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
public class ConnectToServer : MonoBehaviourPunCallbacks
{
    void Start()
    {
        // Starting the procces of conecting server!
        PhotonNetwork.ConnectUsingSettings();
    }

    // Once we are successfully connected to just go ahead load up our maim menu
    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
