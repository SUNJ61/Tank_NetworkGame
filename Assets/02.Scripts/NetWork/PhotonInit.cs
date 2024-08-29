using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;//Pun ��Ʈ��ũ ���� ���� ���̺귯���� ����.
using UnityEngine.UI;
public class PhotonInit : MonoBehaviourPunCallbacks //�κ� ������ �� ���
{
    public InputField userID;
    public InputField RoomID;
    public GameObject roomItem;
    public GameObject scrollContents;

    public List<GameObject> obj;

    public string Version = "V1.0";
    void Awake()
    {
        if (!PhotonNetwork.IsConnected) //���ӵ��� �ʾ����� ���� �ڵ带 ����, �̹� ����� ���¶�� ���� x (Exit��ư�� ���� ������ �� ���� �߻� ����) 
        {
            PhotonNetwork.GameVersion = Version; //���� ������ �����ϴ� ������Ƽ (������Ƽ : Ŭ���� �ȿ� ������ ������ �� ���) 
            PhotonNetwork.ConnectUsingSettings(); //�� ������ ���� ������ ������ ���� (�κ�).
            RoomID.text = "Room_" + Random.Range(0, 1000).ToString("000");
        }
    }
    public override void OnConnectedToMaster() //��Ʈ��ũ�� ������ ���ٸ� �ڵ����� �ݹ�Ǿ
    {
        PhotonNetwork.JoinLobby(); //�κ� �����Ѵ�. (�κ� ���� �ҷ����� �� ù��°�� ȣ�� ��.)
    }
    public override void OnJoinedLobby() //�κ� ������� �� ȣ��� (�κ� ���� �ҷ����� �� �ι�°�� ȣ���)
    {
        Debug.Log("Lobby�� ����");
        //PhotonNetwork.JoinRandomRoom(); //�ƹ����̳� �����϶�� ����Ѵ�. (���� ��ġ ����ŷ) ->��ư �������� �ٲٱ� ���� �ּ�ó��
        userID.text = GetUserID();
    }
    string GetUserID()
    {
        string userID = PlayerPrefs.GetString("USER_ID"); //userID������ USER_ID��� Ű ������. (�ٸ� �Լ������� �� ������ �ҷ������� �ϱ� ����)
        if(string.IsNullOrEmpty(userID)) //userId �� �Էµ��� �ʾҴٸ� (null�̳� ���̶��)
        {
            userID = "USER_" + Random.Range(0, 1000).ToString("000"); //����ڰ� ���̵� �Է����� ������ ���̵� USER_000 ~ USER_999 ������ ���� ������ ǥ��
        }
        return userID;
    }

    public override void OnJoinRandomFailed(short returnCode, string message) //JoinRandomRoom�� ���� ���� ���н� �ڵ����� �ҷ��� ����(=�ݹ�) �Լ� (OnJoinedLobby() �Լ����� ���� ������ ȣ�� ��, ���� ������ ��ŵ)
    {
        print("No Roooms");
        PhotonNetwork.CreateRoom("MyRoom", new RoomOptions { MaxPlayers = 10 });
    }

    public override void OnJoinedRoom() //�濡 �����ϰ�(or �����) �Ǹ� �ڵ����� ȣ��Ǵ� �Լ�
    {
        Debug.Log("Enter Room");
        StartCoroutine(LoadTankMainScene());
    }

    IEnumerator LoadTankMainScene()
    {
        PhotonNetwork.IsMessageQueueRunning = false; //���� �̵��ϴ� ���� ���� Ŭ���� �����κ��� ��Ʈ��ũ �޽��� ������ �ߴ���. (���� �̵��ϴ� ���� ���Ϳ� �´� �� ���� ����)
        AsyncOperation ao = SceneManager.LoadSceneAsync("TankMainScene"); //�񵿱������� ��׶���� ���ε� (�񵿱������� ���� �ε������ν� �� �ε��߿� �ٸ� ����Ƽ ��ũ��Ʈ�� ���� �� ����. ex.�ε�â)
        yield return ao;
    }

    public void OnClickJoinRandomRoom() //������ ���� ��ư�� ������ ��
    {
        PhotonNetwork.NickName = userID.text; //���� ������ ����ڰ� �Է��� ���̵� ����
        PlayerPrefs.SetString("USER_ID", userID.text); //USER_IDŰ�� ���� ������ ����ڰ� �Է��� ���̵� ����.
        PhotonNetwork.JoinRandomRoom(); //������ ����
    }

    public void OnClickCreateRoom() //�� ���� ��ư�� ������ ��
    {
        string _roomID = RoomID.text;
        if(string.IsNullOrEmpty(RoomID.text)) //Awake���� ����̵� ��������� �ʾ��� ��� ����ó��, Ȥ�� ����ڰ� ����̵� ������ ������ ��� ����ó��
        {
            _roomID = "Room_" + Random.Range(0, 1000).ToString("000");
        }

        PhotonNetwork.NickName = userID.text; //�� ������ ����ڰ� ���� Ȥ�� �������� ������ ���̵� �г������� ����.
        PlayerPrefs.SetString("USER_ID", userID.text); //USER_IDŰ�� ���� ������ ����ڰ� �Է��� ���̵� ����.

        RoomOptions roomOptions = new RoomOptions(); //�� �ɼ� Ŭ���� �����Ҵ�.
        roomOptions.IsOpen = true; //�� ������ ���������� �����Ѵ�.
        roomOptions.IsVisible = true; //�� ��� ����Ʈ�� ������ ���� ���̵��� �Ѵ�.
        roomOptions.MaxPlayers = 4; //���� �ִ� �ο����� 4������ �����Ѵ�.

        //roomOptions.CustomRoomPropertiesForLobby = new string[] { "Map", "GameType" }; //roomOptions�� CustomRoomProperties�� �Ӽ� ��Ī�� �迭�� �����ϴ� �ڵ�
        //roomOptions.CustomRoomProperties = new Hashtable() { { "Map", 5 }, { "GameType", "TDM" } }; //����) ������Ƽ �̴ϼȶ����� {} �ȿ� �� ���� �ؾ���, ���ݱ��� ���� 5��° ���� ����ϰ� ��������ġ������ ����� 

        PhotonNetwork.CreateRoom(_roomID, roomOptions, TypedLobby.Default); //������ ������ ���� �����, �ش� ���� �̸��� _roomID, �ش���� ������ roomOptions, �κ� �ִٸ� �ش� �κ� ����.
    } //�� ���� ���н� ����ó�� �Լ��� ����, ���� �ȴ�(��..)

    public override void OnRoomListUpdate(List<RoomInfo> roomList) //������ �� ����� ����Ǿ��� �� ȣ��Ǵ� �ݹ� �Լ�(���Ͽ� ���ο� ���� �����Ǹ� �ڵ����� ȣ��)
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("ROOMITEM")) //ROOMITEM �±׸� ���� ��� ������Ʈ�� ã�� obj�� ��´�. (���� �����ϱ� ����)
        {
            Destroy(obj);
        } //�ش� �ڵ�� ������ ����Ǹ� ��� ���� �����ϴ� ���. ���� ���������� �� �ڵ� ������ �Ʒ��� �ڵ尡 �ٽ� ����Ǳ� �����̴�.

        foreach (RoomInfo roomInfo in roomList) //�� ��Ͼȿ� �ִ� �� ������ ��� ������.
        {
            //Debug.Log(roomInfo.Name); //���� ����� ���̸��� �α�â�� ����. �뿡 �����ϸ� �Ⱥ���.
            GameObject room = (GameObject)Instantiate(roomItem); //RoomItem�������� �������� �����Ѵ�.
            room.transform.SetParent(scrollContents.transform, false); //RoomItem�������� ��ũ������Ʈ ������Ʈ �ڽ����� �����Ѵ�. ��ǥ�� ������ǥ�� ����

            RoomData roomData = room.GetComponent<RoomData>(); //��ȯ�� RoomItem�����տ� �ִ� RoomData���۳�Ʈ�� ������ roomData�� �����Ѵ�.
            roomData.roomID = roomInfo.Name; //������ ���� �̸��� roomData�� roomID������ �Ҵ��Ѵ�.
            roomData.connectPlayer = roomInfo.PlayerCount; //������ ���� ����� Ŭ���̾�Ʈ ���� roomData�� connectPlayer������ �����Ѵ�.
            roomData.maxPlayer = roomInfo.MaxPlayers; //������ ���� ���� ������ �ִ� Ŭ���̾�Ʈ ���� roomData�� maxPlayer������ �����Ѵ�.
            roomData.DisplayRoomData(); //������ ���� ������ ���� �ؽ�Ʈ�� �����ϴ� �Լ�

            roomData.GetComponent<Button>().onClick.AddListener(delegate { OnClickRoomItem(roomData.roomID); }); //RoomItem�� Button���۳�Ʈ�� Ŭ���̺�Ʈ�� �������� ����. (RoomItem�� Ŭ���ϸ� OnClickRoomItem �Լ� ȣ��)
        }
        #region ���� ����� �� ���� �ڵ�

        //for (int i = 0; i < GameObject.FindGameObjectsWithTag("ROOMITEM").Length -1; i++)
        //{

        //}
        #endregion
    }

    private void OnClickRoomItem(string roomID)
    {
        PhotonNetwork.NickName = userID.text; //���� ������ ����ڰ� �Է��� ���̵� ����
        PlayerPrefs.SetString("USER_ID", userID.text); //USER_IDŰ�� ���� ������ ����ڰ� �Է��� ���̵� ����.
        PhotonNetwork.JoinRoom(roomID); //roomID�� ���� ������ �����Ѵ�.
    }

    private void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.InRoom.ToString()); //���� �Ǹ� �ڵ����� �ݹ� �Ǿ ����Ƽ �ǽð� ȭ�鿡 ���̺��� �����ش�.
    }
}
