using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class NewStretcherSystem : MonoBehaviour
{
    public static NewStretcherSystem Instance;

    [Header("--States--")] public bool isIdle;
    public bool isGetOut;
    public bool isPatient;
    public bool isHospital;
    public bool isBack;
    public bool isGetIn;

    private NavMeshAgent agent;

    [Header("--Ambulance Doors--")] public Transform rightDoor;
    public Transform leftDoor;

    [Header("--Event Points--")] public Transform stretcherInsidePoint;
    public Transform stretcherJumpPoint;
    public Transform patientPointOnStretcher;
    public Transform hospitalPoint;
    public Transform bedPoint;


    [Header("--Current Patient--")] public PatientInfo currentPatient = null;

    public GameObject willDropPatient;

    [Header("--Doctor--")] private Doctor doctor;

    private void Awake()
    {
        //StackSystem.Instance.AmbulanceToStretcherAction += GetOut2;
        
        Instance = this;
    }

    private void Start()
    {
        doctor = FindObjectOfType<Doctor>();

        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;

        isIdle = true;
    }

    public void Idle()
    {
        doctor.Idle();
        
        if (AmbulanceSystem.Instance.patientIsTarget || AmbulanceSystem.Instance.hospitalIsTarget)
        {
            isIdle = false;
            isGetOut = true;
        }
    }

    public void GetOut()
    {
        doctor.Jump();

        rightDoor.DOLocalRotate(new Vector3(0f, -110f, 0f), 2f);
        leftDoor.DOLocalRotate(new Vector3(0f, 110f, 0f), 2f).OnComplete(() =>
        {
            transform.DOJump(stretcherJumpPoint.position, .5f, 1, .3f).OnComplete(() =>
            {
                if (AmbulanceSystem.Instance.patientIsTarget)
                {
                    agent.enabled = true;

                    isPatient = true;
                    isGetOut = false;
                }
                else if (AmbulanceSystem.Instance.hospitalIsTarget)
                {
                    willDropPatient = StackSystem.Instance.patientsInTheAmbulance[StackSystem.Instance.patientsInTheAmbulance.Count - 1].gameObject;//IIIGGGHHHH
                    willDropPatient.transform.GetChild(0).transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().enabled = true; //:(((((
                    
                    willDropPatient.transform.parent = patientPointOnStretcher;
                    willDropPatient.transform.localPosition = Vector3.zero;
                    willDropPatient.transform.localRotation = Quaternion.Euler(Vector3.zero);

                    agent.enabled = true;

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
        if (currentPatient != null) currentPatient.transform.localRotation = stretcherInsidePoint.rotation;

        doctor.Jump();
        
        transform.DOJump(stretcherInsidePoint.position, .5f, 1, .4f).OnComplete(() =>
        {
            rightDoor.DOLocalRotate(new Vector3(0f, 0f, 0f), 2f);
            leftDoor.DOLocalRotate(new Vector3(0f, 0f, 0f), 2f).OnComplete(() =>
            {
                if (StackSystem.Instance.patientsInTheAmbulance.Count >= 1 && AmbulanceSystem.Instance.hospitalIsTarget)
                {
                    AmbulanceSystem.Instance.currentTarget = hospitalPoint;
                    isGetIn = false;
                    isGetOut = true;
                }
                else
                {
                    transform.GetComponentInParent<AmbulanceMovement>().isDriveable = true;
                    AmbulanceSystem.Instance.currentTarget = null;
                    currentPatient.transform.parent = stretcherInsidePoint;
                    
                    currentPatient = null;
                    isGetIn = false;
                    isIdle = true;
                    AmbulanceSystem.Instance.hospitalIsTarget = false;
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
        doctor.Walk();

        currentPatient = AmbulanceSystem.Instance.currentTarget.GetComponent<PatientInfo>();
        agent.SetDestination(currentPatient.transform.position);

        yield return new WaitWhile(() => ReachedDestinationOrGaveUp(agent) == false);

        doctor.Idle();
        
        currentPatient.transform.DOJump(patientPointOnStretcher.position, .5f, 1, .4f).OnComplete(() =>
        {
            currentPatient.GetComponent<PatientSystem>().isPatientInAmbulance = true;

            currentPatient.transform.rotation = patientPointOnStretcher.rotation;
            currentPatient.transform.SetParent(transform);

            StackSystem.Instance.AddPatient(currentPatient);
            AmbulanceSystem.Instance.patientIsTarget = false;

            isBack = true;
            isPatient = false;
        });
    }

    public void DropPatient()
    {
        StartCoroutine(DropPatientCO());
    }

    public IEnumerator DropPatientCO()
    {
        for (int i = 0; i < StackSystem.Instance.Beds.Count; i++)
        {
            if (StackSystem.Instance.Beds[i].isEmpty)
            {
                bedPoint = StackSystem.Instance.Beds[i].transform.GetChild(1);
                StackSystem.Instance.Beds[i].isEmpty = false;
                break;
            }
        }

        if (StackSystem.Instance.patientsInTheAmbulance.Count >= 1 && bedPoint != null)
        {
            PatientInfo patient =
                StackSystem.Instance.patientsInTheAmbulance[StackSystem.Instance.patientsInTheAmbulance.Count - 1];

            Transform target = AmbulanceSystem.Instance.currentTarget;
            agent.SetDestination(target.position);
            
            doctor.Walk();

            yield return new WaitWhile(() => ReachedDestinationOrGaveUp(agent) == false);

            doctor.Idle();

            patient.transform.DOJump(bedPoint.position, .5f, 1, .4f).OnComplete(() =>
            {
                patient.GetComponent<PatientSystem>().isPatientInHospital = true;

                patient.transform.SetParent(bedPoint);
                patient.transform.localPosition = new Vector3(0, 0, 0);
                StackSystem.Instance.RemovePatient(patient);
                bedPoint = null;

                //AmbulanceSystem.Instance.hospitalIsTarget = false;

                isBack = true;
                isHospital = false;
            });
        }
        else
        {
            isBack = true;
            isHospital = false;

            AmbulanceSystem.Instance.hospitalIsTarget = false;
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
        
        doctor.Walk();

        yield return new WaitWhile(() => ReachedDestinationOrGaveUp(agent) == false);
        
        doctor.Idle();

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