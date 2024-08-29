using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class RoomData : MonoBehaviourPun
{
    [HideInInspector] //외부 접근을 위해 public으로 선언했지만 인스펙터에 보이지 않게 설정
    public string roomID = string.Empty; //룸 이름을 저장할 변수
    [HideInInspector]
    public int connectPlayer = 0; //연결된 클라이언트 수를 저장할 변수
    [HideInInspector]
    public int maxPlayer = 0; //룸에 접속할 수 있는 최대 클라이언트 수를 저장할 변수

    public Text textRoomID;
    public Text textConnectInfo;

    public void DisplayRoomData()
    {
        textRoomID.text = roomID;
        textConnectInfo.text = $"({connectPlayer.ToString()}/{maxPlayer.ToString()})";
    }
}
