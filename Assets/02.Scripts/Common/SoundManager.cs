using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public bool isMute = false; //음소거
    public static SoundManager s_instance;
    private void Awake()
    {
        if (s_instance == null)
            s_instance = this;
        else if(s_instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    public void BackGroundSound(Vector3 pos, AudioClip bgm, bool isLoop) //배경음악
    {
        if (isMute) return; //음소거 중이라면 함수를 빠져나감

        GameObject soundObj = new GameObject("backGroundSound"); //백그라운드 사운드를 재생할 오브젝트를 생성한다.
        soundObj.transform.position = pos; //소리가 재생되는 위치 == 오브젝트 위치
        AudioSource audioSource = soundObj.AddComponent<AudioSource>(); //백그라운드 사운드를 재생하는 오브젝트에 오디오소스 컴퍼넌트를 만든다.

        audioSource.clip = bgm;
        audioSource.loop = isLoop;
        audioSource.minDistance = 10.0f;
        audioSource.maxDistance = 30.0f;
        audioSource.volume = 1.0f;
        audioSource.Play();
    }
    public void OtherPlaySound(Vector3 pos, AudioClip sfx) //폭파 소리 (SFX)
    {
        if (isMute) return; //음소거 중이라면 함수를 빠져나감

        GameObject soundObj = new GameObject("SFX"); //SFX 사운드를 재생할 오브젝트를 생성한다.
        soundObj.transform.position = pos; //소리가 재생되는 위치 == 오브젝트 위치
        AudioSource audioSource = soundObj.AddComponent<AudioSource>(); //SFX 사운드를 재생하는 오브젝트에 오디오소스 컴퍼넌트를 만든다.

        audioSource.clip = sfx;
        audioSource.minDistance = 10.0f;
        audioSource.maxDistance = 30.0f;
        audioSource.volume = 1.0f;
        audioSource.Play();
    }
}
