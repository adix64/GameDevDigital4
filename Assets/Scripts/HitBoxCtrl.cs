using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxCtrl : MonoBehaviour
{
    public string opponentHurtbox;
    public int damage = 5;
    public float coolOffTime = 0.3333f; // la cat timp poti lovi din nou
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(opponentHurtbox))
        {//are loc coliziunea intre hitbox si opponentHurtbox
            Animator animator = other.transform.parent.GetComponentInParent<Animator>();
            if (animator.GetFloat("timeSinceTakenHit") > coolOffTime)
            {
                animator.Play("TakeHit");//oponentul va suferi
                animator.SetInteger("TakenDamage", damage); //un damage
            }
        }
    }
}
