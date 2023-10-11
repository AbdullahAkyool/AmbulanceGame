using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;


[CreateAssetMenu(fileName = "Patient",menuName = "NewPatient")]
public class PatientSO : ScriptableObject
{
    public string patientName;
    public float health;

    public float GetRandomHealth()
    {
        health = UnityEngine.Random.Range(15f, 95f);
        
        return health;
    }
}
