using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ConnectingPlayer : MonoBehaviourPunCallbacks
{
    private const int MaxPlayersPerRoom = 2;

    private void Start()
    {
        CreateOrJoinRoom();
    }

    void CreateOrJoinRoom()
    {
        EnterRoomParams roomParams = new EnterRoomParams();
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = MaxPlayersPerRoom;
        roomParams.RoomOptions = roomOptions;
        PhotonNetwork.NetworkingClient.OpJoinRandomOrCreateRoom(null, roomParams);
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == MaxPlayersPerRoom)
        {
            // 部屋が満員になったらゲームシーンに移動
            if (!PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("ChangeScene", RpcTarget.AllBuffered);
            }
        }
        else
        {
            Debug.Log("Waiting for more players...");
        }
    }


    [PunRPC]
    void ChangeScene()
    {
        PhotonNetwork.LoadLevel("Two Players Room");
    }

    public void ReloadScene()
    {
        PhotonNetwork.LoadLevel("Waiting Player");
    }
}

