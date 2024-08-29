using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class EnemyDamage : MonoBehaviourPun
{
    private Transform Tr;
    private GameObject expEffect;

    private readonly string ApacheTag = "APACHE";

    private bool hitDamage = false;
    void Start()
    {
        Tr = transform;
        expEffect = Resources.Load<GameObject>("Explosion");
    }

    [PunRPC]
    private void OnDamageRPC()
    {
        if (hitDamage == false)
        {
            //StartCoroutine(ApacheDie());
            var eff = Instantiate(expEffect, Tr.position, Quaternion.identity); //여기서 왜?
            Destroy(this.gameObject);
            Destroy(eff, 2.0f);
            GameManager.Instance.AddScore();
        }
    }

    private void OnDamage(string Tag)
    {
        if(photonView.IsMine && Tag == ApacheTag)
        {
            photonView.RPC("OnDamageRPC", RpcTarget.All);
        }
    }
    IEnumerator ApacheDie() 
    {
        var eff = Instantiate(expEffect, Tr.position, Quaternion.identity); //여기서 왜?
        Destroy(this.gameObject);
        yield return new WaitForSeconds(1.5f);
        Destroy(eff);
    }
}
