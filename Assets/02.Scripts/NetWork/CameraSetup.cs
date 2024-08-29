using Photon.Pun;
using Cinemachine;
public class CameraSetup : MonoBehaviourPun
{
    void Start()
    {
        if (photonView.IsMine) //����䰡 ���� ���̶�� (���� ������Ʈ���)
        {
            CinemachineVirtualCamera virtualCamera = FindObjectOfType(typeof(CinemachineVirtualCamera)) as CinemachineVirtualCamera;
            virtualCamera.Follow = transform;
            virtualCamera.LookAt = transform;
        }
    }
}
