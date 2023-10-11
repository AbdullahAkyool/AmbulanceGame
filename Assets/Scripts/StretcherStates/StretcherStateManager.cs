using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StretcherStateManager : MonoBehaviour
{
    public NewStretcherSystem newStretcherSystem;
    public Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void SetStateTransition()
    {
        anim.SetBool("isIdle", newStretcherSystem.isIdle);
        anim.SetBool("isGetOut", newStretcherSystem.isGetOut);
        anim.SetBool("isPatient", newStretcherSystem.isPatient);
        anim.SetBool("isHospital", newStretcherSystem.isHospital);
        anim.SetBool("isBack", newStretcherSystem.isBack);
        anim.SetBool("isGetIn", newStretcherSystem.isGetIn);
    }

    public void ResetAllConditions()
    {
        anim.SetBool("isIdle", false);
        anim.SetBool("isGetOut", false);
        anim.SetBool("isPatient", false);
        anim.SetBool("isHospital", false);
        anim.SetBool("isBack", false);
        anim.SetBool("isGetIn", false);
    }
}
