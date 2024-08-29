using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
// ray를 쏘아서 마우스 포지션 따라서 
public class TurretCtrl : MonoBehaviourPun, IPunObservable
{
    private Transform tr;
    private float rotSpeed = 5f;
    private Quaternion curRot = Quaternion.identity;
    RaycastHit hit;

    void Start()
    {
        photonView.Synchronization = ViewSynchronization.Unreliable;
        photonView.ObservedComponents[0] = this;

        tr = transform;
        curRot = tr.localRotation; //터렛은 탱크의 자식인데 탱크와 다르게 독립적으로 움직이므로 localRotation을 가져온다.
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(tr.localRotation);
        }
        else
        {
            curRot = (Quaternion)stream.ReceiveNext();
        }
    }
    void Update()
    {
        if (photonView.IsMine) //로컬 오브젝트만 마우스를 통해 로테이션 변경
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //카메라에서 마우스 포지션 방향으로 광선을 발사 
            Debug.DrawRay(ray.origin, ray.direction * 1000.0f, Color.green);

            if (Physics.Raycast(ray, out hit, 1000.0f, 1 << 8))
            {  //레이가 터레인에 맞았다면 

                Vector3 relative = tr.InverseTransformPoint(hit.point);
                //맞았던 월드 위치를 탱크에 맞는 로컬좌표로 바꿈
                // Mathf.Deg2Rad;일반각도를 라디언 각도로 바꿈 
                float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg; //라디언각도를 일반각도로 바꿈
                                                                                   //역탄젠트 함수인 Atan2로  두점간 각도를 계산
                tr.Rotate(0f, angle * Time.deltaTime * rotSpeed, 0f);
            }
        }
        else //수신된 위치 값으로 리모트 오브젝트(터렛) 위치값 변경 
        {
            tr.localRotation = Quaternion.Slerp(tr.localRotation, curRot, Time.deltaTime * 3.0f);
        }
    }
}
