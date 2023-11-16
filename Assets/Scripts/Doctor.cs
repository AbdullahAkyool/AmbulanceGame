using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doctor : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Idle()
    {
        anim.SetBool("isIdle",true);
        anim.SetBool("isWalk",false);
        anim.SetBool("isJump",false);
    }

    public void Walk()
    {
        anim.SetBool("isIdle",false);
        anim.SetBool("isWalk",true);
        anim.SetBool("isJump",false);
    }

    public void Jump()
    {
        anim.SetBool("isIdle",false);
        anim.SetBool("isWalk",false);
        anim.SetBool("isJump",true);
    }
}
