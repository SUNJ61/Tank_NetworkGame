using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMPlay : MonoBehaviour
{
    private Transform Tr;
    public AudioClip bgm;
    void Start()
    {
        Tr = transform;
        SoundManager.s_instance.BackGroundSound(Tr.position, bgm, true);
    }
}
