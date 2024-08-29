using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
public class FireCtrl : MonoBehaviourPun
{
    public GameObject bullet =null;
    public Transform firePos = null;
    public LeaserBeamT BeamT = null;
    public GameObject expEffect;

    private readonly string playerTag = "Player";
    private readonly string apacheTag = "APACHE";
    void Start()
    {
      bullet =  Resources.Load<GameObject>("Bullet");
      firePos = transform.GetChild(4).GetChild(1).GetChild(0).GetChild(0).transform;
      BeamT = GetComponentInChildren<LeaserBeamT>();
      expEffect = Resources.Load<GameObject>("Explosion");
    }
    void Update()
    {
            if (Input.GetMouseButtonDown(0)&&photonView.IsMine) //로컬 오브젝트일때 마우스 좌클릭을 누르면
            {
                Fire(); //발사를 진행하고 
                photonView.RPC("Fire", RpcTarget.Others); //다른 클라이언트의 리모트 오브젝트의 Fire함수를 작동 시킨다.
            }
    }

    [PunRPC]
    void Fire()
    {
        //if (EventSystem.current.IsPointerOverGameObject()) return; //UI에 닿았을 때는 리턴한다. (EventSystem이 있는 것에 마우스 포인터가 있을 경우 리턴하는 함수) ---> 이벤트를 막는 방법 1
        if (HoverEvent.event_instance.isEnter) return; //HoverEvent 컴포넌트를 가진 오브젝트를 눌렀을 때 리턴한다. ---> 이벤트를 막는 방법 2

        //Instantiate(bullet,firePos.position,firePos.rotation);
        RaycastHit hit;
        Ray ray = new Ray(firePos.position, firePos.forward);
        if(Physics.Raycast(ray,out hit,100f,1<<8 | 1<<9 | 1<<10 ))
        {
            BeamT.FireRay();
            ShowEffect(hit);
            if (hit.collider.CompareTag(playerTag))
            {
                string Tag = hit.collider.tag;
                hit.collider.transform.parent.SendMessage("OnDamage", Tag, SendMessageOptions.DontRequireReceiver);
            }
            if(hit.collider.CompareTag(apacheTag))
            {
                string Tag = hit.collider.tag;
                hit.collider.transform.parent.SendMessage("OnDamage", Tag, SendMessageOptions.DontRequireReceiver);
            }
        }
        else
        {
            BeamT.FireRay();
            Vector3 hitpos = ray.GetPoint(200f);
            Vector3 _normal = firePos.position - hitpos.normalized;
            Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, _normal);
            GameObject eff = Instantiate(expEffect, hitpos, rot);
            Destroy(eff, 1.5f);
        }
    }
    void ShowEffect(RaycastHit hitTank)
    {
        Vector3 hitpos = hitTank.point;
        Vector3 _normal = firePos.position - hitpos.normalized;
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, _normal);
        GameObject eff = Instantiate(expEffect,hitpos,rot);
        Destroy(eff,1.5f );
    }
}
