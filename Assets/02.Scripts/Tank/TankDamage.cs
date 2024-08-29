using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
//탱크 hp가 0이하일때 잠시 메쉬렌더러를 비활성화 해서 다시 5초후에 활성화 시킨다.
public class TankDamage : MonoBehaviourPun
{
    [SerializeField] private MeshRenderer[] m_Renderer;
    [SerializeField] private GameObject expEffect;

    private int initHp = 100;
    private int curHp = 0;

    private readonly string playerTag = "Player";
    private readonly string apacheTag = "APACHE";
    private readonly string unTag = "Untagged";

    public Canvas hudCanvas;
    public Image HpBar;

    void Start()
    {
        m_Renderer = GetComponentsInChildren<MeshRenderer>();
        expEffect = Resources.Load<GameObject>("Explosion");

        curHp = initHp;
        HpBar.color = Color.green;
    }

    [PunRPC]
    void OnDamageRPC(string Tag) //Sendmessage로 호출하여 사용
    { //네트워크 상에서 데미지 처리를 동기화
        if(curHp > 0 && Tag == playerTag)
        {
            curHp -= 25;
            HpBar.fillAmount = (float)curHp / (float)initHp;
            if (HpBar.fillAmount <= 0.4f)
                HpBar.color = Color.red;
            else if (HpBar.fillAmount <= 0.6f)
                HpBar.color = Color.yellow;

            if (curHp <= 0)
            {
                StartCoroutine(ExplosionTank());
            }
        }
        else if(curHp > 0 && Tag == apacheTag)
        {
            curHp -= 1;
            HpBar.fillAmount = (float)curHp / (float)initHp;
            if (HpBar.fillAmount <= 0.4f)
                HpBar.color = Color.red;
            else if (HpBar.fillAmount <= 0.6f)
                HpBar.color = Color.yellow;

            if (curHp <= 0)
            {
                StartCoroutine(ExplosionTank());
            }
        }
    }

    public void OnDamage(string Tag)
    {
        if (photonView.IsMine)
            photonView.RPC("OnDamageRPC", RpcTarget.All, Tag);
    }

    IEnumerator ExplosionTank()
    {
        var effect = Instantiate(expEffect, transform.position, Quaternion.identity);
        Destroy(effect, 2.0f);
        SetTankvisible(false);
        hudCanvas.enabled = false;
        gameObject.tag = unTag;

        yield return new WaitForSeconds(5.0f);

        curHp = initHp;
        SetTankvisible(true);
        HpBar.fillAmount = 1.0f;
        HpBar.color = Color.green;
        hudCanvas.enabled= true;
        gameObject.tag = playerTag;
    }

    void SetTankvisible(bool isvisible)
    {
        foreach(var tank in m_Renderer)
            tank.enabled = isvisible;
    }
}
