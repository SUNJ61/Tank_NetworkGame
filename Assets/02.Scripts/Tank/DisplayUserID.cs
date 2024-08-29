using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class DisplayUserID : MonoBehaviourPun
{
    public Text UserID;
    void Start()
    {
        UserID.text = photonView.Owner.NickName;  //텍스트 창에 로컬 오브젝트의 클라이언트 닉네임을 표시한다.
    }
}
