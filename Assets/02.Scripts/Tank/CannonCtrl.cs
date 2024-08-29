using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class CannonCtrl : MonoBehaviourPun, IPunObservable
{
    private Transform tr;

    private Quaternion curRot = Quaternion.identity;

    public float rotSpeed = 5000f;
    public float upperAngle = -30f; //제한 각도
    public float downAngle = 0f; //제한 각도 
    public float currentRotate = 0f; //현재 회전 각도 
    void Start()
    {
        photonView.Synchronization = ViewSynchronization.Unreliable;
        photonView.ObservedComponents[0] = this; 

        tr = transform;
        curRot = tr.localRotation;
    }
    void Update()
    {   //포신 제한
        if (photonView.IsMine)
        {
            float Wheel = -Input.GetAxisRaw("Mouse ScrollWheel");
            float angle = Time.deltaTime * rotSpeed * Wheel;
            if (Wheel <= -0.01f) //포신을 올릴때 
            {
                currentRotate += angle;
                if (currentRotate > upperAngle)
                {
                    tr.Rotate(angle, 0f, 0f);
                }
                else
                {
                    currentRotate = upperAngle;
                }
            }
            else //포신을 내릴때
            {
                currentRotate += angle;
                if (currentRotate < downAngle)
                {
                    tr.Rotate(angle, 0f, 0f);
                }
                else
                {
                    currentRotate = downAngle;
                }
            }
        }
        else
        {
            tr.localRotation = Quaternion.Slerp(tr.localRotation, curRot, Time.deltaTime * 3.0f);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(tr.localRotation);
        }
        else
        {
            curRot = (Quaternion)stream.ReceiveNext(); //바로 여기에다가 tr.localRotation에 바로 데이터를 받으면 코드를 줄일 수 있다.
        }
    }
}
