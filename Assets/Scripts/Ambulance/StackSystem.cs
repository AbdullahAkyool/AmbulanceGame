using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public class StackSystem : MonoBehaviour
{
    public static StackSystem Instance;
    public List<PatientInfo> patientsInTheAmbulance = new List<PatientInfo>();

    [Header("--Beds--")] public GameObject hospital;
    public List<BedSystem> Beds = new List<BedSystem>();

    public int index;
    public event Action AmbulanceToStretcherAction;
    public event Action StretcherToAmbulanceAction;

    private void Awake()
    {
        Instance = this;
        index = 0;
    }

    void Start()
    {
        FindBeds();
    }

    public void AmbulanceToStretcherActionMethod()
    {
        AmbulanceToStretcherAction?.Invoke();
        index = 0;
    }

    public void StretcherToAmbulanceActionMethod()
    {
        StretcherToAmbulanceAction?.Invoke();
        index = 0;
    }

    public void FindBeds()
    {
        BedSystem[] bedsArray = hospital.transform.GetChild(0).GetComponentsInChildren<BedSystem>();

        Beds.Clear();

        for (int i = 0; i < bedsArray.Length; i++)
        {
            if (bedsArray[i] != GetComponent<BedSystem>())
            {
                Beds.Add(bedsArray[i]);
            }
        }
    }

    public void AddPatient(PatientInfo patient)
    {
        //patient.gameObject.SetActive(false);
        patientsInTheAmbulance.Add(patient);
    }

    public void RemovePatient(PatientInfo patient)
    {
        if (patientsInTheAmbulance.Count >= 1)
        {
            patientsInTheAmbulance.Remove(patient);
        }
        //patientsInTheAmbulance[patientsInTheAmbulance.Count^1].gameObject.SetActive(true);
    }

    public void AmbulanceToStretcher(Transform patientPointInStretcher)
    {
        StartCoroutine(AmbulanceToStretcherCO(patientPointInStretcher));
    }

    private IEnumerator AmbulanceToStretcherCO(Transform patientPointInStretcher)
    {
        Vector3 jumpPoint = patientPointInStretcher.position;

        for (int i = 0; i < patientsInTheAmbulance.Count; i++)
        {
            patientsInTheAmbulance[i].transform.DOJump(jumpPoint, 2f, 0, 1).OnComplete((() =>
            {
                jumpPoint = new Vector3(jumpPoint.x, jumpPoint.y += .5f, jumpPoint.z);
            }));
            patientsInTheAmbulance[i].transform.parent = patientPointInStretcher;
            index++;
            yield return new WaitForSeconds(.5f);
        }

        if (index == patientsInTheAmbulance.Count)
        {
            AmbulanceToStretcherActionMethod();
        }
    }

    public void StretcherToAmbulance(Transform patientPointInAmbulance)
    {
        StartCoroutine(StretcherToAmbulanceCO(patientPointInAmbulance));
    }

    private IEnumerator StretcherToAmbulanceCO(Transform patientPointInAmbulance)
    {
        Vector3 jumpPoint = patientPointInAmbulance.position;

        for (int i = 0; i < patientsInTheAmbulance.Count; i++)
        {
            patientsInTheAmbulance[i].transform.DOJump(jumpPoint, 2f, 0, 1).OnComplete((() =>
            {
                jumpPoint = new Vector3(jumpPoint.x, jumpPoint.y += .5f, jumpPoint.z);
            }));
            patientsInTheAmbulance[i].transform.parent = patientPointInAmbulance;
            index++;
            yield return new WaitForSeconds(.5f);
        }
        
        if (index == patientsInTheAmbulance.Count)
        {
            StretcherToAmbulanceActionMethod();
        }
    }
}