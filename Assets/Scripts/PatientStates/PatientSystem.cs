using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PatientSystem : MonoBehaviour
{
    [Header("--States--")]
    public bool isIdle;
    public bool isSick;
    public bool isInAmbulance;
    public bool isAtHospital;
    public bool isHealthy;
    public bool isDead;

    [Header("--Current Place--")]
    public bool isPatientInAmbulance;
    public bool isPatientInHospital;

    [Header("--Health--")]
    public Animator patientAnim;
    private float timer;
    private Transform healEffect;
    private Transform deadEffect;
    
    private void Awake()
    {
        patientAnim = transform.GetChild(0).GetComponent<Animator>();
        healEffect = transform.GetChild(2);
        deadEffect = transform.GetChild(3);
    }

    public void Idle()
    {
        //patientAnim.SetFloat("animSpeed",-1);
        
        patientAnim.SetBool("isFalling", false);
        patientAnim.SetBool("isSick", false);
        patientAnim.SetBool("isIdle", true);
        
        StartCoroutine(PatientRandomHealth());
        
        //patientAnim.SetFloat("animSpeed",1);
    }

    private IEnumerator PatientRandomHealth()
    {
        yield return new WaitForSeconds(Random.Range(5, 10));
        
        GetComponent<PatientInfo>().health = GetComponent<PatientInfo>().patientSo.GetRandomHealth();

        isIdle = false;
        isSick = true;
    }

    public void Sick()
    {
        //patient infodan gelen aciliyet bilgisine göre can azalıyor

        DecreaseHealth(5f);
        
        patientAnim.SetBool("isFalling", false);
        patientAnim.SetBool("isSick", true);
        patientAnim.SetBool("isIdle", false);
        
        if (GetComponent<PatientInfo>().health <= 0)
        {
            isSick = false;
            isDead = true;
        }

        if (isPatientInAmbulance)
        {
            isSick = false;
            isInAmbulance = true;
        }
    }

    public void InTheAmbulance()
    {
        //ambulans leveline göre ya da sabit olarak can yavaşça azalıyor

        IncreaseHealth(2f);
        
        healEffect.gameObject.SetActive(true);

        if (isPatientInHospital)
        {
            isInAmbulance = false;
            isAtHospital = true;
            healEffect.gameObject.SetActive(false);
        }
    }

    public void AtTheHospital()
    {
        //hastane leveline göre ya da sabit olarak can artıyor

        IncreaseHealth(2f);

        isPatientInAmbulance = false;
        
        healEffect.gameObject.SetActive(true);

        if (GetComponent<PatientInfo>().health >= 100)
        {
            GetComponent<PatientInfo>().health = 100;
            isAtHospital = false;
            isHealthy = true;
            healEffect.gameObject.SetActive(false);
        }
    }

    public void Healthy()
    {
        //can 100 olduktan sonra hastaneden çıkış

        Debug.Log("HEALTHY");
        isPatientInHospital = false;

        isHealthy = false;
        isIdle = true;
        
        GetComponent<Collider>().enabled = true;
    }

    public void Dead()
    {
        //can 0 olduktan sonra materiyal ve diğer özellikler kapatılıyor
    
        Debug.Log("DEAD");

        deadEffect.gameObject.SetActive(true);
        isPatientInHospital = false;
        
        GetComponent<Collider>().enabled = false;
    }
    
    private void IncreaseHealth(float value)
    {
        timer += Time.deltaTime;

        if (timer >= 3f)
        {
            if (GetComponent<PatientInfo>().health < GetComponent<PatientInfo>().maxHealth)
                GetComponent<PatientInfo>().health += value;
            timer = 0f;
        }
    }

    private void DecreaseHealth(float value)
    {
        timer += Time.deltaTime;

        if (timer >= 3f)
        {
            if (GetComponent<PatientInfo>().health > 0) GetComponent<PatientInfo>().health -= value;
            timer = 0f;
        }
    }
}