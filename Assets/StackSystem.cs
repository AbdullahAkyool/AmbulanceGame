using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StackSystem : MonoBehaviour
{
    public static StackSystem Instance;
    public List<PatientInfo> patientsInTheAmbulance = new List<PatientInfo>();

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    public void AddPatient(PatientInfo patient)
    {
        //patient.gameObject.SetActive(false);
        patientsInTheAmbulance.Add(patient);
    }

    public void RemovePatient()
    {
        if (patientsInTheAmbulance.Count >= 1)
        {
            patientsInTheAmbulance.Remove(patientsInTheAmbulance[patientsInTheAmbulance.Count^1]);
        }
        //patientsInTheAmbulance[patientsInTheAmbulance.Count^1].gameObject.SetActive(true);
    }
}
