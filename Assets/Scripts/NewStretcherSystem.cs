using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class NewStretcherSystem : MonoBehaviour
{
    public static NewStretcherSystem Instance;

    private NavMeshAgent agent;

    public Transform rightDoor;
    public Transform leftDoor;

    public Transform stretcherInsidePoint;
    public Transform stretcherJumpPoint;
    public Transform patientPointOnStretcher;
    public Transform hospitalPoint;

    public PatientInfo currentPatient = null;

    public bool isIdle;
    public bool isGetOut;
    public bool isPatient;
    public bool isHospital;
    public bool isBack;
    public bool isGetIn;

    private void Awake()
    {
        Instance = this;

        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
        
        isIdle = true;
    }

    public void Idle()
    {
        if (AmbulanceSystem.Instance.patientIsTarget || AmbulanceSystem.Instance.hospitalIsTarget)
        {
            isIdle = false;
            isGetOut = true;
        }
    }

    public void GetOut()
    {
        rightDoor.DOLocalRotate(new Vector3(0f, -110f, 0f), 2f);
        leftDoor.DOLocalRotate(new Vector3(0f, 110f, 0f), 2f).OnComplete(() =>
        {
            transform.DOJump(stretcherJumpPoint.position, .5f, 1, .3f).OnComplete(() =>
            {
                agent.enabled = true;

                if (AmbulanceSystem.Instance.patientIsTarget)
                {
                    //StartCoroutine(TakePatient());
                    isPatient = true;
                    isGetOut = false;
                }
                else if(AmbulanceSystem.Instance.hospitalIsTarget)
                {
                    //StartCoroutine(DropPatient());
                    isHospital = true;
                    isGetOut = false;
                }
            });
        });
    }

    public void GetIn()
    {
        agent.enabled = false;
        transform.rotation = stretcherInsidePoint.rotation;

        transform.DOJump(stretcherInsidePoint.position, .5f, 1, .4f).OnComplete(() =>
        {
            rightDoor.DOLocalRotate(new Vector3(0f, 0f, 0f), 2f);
            leftDoor.DOLocalRotate(new Vector3(0f, 0f, 0f), 2f).OnComplete(() =>
            {
                transform.GetComponentInParent<AmbulanceMovement>().isDriveable = true;
                AmbulanceSystem.Instance.currentTarget = null;

                if (StackSystem.Instance.patientsInTheAmbulance.Count >= 1 && AmbulanceSystem.Instance.hospitalIsTarget)
                {
                    isGetIn = false;
                    isGetOut = true;
                }
                else
                {
                    isGetIn = false;
                    isIdle = true;
                }
            });
        });
    }

    public void TakePatient()
    {
        StartCoroutine(TakePatientCO());
    } 

    public IEnumerator TakePatientCO()
    {
        currentPatient = AmbulanceSystem.Instance.currentTarget.GetComponent<PatientInfo>();
        agent.SetDestination(currentPatient.transform.position);

        yield return new WaitWhile(() => ReachedDestinationOrGaveUp(agent) == false);


        currentPatient.transform.DOJump(patientPointOnStretcher.position, .5f, 1, .4f).OnComplete(() =>
        {
            currentPatient.transform.rotation = patientPointOnStretcher.rotation;
            currentPatient.transform.SetParent(transform);
            StackSystem.Instance.AddPatient(currentPatient);
            AmbulanceSystem.Instance.patientIsTarget = false;
            
            //StartCoroutine(GetBack());

            isBack = true;
            isPatient = false;
            
            currentPatient = null;
        });
    }

    public void DropPatient()
    {
        StartCoroutine(DropPatientCO());
    }
    
    public IEnumerator DropPatientCO()
    {
        if (StackSystem.Instance.patientsInTheAmbulance.Count >= 1)
        {
            PatientInfo patient =
                StackSystem.Instance.patientsInTheAmbulance[StackSystem.Instance.patientsInTheAmbulance.Count - 1];
            
            Transform target = AmbulanceSystem.Instance.currentTarget;
            agent.SetDestination(target.position);

            yield return new WaitWhile(() => ReachedDestinationOrGaveUp(agent) == false);

            patient.transform.DOJump(hospitalPoint.position, .5f, 1, .4f).OnComplete(() =>
            {
                patient.transform.SetParent(hospitalPoint);
                patient.transform.localPosition = new Vector3(0, 0, 0);
                StackSystem.Instance.RemovePatient(patient);
                //AmbulanceSystem.Instance.hospitalIsTarget = false;

                //StartCoroutine(GetBack());

                isBack = true;
                isHospital = false;
            });
        }
    }

    public void GetBack()
    {
        StartCoroutine(GetBackCO());
    }
    
    public IEnumerator GetBackCO()
    {
        AmbulanceSystem.Instance.CurrentTarget(stretcherJumpPoint);

        agent.SetDestination(AmbulanceSystem.Instance.currentTarget.position);

        yield return new WaitWhile(() => ReachedDestinationOrGaveUp(agent) == false);

        //GetIn();

        isGetIn = true;
        isBack = false;
    }

    public bool ReachedDestinationOrGaveUp(NavMeshAgent navMeshAgent)
    {
        if (!navMeshAgent.pathPending)
        {
            if (navMeshAgent.enabled && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
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