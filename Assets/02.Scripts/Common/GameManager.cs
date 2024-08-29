using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get { return instance; }
        set {
            if (instance == null)
                instance = value;
            else if (instance != value)
                Destroy(value);
            }
    }
    public Text txtConnect;
    public Text txtLogMsg;
    public Text txtScore;

    private string LogMessage;

    private int Score = 0;
    private int curScore = 0;

    public bool isGameOver = false;
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        CreateTank();
        PhotonNetwork.IsMessageQueueRunning = true; //���ε尡 ������ ���� Ŭ���� �����κ��� �޼����� �޵��� ����. ��, ��Ʈ��ũ ����ȭ
    }
    void Start()
    {
        string msg = "\n<color=#00ff00>[" + PhotonNetwork.NickName + "]Connected</color>"; //���� ������ �� �ڵ� �ν� �ȵ�.
        photonView.RPC("LogMSG", RpcTarget.AllBuffered, msg); //���߿� ���� ����� ������ ���� ����� �α׸� ����.(��� ���´� ó�� ���� -> 1 �ι�° ������ -> 12 ����° ������ -> 123, ������ ���� ��� ���)
    }

    void CreateTank() //��ũ ���� ���ø� ���� ��� ����߾���.
    {
        float pos = Random.Range(-50f, 50f);
        PhotonNetwork.Instantiate("Tank", new Vector3(pos, 5f, pos), Quaternion.identity); //"Tank"�̰��� ���ҽ� ���Ͽ� �ִ� ��ũ �������̴�.
    }

    [PunRPC]
    public void ApplyPlayerCountUpdate() //������� ������ �ڵ� (�� Ŭ���̾�Ʈ�� ���� ������ ������Ʈ�ϴ� ��� ���)
    {
        Room currentRoom = PhotonNetwork.CurrentRoom;
        txtConnect.text = $"{currentRoom.PlayerCount.ToString()}/{currentRoom.MaxPlayers.ToString()}";
    }

    [PunRPC]
    void GetConnectPlayerCount() //���� �濡 �÷��̾� ���� ��Ÿ���� �ڵ�
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //Room currentRoom = PhotonNetwork.CurrentRoom; //���� Ŭ���̾�Ʈ�� ������ ���� ��
            //txtConnect.text = $"{currentRoom.PlayerCount.ToString()}/{currentRoom.MaxPlayers.ToString()}"; //ui�� ���� Ŭ���̾�Ʈ ��/�ִ� ���Ӱ����� Ŭ�󸮾�Ʈ �� �� �����Ѵ�.

            #region ���� ����� ���� �ڵ�
            //curPlayer = $"{currentRoom.PlayerCount.ToString()}/{currentRoom.MaxPlayers.ToString()}";
            //txtConnect.text = curPlayer; //ui�� ���� Ŭ���̾�Ʈ ��/�ִ� ���Ӱ����� Ŭ�󸮾�Ʈ �� �� �����Ѵ�.
            //(������, �÷��̾ ������ string�� ������ ���۵Ǵ� �ӵ� ���� RPC�� �Լ��� �ҷ����� �ӵ��� ����)
            #endregion
            photonView.RPC("ApplyPlayerCountUpdate", RpcTarget.All);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) //Ŭ���̾�Ʈ�� �濡 ������ ȣ��Ǵ� �Լ�.
    {
        GetConnectPlayerCount();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) //Ŭ���̾�Ʈ�� �濡�� ������ ȣ��Ǵ� �Լ�.
    {
        GetConnectPlayerCount();
    }

    public void OnClickExitRoom() //Exit��ư�� ������ �� ȣ��Ǵ� �Լ�
    {
        string msg = "\n<color=#ff0000>[" + PhotonNetwork.NickName + "]Disconnected</color>";
        photonView.RPC("LogMSG", RpcTarget.All, msg);
        PhotonNetwork.LeaveRoom(); //���� ���� Ŭ���̾�Ʈ�� �ִ� �뿡�� ������.
    }

    public override void OnLeftRoom() //�뿡�� ������ ����Ǿ��� �� ȣ�� �Ǵ� �Լ�
    {
        SceneManager.LoadScene("Lobby Scene"); //�κ���� ȣ���Ѵ�.
    }

    [PunRPC]
    void LogMSG(string msg) //������ �α� �޼����� �����ϴ� ���� �ƴϱ� ������ ���� �α׸޼����� �ߴ°��� �ٸ���.
    {
        txtLogMsg.text = txtLogMsg.text + msg; //�޼����� ��� �����ؼ� �ִ´�.
    }

    public void AddScore()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Score++;
            ScoreManager();
        }
    }

    [PunRPC]
    private void ScoreManager()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            txtScore.text = $"Score : {Score.ToString("0000")}";
            photonView.RPC("ScoreManager", RpcTarget.Others);
        }
        else
        {
            txtScore.text = $"Score : {curScore.ToString("0000")}";
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(Score);
        }
        else
        {
            curScore = (int)stream.ReceiveNext();
        }
    }
}
