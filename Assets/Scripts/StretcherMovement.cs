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

    public GameObject stretcherMesh;
    public bool isDropping;

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
            StackSystem.Instance.patientsInTheAmbulance[StackSystem.Instance.patientsInTheAmbulance.Count - 1]
                .gameObject.SetActive(true);
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
            //transform.GetComponentInParent<AmbulanceSystem>().CloseDoors();
            transform.GetComponentInParent<AmbulanceMovement>().isDriveable = true;

            AmbulanceSystem.Instance.currentTarget = null;
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
            DropPatient();
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

    public void DropPatient()
    {
        isDropping = true;
        
        if (StackSystem.Instance.patientsInTheAmbulance.Count >= 1)
        {
            for (int i = 0; i < StackSystem.Instance.patientsInTheAmbulance.Count; i++)
            {
                PatientInfo patient = StackSystem.Instance.patientsInTheAmbulance[i];

                patient.transform.DOJump(hospitalPoint.position, .5f, 1, .4f).OnComplete(() =>
                {
                    StackSystem.Instance.RemovePatient(patient);
                    patient.transform.SetParent(hospitalPoint);
                    patient.transform.localPosition = new Vector3(0, 0, 0);

                    StartCoroutine(BackToAmbulance());
                });
            }
        }
    }

    IEnumerator BackToAmbulance()
    {
        AmbulanceSystem.Instance.currentTarget = stretcherJumpPoint;

        agent.SetDestination(AmbulanceSystem.Instance.currentTarget.position);
        yield return new WaitWhile(() => ReachedDestinationOrGaveUp(agent) == false);

        if (!isDropping)
        {
            GetInTheAmbulance();
        }
        else if (isDropping)
        {
            stretcherMesh.transform.DOJump(stretcherInsidePoint.position, .5f, 1, .4f).OnComplete(() =>
            {
                stretcherMesh.transform.DOJump(stretcherJumpPoint.position, .5f, 1, .3f);
            });
        }
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