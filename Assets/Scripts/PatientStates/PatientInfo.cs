using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PatientInfo : MonoBehaviour
{
    public PatientSO patientSo;
    private string patientName;
    public Image healthBar;
    public float health;
    public float maxHealth = 100;
    private float lerpSpeed;

    void Start()
    {
        patientName = patientSo.patientName;
        //health = patientSo.GetRandomHealth();
        health = maxHealth;
    }
    
    void Update()
    {
        HealthBarFilled();
    }

    private void LateUpdate()
    {
        healthBar.transform.rotation = Camera.main.transform.rotation;
    }

    public void HealthBarFilled()
    {
        if (health > maxHealth) health = 100;
        if (health < 0) health = 0;
        lerpSpeed = 3f * Time.deltaTime;
        
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, health / maxHealth, lerpSpeed);
        ColorChanger();
    }

    public void ColorChanger()
    {
        Color healthColor = Color.Lerp(Color.red, Color.green, (health / maxHealth));
        healthBar.color = healthColor;
    }
}
