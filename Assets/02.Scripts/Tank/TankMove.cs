using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
// A: ���� ȸ�� D ���������� ȸ��  W ����  S ���� 
public class TankMove : MonoBehaviourPun, IPunObservable
{
    [SerializeField]private Rigidbody rb;
    private Transform tr;

    private float h = 0f, v = 0f;
    private Vector3 curPos = Vector3.zero; //����Ʈ ������Ʈ�� transform���� ���� �ޱ� ���� �����̴�. 
    private Quaternion curRot = Quaternion.identity; //����Ʈ ������Ʈ�� rotation���� ���� �ޱ� ���� �����̴�. 

    public float moveSpeed = 12f;
    public float rotSpeed = 90f;
    void Awake()
    {
        photonView.Synchronization = ViewSynchronization.Unreliable; //����ȭ ���(������ ���� Ÿ��)�� Unreliable�� �ٲ۴�. = UDP
        photonView.ObservedComponents[0] = this; //������� ���� �Ӽ��� TankMove��ũ��Ʈ�� �����Ѵ�. �ٸ� ������Ʈ�� ����� Ʈ�������� �˾Ƽ� ������. (���� ���ε� all�̱� ����)

        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();

        rb.centerOfMass = new Vector3(0f, -0.5f, 0f); //�����߽� ��ġ ����
        curPos = tr.position; //����Ʈ ������Ʈ�� ��ȯ�� ��ġ�� ���� 
        curRot = tr.rotation; //����Ʈ ������Ʈ�� ó�� �ٶ󺸴� ������ ���� 
    }
    void Update()
    {
        if (photonView.IsMine) //Ŭ���̾�Ʈ ���忡�� ���� ������Ʈ�� Ű����� �����̵��� �Ѵ�.
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            tr.Translate(Vector3.forward * v * Time.deltaTime * moveSpeed);
            tr.Rotate(Vector3.up * h * Time.deltaTime * rotSpeed);
        }
        else //Ŭ���̾�Ʈ ���忡�� �ٸ� ����Ʈ ������Ʈ�� �������� �����Ѵ�. -> �������� �ٷ� ��ġ���� �����Ͽ� �ּ� ó��.
        {
            tr.position = Vector3.Lerp(tr.position, curPos, Time.deltaTime * 3.0f); //����Ʈ ������Ʈ�� ��ġ�� curPos�� �����δ�.
            tr.rotation = Quaternion.Slerp(tr.rotation, curRot, Time.deltaTime * 3.0f); //����Ʈ ������Ʈ�� ��ġ�� curRot���� �����δ�.
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) //��ũ�� transform�� rotation�� ���� ��,�����ϱ����� ����Ѵ�.
    {
        if(stream.IsWriting) //�˾Ƽ� ����ȭ��
        {
            stream.SendNext(tr.position); //�� ���� ������Ʈ�� ��ġ�� ������ ������.
            stream.SendNext(tr.rotation); //�� ���� ������Ʈ�� ������ ������ ������.
        }
        else //�˾Ƽ� ������ȭ ��
        {
            curPos = (Vector3)stream.ReceiveNext(); //����Ʈ ������Ʈ�� ��ġ�� �����κ��� �޴´�. -> �������� �ٷ� ���� ������ �� ����, ���� ������ �� ���ٴ� �ε巴���� ����.
            curRot = (Quaternion)stream.ReceiveNext(); //����Ʈ ������Ʈ�� ������ �����κ��� �޴´�. -> �������� �ٷ� ���� ������ �� ����, ���� ������ �� ���ٴ� �ε巴���� ����.
        }
    }
}
