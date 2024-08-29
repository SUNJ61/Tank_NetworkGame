using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
// ray�� ��Ƽ� ���콺 ������ ���� 
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
        curRot = tr.localRotation; //�ͷ��� ��ũ�� �ڽ��ε� ��ũ�� �ٸ��� ���������� �����̹Ƿ� localRotation�� �����´�.
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
        if (photonView.IsMine) //���� ������Ʈ�� ���콺�� ���� �����̼� ����
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //ī�޶󿡼� ���콺 ������ �������� ������ �߻� 
            Debug.DrawRay(ray.origin, ray.direction * 1000.0f, Color.green);

            if (Physics.Raycast(ray, out hit, 1000.0f, 1 << 8))
            {  //���̰� �ͷ��ο� �¾Ҵٸ� 

                Vector3 relative = tr.InverseTransformPoint(hit.point);
                //�¾Ҵ� ���� ��ġ�� ��ũ�� �´� ������ǥ�� �ٲ�
                // Mathf.Deg2Rad;�Ϲݰ����� ���� ������ �ٲ� 
                float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg; //���𰢵��� �Ϲݰ����� �ٲ�
                                                                                   //��ź��Ʈ �Լ��� Atan2��  ������ ������ ���
                tr.Rotate(0f, angle * Time.deltaTime * rotSpeed, 0f);
            }
        }
        else //���ŵ� ��ġ ������ ����Ʈ ������Ʈ(�ͷ�) ��ġ�� ���� 
        {
            tr.localRotation = Quaternion.Slerp(tr.localRotation, curRot, Time.deltaTime * 3.0f);
        }
    }
}
