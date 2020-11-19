using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxCtrl : MonoBehaviour
{
    public string opponentHurtbox;
    public int damage = 5;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(opponentHurtbox))
        {//are loc coliziunea intre hitbox si opponentHurtbox
            Animator animator = other.transform.parent.GetComponentInParent<Animator>();
            animator.Play("TakeHit");//oponentul va suferi
            animator.SetInteger("TakenDamage", damage); //un damage
        }
    }
}
