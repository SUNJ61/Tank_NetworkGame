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
        if (photonView.IsMine) //포톤뷰가 나의 것이라면 (로컬 오브젝트라면)
        {
            virtualCamera.LookAt = transform; //카메라를 쳐다본다.
        }
    }
}
