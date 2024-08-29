using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
// A: 왼쪽 회전 D 오른쪽으로 회전  W 전진  S 후진 
public class TankMove : MonoBehaviourPun, IPunObservable
{
    [SerializeField]private Rigidbody rb;
    private Transform tr;

    private float h = 0f, v = 0f;
    private Vector3 curPos = Vector3.zero; //리모트 오브젝트의 transform값을 수신 받기 위한 변수이다. 
    private Quaternion curRot = Quaternion.identity; //리모트 오브젝트의 rotation값을 수신 받기 위한 변수이다. 

    public float moveSpeed = 12f;
    public float rotSpeed = 90f;
    void Awake()
    {
        photonView.Synchronization = ViewSynchronization.Unreliable; //동기화 방법(데이터 전송 타입)을 Unreliable로 바꾼다. = UDP
        photonView.ObservedComponents[0] = this; //포톤뷰의 관찰 속성에 TankMove스크립트를 연결한다. 다른 컴포넌트인 포톤뷰 트랜스폼은 알아서 잡힌다. (오토 파인드 all이기 때문)

        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();

        rb.centerOfMass = new Vector3(0f, -0.5f, 0f); //무게중심 위치 설정
        curPos = tr.position; //리모트 오브젝트의 소환된 위치를 저장 
        curRot = tr.rotation; //리모트 오브젝트의 처음 바라보는 방향을 저장 
    }
    void Update()
    {
        if (photonView.IsMine) //클라이언트 입장에서 로컬 오브젝트만 키보드로 움직이도록 한다.
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            tr.Translate(Vector3.forward * v * Time.deltaTime * moveSpeed);
            tr.Rotate(Vector3.up * h * Time.deltaTime * rotSpeed);
        }
        else //클라이언트 입장에서 다른 리모트 오브젝트의 움직임을 관리한다. -> 서버에서 바로 위치값을 저장하여 주석 처리.
        {
            tr.position = Vector3.Lerp(tr.position, curPos, Time.deltaTime * 3.0f); //리모트 오브젝트의 위치를 curPos로 움직인다.
            tr.rotation = Quaternion.Slerp(tr.rotation, curRot, Time.deltaTime * 3.0f); //리모트 오브젝트의 위치를 curRot으로 움직인다.
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) //탱크의 transform과 rotation을 서로 송,수신하기위해 사용한다.
    {
        if(stream.IsWriting) //알아서 직렬화됨
        {
            stream.SendNext(tr.position); //내 로컬 오브젝트의 위치를 서버로 보낸다.
            stream.SendNext(tr.rotation); //내 로컬 오브젝트의 방향을 서버로 보낸다.
        }
        else //알아서 역직렬화 됨
        {
            curPos = (Vector3)stream.ReceiveNext(); //리모트 오브젝트의 위치를 서버로부터 받는다. -> 서버에서 바로 값을 변경할 수 있음, 따로 선언한 것 보다는 부드럽지는 않음.
            curRot = (Quaternion)stream.ReceiveNext(); //리모트 오브젝트의 방향을 서버로부터 받는다. -> 서버에서 바로 값을 변경할 수 있음, 따로 선언한 것 보다는 부드럽지는 않음.
        }
    }
}
