using Photon.Pun;
using Cinemachine;
public class CameraSetup : MonoBehaviourPun
{
    void Start()
    {
        if (photonView.IsMine) //포톤뷰가 나의 것이라면 (로컬 오브젝트라면)
        {
            CinemachineVirtualCamera virtualCamera = FindObjectOfType(typeof(CinemachineVirtualCamera)) as CinemachineVirtualCamera;
            virtualCamera.Follow = transform;
            virtualCamera.LookAt = transform;
        }
    }
}
