using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public bool isMute = false; //���Ұ�
    public static SoundManager s_instance;
    private void Awake()
    {
        if (s_instance == null)
            s_instance = this;
        else if(s_instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    public void BackGroundSound(Vector3 pos, AudioClip bgm, bool isLoop) //�������
    {
        if (isMute) return; //���Ұ� ���̶�� �Լ��� ��������

        GameObject soundObj = new GameObject("backGroundSound"); //��׶��� ���带 ����� ������Ʈ�� �����Ѵ�.
        soundObj.transform.position = pos; //�Ҹ��� ����Ǵ� ��ġ == ������Ʈ ��ġ
        AudioSource audioSource = soundObj.AddComponent<AudioSource>(); //��׶��� ���带 ����ϴ� ������Ʈ�� ������ҽ� ���۳�Ʈ�� �����.

        audioSource.clip = bgm;
        audioSource.loop = isLoop;
        audioSource.minDistance = 10.0f;
        audioSource.maxDistance = 30.0f;
        audioSource.volume = 1.0f;
        audioSource.Play();
    }
    public void OtherPlaySound(Vector3 pos, AudioClip sfx) //���� �Ҹ� (SFX)
    {
        if (isMute) return; //���Ұ� ���̶�� �Լ��� ��������

        GameObject soundObj = new GameObject("SFX"); //SFX ���带 ����� ������Ʈ�� �����Ѵ�.
        soundObj.transform.position = pos; //�Ҹ��� ����Ǵ� ��ġ == ������Ʈ ��ġ
        AudioSource audioSource = soundObj.AddComponent<AudioSource>(); //SFX ���带 ����ϴ� ������Ʈ�� ������ҽ� ���۳�Ʈ�� �����.

        audioSource.clip = sfx;
        audioSource.minDistance = 10.0f;
        audioSource.maxDistance = 30.0f;
        audioSource.volume = 1.0f;
        audioSource.Play();
    }
}
