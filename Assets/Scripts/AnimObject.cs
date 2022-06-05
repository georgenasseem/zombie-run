using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimObject : MonoBehaviour
{
    public static AnimObject animObject;
    public Animator anim;
    public bool playJumpAnim = true;


    public void CheckJump(bool canJump)
    {
        if(playJumpAnim)
        {
            anim.SetBool("inAir", !canJump);

            if(!canJump)
            {
                playJumpAnim = false;
            }
        }
    }

    public void Death()
    {
        anim.SetTrigger("die");
    }
}
