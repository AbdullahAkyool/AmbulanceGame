using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatientStateManager : MonoBehaviour
{
    public PatientSystem patientSystem;
    public Animator anim;
    void Start()
    {
        patientSystem = GetComponent<PatientSystem>();
        anim = GetComponent<Animator>();
    }
    
    public void SetStateTransition()
    {
        anim.SetBool("isIdle", patientSystem.isIdle);
        anim.SetBool("isSick", patientSystem.isSick);
        anim.SetBool("isInAmbulance", patientSystem.isInAmbulance);
        anim.SetBool("isAtHospital", patientSystem.isAtHospital);
        anim.SetBool("isHealthy", patientSystem.isHealthy);
        anim.SetBool("isDead", patientSystem.isDead);
    }

    public void ResetAllConditions()
    {
        anim.SetBool("isIdle", false);
        anim.SetBool("isSick", false);
        anim.SetBool("isInAmbulance", false);
        anim.SetBool("isAtHospital", false);
        anim.SetBool("isHealthy", false);
        anim.SetBool("isDead", false);
    }
}
