﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
public class SpinnerTopGameManager : MonoBehaviourPunCallbacks
{

    [Header("UI")]

    public GameObject uI_InformPanelGameObject;
    public TextMeshProUGUI uI_InformText;
    public GameObject searchForGamesButtonGameObject;
    // Start is called before the first frame update
    void Start()
    {
        uI_InformPanelGameObject.SetActive(true);
        uI_InformText.text = "Search for Games to Battle";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #region UI Callback Methods
    public void JoinRandomRoom()
    {
        uI_InformText.text = "Searching for available rooms ...";
        PhotonNetwork.JoinRandomRoom();
        searchForGamesButtonGameObject.SetActive(false);
    }

    public void OnQuitMatchButtonClicked()
    {
        if(PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            SceneLoader.Instance.LoadScene("Scene_Lobby");
        }
       
    }
    #endregion

    #region PHOTON Callback Methods
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        uI_InformText.text = message;
        CreateAndJoinRoom();
    }
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            uI_InformText.text = "Joined to " + PhotonNetwork.CurrentRoom.Name + " Waiting for other player";
        }
        else
        {
            uI_InformText.text = "Joined to " + PhotonNetwork.CurrentRoom.Name ;
            StartCoroutine(DeactivateAfterSeconds(uI_InformPanelGameObject, 2.0f));
        }
        //base.OnJoinedRoom();
        Debug.Log( "Joined to" + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + "joined to " + PhotonNetwork.CurrentRoom.Name+" Player Count "+ PhotonNetwork.CurrentRoom.PlayerCount);
        uI_InformText.text = newPlayer.NickName + "joined to " + PhotonNetwork.CurrentRoom.Name + " Player Count " + PhotonNetwork.CurrentRoom.PlayerCount;

        StartCoroutine(DeactivateAfterSeconds(uI_InformPanelGameObject, 2.0f));
    }

    public override void OnLeftRoom()
    {
        SceneLoader.Instance.LoadScene("Scene_Lobby");
    }
    #endregion

    #region PRIVATE Methoods
    void CreateAndJoinRoom()
    {
        string randomRoomName ="Room"+ Random.Range(0,1000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
    }

    IEnumerator DeactivateAfterSeconds(GameObject _gameObject, float _seconds)
    {
        yield return new WaitForSeconds(_seconds);
        _gameObject.SetActive(false);
    }
    #endregion
}
