using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Login UI")]
    public InputField playerNameInputField;
    public GameObject uI_LoginGameobject;
    [Header("Lobby UI")]
    public GameObject uI_LobbyGameobject;
    public GameObject uI_3DGameobject;

    [Header("Connection Status UI")]
    public GameObject uI_ConnectionStatusGameObject;
    public Text connectionStatusText;
    public bool showConnectionStatus = false;
    #region UNITY METHODS
    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsConnected)
        {
            Debug.Log("connect already");
            uI_LoginGameobject.SetActive(false);
            uI_LobbyGameobject.SetActive(true);
            uI_3DGameobject.SetActive(true);
            uI_ConnectionStatusGameObject.SetActive(false);
            
        }
        else
        {
            uI_LobbyGameobject.SetActive(false);
            uI_3DGameobject.SetActive(false);
            uI_ConnectionStatusGameObject.SetActive(false);
            uI_LoginGameobject.SetActive(true);
        }
    }

    // Update is called once per frame

    void Update()
    {
        if (showConnectionStatus)
        {
            connectionStatusText.text = "Connection Status: " + PhotonNetwork.NetworkClientState;
        }
    }
    #endregion
    #region
    public void onEnterGameButtonClicked()
    {

        string playerName = playerNameInputField.text;
        if(!string.IsNullOrEmpty(playerName))
        {
            uI_LobbyGameobject.SetActive(false);
            uI_3DGameobject.SetActive(false);
            uI_LoginGameobject.SetActive(false);
            showConnectionStatus = true;
            uI_ConnectionStatusGameObject.SetActive(true);
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
            }
        }
        else
        {
            Debug.Log("Player Name is invalid or empty!");
        }
    }

    public void OnQuickMatchButtonCLicked()
    {
        SceneManager.LoadScene("Scene_Loading");
        SceneLoader.Instance.LoadScene("Scene_PlayerSelection");
    }
    #endregion

    #region PHOTON Callback Methods
    public override void OnConnected()
    {
        Debug.Log("We connected to the internet");
    }
    public override void OnConnectedToMaster()
    {
        // base.OnConnectedToMaster();
        Debug.Log(PhotonNetwork.LocalPlayer.NickName+ "We connected to the master");
        uI_LobbyGameobject.SetActive(true);
        uI_3DGameobject.SetActive(true);
        uI_LoginGameobject.SetActive(false);
        uI_ConnectionStatusGameObject.SetActive(false);
    }

  
    #endregion
}
