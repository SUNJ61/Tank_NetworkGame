using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnManager : MonoBehaviourPun
{
    [SerializeField] private List<Transform> spawnList;
    [SerializeField] private GameObject apachePrefab;
    void Awake()
    {
        apachePrefab = Resources.Load<GameObject>("Apache");
        photonView.Synchronization = ViewSynchronization.Unreliable;
        //photonView.ObservedComponents[0] = this;
    }
    private void Start()
    {
        var spawnPoint = GameObject.Find("SpawnPoints").gameObject;
        if (spawnPoint != null)
            spawnPoint.GetComponentsInChildren<Transform>(spawnList);

        spawnList.RemoveAt(0);
        if (spawnList.Count > 0 && PhotonNetwork.IsMasterClient)
        {
            //StartCoroutine(CreateApache());
            InvokeRepeating("InvokeCreateApache", 0.01f, 3.0f);
        }
    }
    #region StartCoroutine栏肺 利 积己
    IEnumerator CreateApache()
    {
        while (GameManager.Instance.isGameOver == false)
        {
            int count = (int)GameObject.FindGameObjectsWithTag("APACHE").Length;
            if (count < 10)
            {
                yield return new WaitForSeconds(3.0f);
                int idx = Random.Range(0, spawnList.Count);
                PhotonNetwork.Instantiate("Apache", spawnList[idx].position, spawnList[idx].rotation, 0, null);
            }
            else
            {
                yield return null;
            }
        }
    }
    #endregion
    #region InvkeRepeating栏肺 利 积己
    private void InvokeCreateApache()
    {
        if (GameManager.Instance.isGameOver == false)
        {
            int count = (int)GameObject.FindGameObjectsWithTag("APACHE").Length;
            if (count < 10)
            {
                int idx = Random.Range(0, spawnList.Count);
                PhotonNetwork.Instantiate(apachePrefab.name, spawnList[idx].position, spawnList[idx].rotation, 0, null);
            }
        }
    }
    #endregion
}
