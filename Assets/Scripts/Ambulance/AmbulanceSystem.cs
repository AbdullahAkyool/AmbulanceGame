using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AmbulanceSystem : MonoBehaviour
{
    public static AmbulanceSystem Instance;

    public bool patientIsTarget = false;
    public bool hospitalIsTarget = false;
    public Transform currentTarget;
    public Transform dropPoint;

    private void Awake()
    {
        Instance = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Patient"))
        {
            if (other.GetComponent<PatientSystem>().isSick)
            {
                GetComponent<AmbulanceMovement>().isDriveable = false;
                other.GetComponent<Collider>().enabled = false;

                patientIsTarget = true;
                hospitalIsTarget = false;

                CurrentTarget(other.transform);
            }
        }

        if (other.CompareTag("HospitalCheckPoint"))
        {
            if (StackSystem.Instance.patientsInTheAmbulance.Count >= 1)
            {
                GetComponent<AmbulanceMovement>().isDriveable = false;
                hospitalIsTarget = true;
            }

            patientIsTarget = false;

            CurrentTarget(dropPoint);
        }
    }

    public Transform CurrentTarget(Transform target)
    {
        currentTarget = target;
        return currentTarget;
    }
}