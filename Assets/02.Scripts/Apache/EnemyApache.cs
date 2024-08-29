using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class EnemyApache : MonoBehaviourPun
{
    public List<Transform>patrolList = new List<Transform>();
    private Transform tr=null;
    private GameObject expEffect;
    private GameObject[] playerTanks = null; //�÷��̾� ��ũ���� ���� ����Ʈ

    [SerializeField] Transform FirePos1;
    [SerializeField] Transform FirePos2;
    [SerializeField] GameObject A_bullet;
    [SerializeField] LeaserBeam[] leaserBeams;

    private float curDelay = 0f;
    private float maxDelay = 1f;

    private int wayPointCount = 0;
    private int tankIndex = 0;
    private int tankSame;

    private string tankTag = "Player";
    private readonly string apacheTag = "APACHE";

    private bool isSearch = true;
    
    public float moveSpeed = 10.0f;
    public float rotSpeed = 15f;
    void Start()
    {
        leaserBeams[0] = GetComponentsInChildren<LeaserBeam>()[0];
        leaserBeams[1] = GetComponentsInChildren<LeaserBeam>()[1];
        var patrolPoint = GameObject.Find("PatrolPoint");
        if(patrolPoint != null )
            patrolPoint.GetComponentsInChildren<Transform>(patrolList);
        patrolList.RemoveAt(0);
        tr = transform;
        A_bullet = Resources.Load<GameObject>("A_Bullet");
        curDelay = maxDelay;
        expEffect = Resources.Load<GameObject>("Explosion");
    }

    void Update()
    {
        if (PhotonNetwork.IsConnected) //����䰡 ���� �Ǿ��ٸ� ����, ��Ʈ�� �Լ� ����
        {
            if (isSearch)
            {
                WayPointMove();
            }
            else
            {
                Attack();
            }
        }
    }

    void WayPointMove()
    {
        Vector3 PointDist = Vector3.zero;
        float dist = 0f;
       if (wayPointCount ==0)
        {
            PointDist = patrolList[0].position - tr.position;
            tr.rotation = Quaternion.Slerp(tr.rotation,Quaternion.LookRotation(PointDist), Time.deltaTime * rotSpeed);
            tr.Translate(Vector3.forward * moveSpeed *Time.deltaTime);
            dist = Vector3.Distance(tr.position, patrolList[0].position);
            if (dist <= 5.5f)
                wayPointCount = 1;
        }
        
        else if(wayPointCount ==1)
        {
            PointDist = patrolList[1].position - tr.position;
            tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.LookRotation(PointDist), Time.deltaTime * rotSpeed);
            tr.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            dist = Vector3.Distance(tr.position, patrolList[1].position);
            if (dist <= 5.5f)
                wayPointCount = 2;
        }
        
        else if (wayPointCount == 2)
        {
            PointDist = patrolList[2].position - tr.position;
            tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.LookRotation(PointDist), Time.deltaTime * rotSpeed);
            tr.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            dist = Vector3.Distance(tr.position, patrolList[2].position);
            if (dist <= 5.5f)
                wayPointCount = 3;
        }
        else if (wayPointCount == 3)
        {
            PointDist = patrolList[3].position - tr.position;
            tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.LookRotation(PointDist), Time.deltaTime * rotSpeed);
            tr.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            dist = Vector3.Distance(tr.position, patrolList[3].position);
            if (dist <= 5.5f)
                wayPointCount = 0;
        }

        Search();
    }

    void Search()
    {
        #region �̱� ��� �߰� ����
        //float TankFindDist  = (GameObject.FindWithTag("Player").transform.position - tr.position).magnitude;
        //if (TankFindDist <= 80f)
        //    isSearch = false;
        #endregion
        #region ���� ������ ��Ƽ �߰� ����
        playerTanks = GameObject.FindGameObjectsWithTag(tankTag);
        Transform target = playerTanks[0].transform; //�� �迭 ������Ʈ�� ���� ù��° ������Ʈ�� Ÿ�� ������ �ִ´�.
        float distance = Vector3.Distance(target.position, tr.position); //����ġ �ڱ� �ڽŰ�, Ÿ���� ������ �Ÿ��� ����Ͽ� �����Ѵ�. ().sqrMagnitude���� distance�� ����

        float distanceAll;
        foreach (var tank in playerTanks) //��� ��ũ�� ����ġ ������ �뷫���� �Ÿ��� ���Ѵ�.
        {
            distanceAll = Vector3.Distance(tank.transform.position, tr.position); // distanceAll = (tank.transform.position - tr.position).sqrMagnitude;�� ����� �ڵ�
            tankIndex++;
            if (distanceAll < distance) //�迭 ù��° ��ũ���� ��ġ�� ����ġ ������ �Ÿ��� �ٸ� ��ũ�� �Ÿ����� Ŭ ��� (����ġ ���� �� ����� ��ũ�� ������ ���)
            {
                target = tank.transform;
                distance = distanceAll; //distance = (tank.transform.position - tr.position).sqrMagnitude;�� ����� �ڵ�
                tankSame = tankIndex;
            }
            if (tankIndex == tankSame)
            {
                target = tank.transform;
                distance = distanceAll;
            }
        }
        tankIndex = 0;

        if (distance < 80f)
            isSearch = false;
        #endregion

        #region ������� ������ ��Ƽ �߰� ���� 
        //playerTanks = GameObject.FindGameObjectsWithTag(tankTag);
        //Transform target_T = playerTanks[0].transform;
        ////ù��° �迭��ҿ� �ִ� ��ũ�� ��ġ�� ����ġ ����� �Ÿ��� ���
        ////ù��° �迭 ����� ��ũ�� �������� �Ÿ��� ��� ���� ������ �� ���̴�.

        //float dist_T = (target_T.position - tr.position).magnitude;
        //float dist2D;

        //foreach (GameObject _tank in playerTanks)
        //{//ù��° ����� ��ũ ������ �������� ����ġ ���� ��ũ���� ��ü�Ÿ��� ���.

        //    dist2D = (_tank.transform.position - tr.position).magnitude;

        //    if (dist2D < dist_T)
        //    {
        //        target_T = _tank.transform;
        //        dist_T = (_tank.transform.position - tr.position).magnitude;
        //    }
        //}

        //if (target_T.position.magnitude < 100f)
        //{
        //    isSearch = false;
        //}
        #endregion
    }
    void Attack()
    {
        #region ���� ������ ��Ƽ ���ݷ���
        playerTanks = GameObject.FindGameObjectsWithTag(tankTag);
        Transform target = playerTanks[0].transform;
        float dist = Vector3.Distance(target.position, tr.position); //(target.position - tr.position).magnitude;�� ����� �ڵ�

        float distAll;
        foreach (var tank in playerTanks)
        {
            distAll = Vector3.Distance(tank.transform.position, tr.position); //distanceAll = (tank.transform.position - tr.position).sqrMagnitude;�� ����� �ڵ�
            tankIndex++;
            if (distAll < dist)
            {
                target = tank.transform;
                dist = distAll; // //distance = (tank.transform.position - tr.position).sqrMagnitude;�� ����� �ڵ�
                tankSame = tankIndex;

                tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.LookRotation(target.position - tr.position), Time.deltaTime * rotSpeed);
                FireRay();
            }
            if (tankIndex == tankSame)
            {
                target = tank.transform;
                dist = distAll;

                tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.LookRotation(target.position - tr.position), Time.deltaTime * rotSpeed);
                FireRay();
            }
        }
        tankIndex = 0;

        if (dist > 80f)
            isSearch = true;
        #endregion

        #region ������� ������ ��Ƽ ���� ����
        //playerTanks = GameObject.FindGameObjectsWithTag(tankTag);
        //Transform target_T = playerTanks[0].transform;
        ////ù��° �迭��ҿ� �ִ� ��ũ�� ��ġ�� ����ġ ����� �Ÿ��� ���
        ////ù��° �迭 ����� ��ũ�� �������� �Ÿ��� ��� ���� ������ �� ���̴�.
        //float dist_T = (target_T.position - tr.position).magnitude;
        //float dist2D;

        //foreach (GameObject _tank in playerTanks)
        //{//ù��° ����� ��ũ ������ �������� ����ġ ���� ��ũ���� ��ü�Ÿ��� ���.

        //    dist2D = (_tank.transform.position - tr.position).magnitude;

        //    if (dist2D < dist_T)
        //    {
        //        target_T = _tank.transform;
        //        dist_T = (_tank.transform.position - tr.position).magnitude;
        //    }

        //}

        //Vector3 _normal = target_T.position - tr.position;

        //tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.LookRotation(_normal), Time.deltaTime * 3.0f);
        //FireRay();

        //if (target_T.position.magnitude > 100f)
        //{
        //    isSearch = true;
        //}
        #endregion

        #region �̱ۿ� �ڵ�
        //Vector3 targetDist = (GameObject.FindWithTag("Player").transform.position - tr.position);
        //tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.LookRotation(targetDist.normalized), Time.deltaTime * rotSpeed);

        //if (targetDist.magnitude > 80f)
        //{
        //    isSearch = true;
        //}
        #endregion
    }

    [PunRPC]
    private void FireRay()
    {
        Ray ray = new Ray(FirePos1.position, FirePos1.forward * 100.0f);
        Ray ray1 = new Ray(FirePos2.position, FirePos2.forward * 100.0f);
        RaycastHit hit;
        //RaycastHit hit1;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 9) || Physics.Raycast(ray1, out hit, Mathf.Infinity, 1 << 9))
        {
            curDelay -= 0.01f;
            if (curDelay <= 0)
            {
                curDelay = maxDelay;
                leaserBeams[0].FireRay();
                leaserBeams[1].FireRay();
                ShowEffect(hit);
                if(hit.collider.tag == tankTag)
                {
                    string Tag = apacheTag;
                    hit.collider.transform.parent.SendMessage("OnDamage", Tag, SendMessageOptions.DontRequireReceiver);
                }
            }
        }
        else if(Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 8) || Physics.Raycast(ray1, out hit, Mathf.Infinity, 1 << 8))
        {
            curDelay -= 0.01f;
            if (curDelay <= 0)
            {
                curDelay = maxDelay;
                leaserBeams[0].FireRay();
                leaserBeams[1].FireRay();
                ShowEffect(hit);

            }
        }
    }

    void ShowEffect(RaycastHit hit)
    {
        Vector3 hitPos = hit.point;
        Vector3 _normal =(FirePos1.position - hitPos).normalized;
        Quaternion rot  = Quaternion.FromToRotation(-Vector3.forward, _normal);
        GameObject hitEff = Instantiate(expEffect, hitPos, rot);
        Destroy(hitEff, 1.0f);

    }
    private void Fire()
    {
        //Instantiate(A_bullet, FirePos1.position, FirePos1.rotation);
        //Instantiate(A_bullet,FirePos2.position, FirePos2.rotation);
    }
}
