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
    public Transform hospitalPoint;

    private PatientInfo currentPatient = null;
    private bool onTheHospitalWay = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
    }

    public void GetOutOfTheAmbulance(Transform target)
    {
        if (AmbulanceSystem.Instance.patientIsTarget)
        {
            foreach (var ptnt in StackSystem.Instance.patientsInTheAmbulance)
            {
                ptnt.gameObject.SetActive(false);
            }
        }
        else if (!AmbulanceSystem.Instance.patientIsTarget)
        {
            StackSystem.Instance.patientsInTheAmbulance[StackSystem.Instance.patientsInTheAmbulance.Count-1].gameObject.SetActive(true);
        }
        
        transform.DOJump(stretcherJumpPoint.position, .5f, 1, .3f).OnComplete(() =>
        {
            agent.enabled = true;
            StartCoroutine(GoToTarget(target));
        });
    }

    public void GetInTheAmbulance()
    {
        agent.enabled = false;
        transform.rotation = stretcherInsidePoint.rotation;

        transform.DOJump(stretcherInsidePoint.position, .5f, 1, .4f).OnComplete(() =>
        {
            transform.GetComponentInParent<AmbulanceSystem>().CloseDoors();
            transform.GetComponentInParent<AmbulanceMovement>().isDriveable = true;
        });
    }

    IEnumerator GoToTarget(Transform target)
    {
        agent.SetDestination(target.position);
        
        yield return new WaitWhile(() => ReachedDestinationOrGaveUp(agent) == false);

        if (AmbulanceSystem.Instance.patientIsTarget)
        {
            TakePatient(target.gameObject);
        }
        else
        {
            DropPatient(StackSystem.Instance.patientsInTheAmbulance[StackSystem.Instance.patientsInTheAmbulance.Count-1]);
        }
    }


    public void TakePatient(GameObject patient)
    {
        patient.transform.DOJump(patientPointOnStretcher.position, .5f, 1, .4f).OnComplete(() =>
        {
            patient.transform.rotation = patientPointOnStretcher.rotation;
            patient.transform.SetParent(transform);
            
            StartCoroutine(BackToAmbulance());
            
            currentPatient = patient.GetComponent<PatientInfo>();
            StackSystem.Instance.AddPatient(currentPatient);
            currentPatient = null;
        });
    }
    
    public void DropPatient(PatientInfo patient)
    {
        patient.transform.DOJump(hospitalPoint.position, .5f, 1, .4f).OnComplete(() =>
        {
            StackSystem.Instance.RemovePatient();
            patient.transform.SetParent(hospitalPoint);
            patient.transform.localPosition = new Vector3(0, 0, 0);
            
            StartCoroutine(BackToAmbulance());
        });
    }

    IEnumerator BackToAmbulance()
    {
        agent.SetDestination(stretcherJumpPoint.position);
        yield return new WaitWhile(() => ReachedDestinationOrGaveUp(agent) == false);
        GetInTheAmbulance();
    }

    public bool ReachedDestinationOrGaveUp(NavMeshAgent navMeshAgent)
    {
        if (!navMeshAgent.pathPending)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }

        return false;
    }
}