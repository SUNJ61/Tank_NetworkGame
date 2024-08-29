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
        PhotonNetwork.IsMessageQueueRunning = true; //씬로드가 끝나면 포톤 클라우드 서버로부터 메세지를 받도록 설정. 즉, 네트워크 동기화
    }
    void Start()
    {
        string msg = "\n<color=#00ff00>[" + PhotonNetwork.NickName + "]Connected</color>"; //띄어쓰기 넣으면 색 코드 인식 안됨.
        photonView.RPC("LogMSG", RpcTarget.AllBuffered, msg); //나중에 들어온 사람도 이전에 들어온 사람의 로그를 본다.(출력 형태는 처음 들어가면 -> 1 두번째 들어오면 -> 12 세번째 들어오면 -> 123, 이전의 값도 계속 출력)
    }

    void CreateTank() //탱크 생성 예시를 위해 잠시 사용했었다.
    {
        float pos = Random.Range(-50f, 50f);
        PhotonNetwork.Instantiate("Tank", new Vector3(pos, 5f, pos), Quaternion.identity); //"Tank"이것은 리소스 파일에 있는 탱크 프리팹이다.
    }

    [PunRPC]
    public void ApplyPlayerCountUpdate() //강사님이 수정한 코드 (각 클라이언트가 각자 데이터 업데이트하는 방식 사용)
    {
        Room currentRoom = PhotonNetwork.CurrentRoom;
        txtConnect.text = $"{currentRoom.PlayerCount.ToString()}/{currentRoom.MaxPlayers.ToString()}";
    }

    [PunRPC]
    void GetConnectPlayerCount() //현재 방에 플레이어 수를 나타내는 코드
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //Room currentRoom = PhotonNetwork.CurrentRoom; //로컬 클라이언트가 접속한 현재 룸
            //txtConnect.text = $"{currentRoom.PlayerCount.ToString()}/{currentRoom.MaxPlayers.ToString()}"; //ui를 접속 클라이언트 수/최대 접속가능한 클라리언트 수 로 변경한다.

            #region 내가 사용한 수정 코드
            //curPlayer = $"{currentRoom.PlayerCount.ToString()}/{currentRoom.MaxPlayers.ToString()}";
            //txtConnect.text = curPlayer; //ui를 접속 클라이언트 수/최대 접속가능한 클라리언트 수 로 변경한다.
            //(문제점, 플레이어가 들어오고 string이 서버로 전송되는 속도 보다 RPC로 함수를 불러오는 속도가 빠름)
            #endregion
            photonView.RPC("ApplyPlayerCountUpdate", RpcTarget.All);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) //클라이언트가 방에 들어오면 호출되는 함수.
    {
        GetConnectPlayerCount();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) //클라이언트가 방에서 나가면 호출되는 함수.
    {
        GetConnectPlayerCount();
    }

    public void OnClickExitRoom() //Exit버튼을 눌렀을 때 호출되는 함수
    {
        string msg = "\n<color=#ff0000>[" + PhotonNetwork.NickName + "]Disconnected</color>";
        photonView.RPC("LogMSG", RpcTarget.All, msg);
        PhotonNetwork.LeaveRoom(); //현재 로컬 클라이언트가 있는 룸에서 나간다.
    }

    public override void OnLeftRoom() //룸에서 접속이 종료되었을 때 호출 되는 함수
    {
        SceneManager.LoadScene("Lobby Scene"); //로비씬을 호출한다.
    }

    [PunRPC]
    void LogMSG(string msg) //서버에 로그 메세지를 저장하는 것이 아니기 때문에 각자 로그메세지가 뜨는것이 다르다.
    {
        txtLogMsg.text = txtLogMsg.text + msg; //메세지를 계속 누적해서 넣는다.
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
