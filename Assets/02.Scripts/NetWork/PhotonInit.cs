using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;//Pun 네트워크 관련 각종 라이브러리가 있음.
using UnityEngine.UI;
public class PhotonInit : MonoBehaviourPunCallbacks //로비를 구성할 때 상속
{
    public InputField userID;
    public InputField RoomID;
    public GameObject roomItem;
    public GameObject scrollContents;

    public List<GameObject> obj;

    public string Version = "V1.0";
    void Awake()
    {
        if (!PhotonNetwork.IsConnected) //접속되지 않았으면 다음 코드를 실행, 이미 연결된 상태라면 실행 x (Exit버튼을 통해 나왔을 때 오류 발생 방지) 
        {
            PhotonNetwork.GameVersion = Version; //게임 버전을 결정하는 프로퍼티 (프로퍼티 : 클래스 안에 변수를 접근할 때 사용) 
            PhotonNetwork.ConnectUsingSettings(); //위 버전을 가진 마스터 서버로 접속 (로비).
            RoomID.text = "Room_" + Random.Range(0, 1000).ToString("000");
        }
    }
    public override void OnConnectedToMaster() //네트워크에 문제가 없다면 자동으로 콜백되어서
    {
        PhotonNetwork.JoinLobby(); //로비에 연결한다. (로비 씬을 불러왔을 때 첫번째로 호출 됨.)
    }
    public override void OnJoinedLobby() //로비에 연결됬을 때 호출됨 (로비 씬을 불러왔을 때 두번째로 호출됨)
    {
        Debug.Log("Lobby에 입장");
        //PhotonNetwork.JoinRandomRoom(); //아무방이나 접속하라고 명령한다. (랜덤 매치 매이킹) ->버튼 입장으로 바꾸기 위해 주석처리
        userID.text = GetUserID();
    }
    string GetUserID()
    {
        string userID = PlayerPrefs.GetString("USER_ID"); //userID변수에 USER_ID라는 키 값지정. (다른 함수에서도 이 변수를 불러오도록 하기 위해)
        if(string.IsNullOrEmpty(userID)) //userId 가 입력되지 않았다면 (null이나 빈값이라면)
        {
            userID = "USER_" + Random.Range(0, 1000).ToString("000"); //사용자가 아이디 입력하지 않으면 아이디를 USER_000 ~ USER_999 사이의 랜덤 값으로 표현
        }
        return userID;
    }

    public override void OnJoinRandomFailed(short returnCode, string message) //JoinRandomRoom을 통해 연결 실패시 자동으로 불러와 지는(=콜백) 함수 (OnJoinedLobby() 함수에서 방이 없으면 호출 됨, 방이 있으면 스킵)
    {
        print("No Roooms");
        PhotonNetwork.CreateRoom("MyRoom", new RoomOptions { MaxPlayers = 10 });
    }

    public override void OnJoinedRoom() //방에 접속하게(or 만들게) 되면 자동으로 호출되는 함수
    {
        Debug.Log("Enter Room");
        StartCoroutine(LoadTankMainScene());
    }

    IEnumerator LoadTankMainScene()
    {
        PhotonNetwork.IsMessageQueueRunning = false; //씬이 이동하는 동안 포톤 클라우드 서버로부터 네트워크 메시지 수신을 중단함. (씬이 이동하는 동안 몬스터에 맞는 것 등을 방지)
        AsyncOperation ao = SceneManager.LoadSceneAsync("TankMainScene"); //비동기적으로 백그라운드로 씬로딩 (비동기적으로 씬을 로드함으로써 씬 로딩중에 다른 유니티 스크립트가 사용될 수 있음. ex.로딩창)
        yield return ao;
    }

    public void OnClickJoinRandomRoom() //랜덤방 입장 버튼을 눌렀을 때
    {
        PhotonNetwork.NickName = userID.text; //포톤 서버에 사용자가 입력한 아이디 전달
        PlayerPrefs.SetString("USER_ID", userID.text); //USER_ID키를 가진 변수에 사용자가 입력한 아이디 저장.
        PhotonNetwork.JoinRandomRoom(); //랜덤방 입장
    }

    public void OnClickCreateRoom() //방 생성 버튼을 눌렀을 때
    {
        string _roomID = RoomID.text;
        if(string.IsNullOrEmpty(RoomID.text)) //Awake에서 룸아이디가 만들어지지 않았을 경우 예외처리, 혹은 사용자가 룸아이디를 지워서 생성할 경우 예외처리
        {
            _roomID = "Room_" + Random.Range(0, 1000).ToString("000");
        }

        PhotonNetwork.NickName = userID.text; //방 생성시 사용자가 지정 혹은 랜덤으로 지정된 아이디가 닉네임으로 전달.
        PlayerPrefs.SetString("USER_ID", userID.text); //USER_ID키를 가진 변수에 사용자가 입력한 아이디 저장.

        RoomOptions roomOptions = new RoomOptions(); //룸 옵션 클래스 동적할당.
        roomOptions.IsOpen = true; //방 설정을 공개방으로 설정한다.
        roomOptions.IsVisible = true; //방 목록 리스트에 생성한 방을 보이도록 한다.
        roomOptions.MaxPlayers = 4; //방의 최대 인원수를 4명으로 설정한다.

        //roomOptions.CustomRoomPropertiesForLobby = new string[] { "Map", "GameType" }; //roomOptions의 CustomRoomProperties의 속성 명칭을 배열로 전달하는 코드
        //roomOptions.CustomRoomProperties = new Hashtable() { { "Map", 5 }, { "GameType", "TDM" } }; //예시) 프로퍼티 이니셜라이즈 {} 안에 더 선언 해야함, 지금까지 쓴건 5번째 맵을 사용하고 팀데스매치형태의 방생성 

        PhotonNetwork.CreateRoom(_roomID, roomOptions, TypedLobby.Default); //지정한 조건의 방을 만든다, 해당 방의 이름은 _roomID, 해당방의 설정은 roomOptions, 로비가 있다면 해당 로비에 입장.
    } //방 생성 실패시 예외처리 함수는 생략, 거의 된다(흠..)

    public override void OnRoomListUpdate(List<RoomInfo> roomList) //생성된 룸 목록이 변경되었을 때 호출되는 콜백 함수(룸목록에 새로운 룸이 생성되면 자동으로 호출)
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("ROOMITEM")) //ROOMITEM 태그를 가진 모든 오브젝트를 찾아 obj에 담는다. (방을 삭제하기 위해)
        {
            Destroy(obj);
        } //해당 코드는 방목록이 변경되면 모든 방을 삭제하는 방식. 룸이 남는이유는 이 코드 실행후 아래의 코드가 다시 실행되기 때문이다.

        foreach (RoomInfo roomInfo in roomList) //룸 목록안에 있는 룸 정보를 모두 꺼낸다.
        {
            //Debug.Log(roomInfo.Name); //방을 만들면 방이름을 로그창에 띄운다. 룸에 접속하면 안보임.
            GameObject room = (GameObject)Instantiate(roomItem); //RoomItem프리팹을 동적으로 생성한다.
            room.transform.SetParent(scrollContents.transform, false); //RoomItem프리팹을 스크롤콘텐트 오브젝트 자식으로 생성한다. 좌표는 로컬좌표로 생성

            RoomData roomData = room.GetComponent<RoomData>(); //소환한 RoomItem프리팹에 있는 RoomData컴퍼넌트의 정보를 roomData에 저장한다.
            roomData.roomID = roomInfo.Name; //생성된 룸의 이름을 roomData에 roomID변수에 할당한다.
            roomData.connectPlayer = roomInfo.PlayerCount; //생성된 룸의 연결된 클라이언트 수를 roomData에 connectPlayer변수에 저장한다.
            roomData.maxPlayer = roomInfo.MaxPlayers; //생성된 룸의 접속 가능한 최대 클라이언트 수를 roomData에 maxPlayer변수에 저장한다.
            roomData.DisplayRoomData(); //위에서 받은 정보를 토대로 텍스트를 변경하는 함수

            roomData.GetComponent<Button>().onClick.AddListener(delegate { OnClickRoomItem(roomData.roomID); }); //RoomItem의 Button컴퍼넌트에 클릭이벤트를 동적으로 연결. (RoomItem을 클릭하면 OnClickRoomItem 함수 호출)
        }
        #region 내가 만드는 방 삭제 코드

        //for (int i = 0; i < GameObject.FindGameObjectsWithTag("ROOMITEM").Length -1; i++)
        //{

        //}
        #endregion
    }

    private void OnClickRoomItem(string roomID)
    {
        PhotonNetwork.NickName = userID.text; //포톤 서버에 사용자가 입력한 아이디 전달
        PlayerPrefs.SetString("USER_ID", userID.text); //USER_ID키를 가진 변수에 사용자가 입력한 아이디 저장.
        PhotonNetwork.JoinRoom(roomID); //roomID를 가진 방으로 접속한다.
    }

    private void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.InRoom.ToString()); //실행 되면 자동으로 콜백 되어서 유니티 실시간 화면에 레이블을 보여준다.
    }
}
