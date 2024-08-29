using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class EnemyApache : MonoBehaviourPun
{
    public List<Transform>patrolList = new List<Transform>();
    private Transform tr=null;
    private GameObject expEffect;
    private GameObject[] playerTanks = null; //플레이어 탱크들을 담을 리스트

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
        if (PhotonNetwork.IsConnected) //포톤뷰가 연결 되었다면 공격, 패트롤 함수 실행
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
        #region 싱글 모드 추격 로직
        //float TankFindDist  = (GameObject.FindWithTag("Player").transform.position - tr.position).magnitude;
        //if (TankFindDist <= 80f)
        //    isSearch = false;
        #endregion
        #region 내가 수정한 멀티 추격 로직
        playerTanks = GameObject.FindGameObjectsWithTag(tankTag);
        Transform target = playerTanks[0].transform; //위 배열 오브젝트에 들어온 첫번째 오브젝트를 타겟 변수에 넣는다.
        float distance = Vector3.Distance(target.position, tr.position); //아파치 자기 자신과, 타겟의 사이의 거리를 계산하여 저장한다. ().sqrMagnitude보다 distance가 빠름

        float distanceAll;
        foreach (var tank in playerTanks) //모든 탱크와 아파치 사이의 대략적인 거리를 구한다.
        {
            distanceAll = Vector3.Distance(tank.transform.position, tr.position); // distanceAll = (tank.transform.position - tr.position).sqrMagnitude;가 강사님 코드
            tankIndex++;
            if (distanceAll < distance) //배열 첫번째 탱크와의 위치와 아파치 사이의 거리가 다른 탱크의 거리보다 클 경우 (아파치 기준 더 가까운 탱크가 생겼을 경우)
            {
                target = tank.transform;
                distance = distanceAll; //distance = (tank.transform.position - tr.position).sqrMagnitude;가 강사님 코드
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

        #region 강사님이 수정한 멀티 추격 로직 
        //playerTanks = GameObject.FindGameObjectsWithTag(tankTag);
        //Transform target_T = playerTanks[0].transform;
        ////첫번째 배열요소에 있는 탱크의 위치와 아파치 헬기의 거리를 잰다
        ////첫번째 배열 요소의 탱크의 포지션은 거리를 재기 위한 기준이 될 뿐이다.

        //float dist_T = (target_T.position - tr.position).magnitude;
        //float dist2D;

        //foreach (GameObject _tank in playerTanks)
        //{//첫번째 요소의 탱크 포지션 기준으로 아파치 헬기와 탱크들의 전체거리를 잰다.

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
        #region 내가 수정한 멀티 공격로직
        playerTanks = GameObject.FindGameObjectsWithTag(tankTag);
        Transform target = playerTanks[0].transform;
        float dist = Vector3.Distance(target.position, tr.position); //(target.position - tr.position).magnitude;가 강사님 코드

        float distAll;
        foreach (var tank in playerTanks)
        {
            distAll = Vector3.Distance(tank.transform.position, tr.position); //distanceAll = (tank.transform.position - tr.position).sqrMagnitude;가 강사님 코드
            tankIndex++;
            if (distAll < dist)
            {
                target = tank.transform;
                dist = distAll; // //distance = (tank.transform.position - tr.position).sqrMagnitude;가 강사님 코드
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

        #region 강사님이 수정한 멀티 공격 로직
        //playerTanks = GameObject.FindGameObjectsWithTag(tankTag);
        //Transform target_T = playerTanks[0].transform;
        ////첫번째 배열요소에 있는 탱크의 위치와 아파치 헬기의 거리를 잰다
        ////첫번째 배열 요소의 탱크의 포지션은 거리를 재기 위한 기준이 될 뿐이다.
        //float dist_T = (target_T.position - tr.position).magnitude;
        //float dist2D;

        //foreach (GameObject _tank in playerTanks)
        //{//첫번째 요소의 탱크 포지션 기준으로 아파치 헬기와 탱크들의 전체거리를 잰다.

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

        #region 싱글용 코드
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
