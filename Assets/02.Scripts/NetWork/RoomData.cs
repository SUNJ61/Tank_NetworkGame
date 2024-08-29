using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class RoomData : MonoBehaviourPun
{
    [HideInInspector] //�ܺ� ������ ���� public���� ���������� �ν����Ϳ� ������ �ʰ� ����
    public string roomID = string.Empty; //�� �̸��� ������ ����
    [HideInInspector]
    public int connectPlayer = 0; //����� Ŭ���̾�Ʈ ���� ������ ����
    [HideInInspector]
    public int maxPlayer = 0; //�뿡 ������ �� �ִ� �ִ� Ŭ���̾�Ʈ ���� ������ ����

    public Text textRoomID;
    public Text textConnectInfo;

    public void DisplayRoomData()
    {
        textRoomID.text = roomID;
        textConnectInfo.text = $"({connectPlayer.ToString()}/{maxPlayer.ToString()})";
    }
}
