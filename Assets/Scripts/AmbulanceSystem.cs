using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class AmbulanceSystem : MonoBehaviour
{
    public static AmbulanceSystem Instance;
    
    public Transform rightDoor;
    public Transform leftDoor;
    private GameObject currentPatient = null;
    public bool patientIsTarget = false;
    public Transform hospitalPoint;

    private void Awake()
    { 
        Instance = this;
    }

    public void OpenDoors()
    {
        rightDoor.DOLocalRotate(new Vector3(0f, -110f, 0f), 2f);
        leftDoor.DOLocalRotate(new Vector3(0f, 110f, 0f), 2f).OnComplete(() =>
        {
            if (patientIsTarget)
            {
                StretcherMovement.Instance.GetOutOfTheAmbulance(currentPatient.transform);
            }
            else
            {
                StretcherMovement.Instance.GetOutOfTheAmbulance(hospitalPoint.transform);
            }
        });
    }

    public void CloseDoors()
    {
        rightDoor.DOLocalRotate(new Vector3(0f, 0f, 0f), 2f);
        leftDoor.DOLocalRotate(new Vector3(0f, 0f, 0f), 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Patient"))
        {
            patientIsTarget = true;
            currentPatient = other.gameObject;
            GetComponent<AmbulanceMovement>().isDriveable = false;
            OpenDoors();
            other.GetComponent<Collider>().enabled = false;
        }
        if (other.CompareTag("HospitalCheckPoint"))
        {
            patientIsTarget = false;
            OpenDoors();
        }
    }
}