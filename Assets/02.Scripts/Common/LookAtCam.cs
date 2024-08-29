using Cinemachine;
using Photon.Pun;

public class LookAtCam : MonoBehaviourPun
{
    private CinemachineVirtualCamera virtualCamera;
    void Start()
    {
        virtualCamera = FindObjectOfType(typeof(CinemachineVirtualCamera)) as CinemachineVirtualCamera;
    }
    void Update()
    {
        if (photonView.IsMine) //����䰡 ���� ���̶�� (���� ������Ʈ���)
        {
            virtualCamera.LookAt = transform; //ī�޶� �Ĵٺ���.
        }
    }
}
