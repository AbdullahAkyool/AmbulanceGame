using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StackSystem : MonoBehaviour
{
    public static StackSystem Instance;
    public List<PatientInfo> patientsInTheAmbulance = new List<PatientInfo>();
    
    [Header("--Beds--")] 
    public GameObject hospital;
    public List<BedSystem> Beds = new List<BedSystem>();

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
       FindBeds();
    }
    
    void Update()
    {
        
    }

    public void FindBeds()
    {
        BedSystem[] bedsArray  = hospital.transform.GetChild(0).GetComponentsInChildren<BedSystem>();
        
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
}
