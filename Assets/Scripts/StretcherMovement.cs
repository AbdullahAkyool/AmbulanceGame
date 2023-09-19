using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class StretcherMovement : MonoBehaviour
{
    public static StretcherMovement Instance;
    
    private NavMeshAgent agent;

    public Transform stretcherInsidePoint;
    public Transform stretcherJumpPoint;
    public Transform patientPointOnStretcher;
    private void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
    }

    public void GetOutOfTheAmbulance(PatientInfo patient)
    {
        transform.DOJump(stretcherJumpPoint.position, .5f, 1, .3f).OnComplete(() =>
        {
            agent.enabled = true;
            StartCoroutine(GoToPatient(patient.transform));
        });
    }

    public void GetInTheAmbulance()
    {
        agent.enabled = false;
        transform.rotation = stretcherInsidePoint.rotation;
        transform.DOJump(stretcherInsidePoint.position, .5f, 1, .4f).OnComplete(() =>
        {
            transform.GetComponentInParent<AmbulanceMovement>().isDriveable = true;
            transform.GetComponentInParent<AmbulanceSystem>().CloseDoors();
        });
    }
    
    IEnumerator  GoToPatient(Transform target)
    {
        agent.SetDestination(target.position);
        yield return new WaitWhile(() => ReachedDestinationOrGaveUp(agent)==false);
        TakePatient(target.GetComponent<PatientInfo>());
    }
    
    public void TakePatient(PatientInfo patient)
    {
        patient.transform.DOJump(patientPointOnStretcher.position, .5f, 1, .4f) .OnComplete(() =>
        {
            patient.transform.rotation = patientPointOnStretcher.rotation;
            patient.transform.SetParent(transform);
            StartCoroutine(BackToAmbulance());
        });
    }
    
    IEnumerator BackToAmbulance()
    {
        agent.SetDestination(stretcherJumpPoint.position);
        yield return new WaitWhile(() => ReachedDestinationOrGaveUp(agent)==false);
        GetInTheAmbulance();
    }
    
    
    
    public  bool ReachedDestinationOrGaveUp(NavMeshAgent navMeshAgent) {
        if (!navMeshAgent.pathPending) {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f) {
                    return true;
                }
            }
        }
        return false;
    }
}
