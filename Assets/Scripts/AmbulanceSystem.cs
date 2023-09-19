using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;

public class AmbulanceSystem : MonoBehaviour
{
    public Transform rightDoor;
    public Transform leftDoor;
    private GameObject currentPatient = null;
    
    public void OpenDoors()
    {
        rightDoor.DOLocalRotate(new Vector3(0f, -110f, 0f), 2f);
        leftDoor.DOLocalRotate(new Vector3(0f, 110f, 0f), 2f).OnComplete(() =>
        {
            StretcherMovement.Instance.GetOutOfTheAmbulance(currentPatient.GetComponent<PatientInfo>());
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
            currentPatient = other.gameObject;
            GetComponent<AmbulanceMovement>().isDriveable = false;
            OpenDoors();
            other.GetComponent<Collider>().enabled = false;
        }
    }
}