using UnityEngine;
using UnityEngine.VFX;

public class EnemyVFXManager : MonoBehaviour
{
    public VisualEffect FootStep;
    public VisualEffect AttackVFX;
    public ParticleSystem BeingHitVFX;
    public VisualEffect BeingHitSplashVFX;
    

    public void PlayerAttackVFX()
    {
        AttackVFX.SendEvent("OnPlay");
    }

    public void BurstFootStep()
    {
        FootStep.SendEvent("OnPlay");
    }

    public void PlayBeingHitVFX(Vector3 attackerPos)
    {
        Vector3 forceForward = transform.position - attackerPos;
        forceForward.Normalize();
        forceForward.y = 0;
        BeingHitVFX.transform.rotation = Quaternion.LookRotation(forceForward);
        BeingHitVFX.Play();

        Vector3 splashPos = transform.position;
        splashPos.y += 2f;
        VisualEffect newSplashVFS = Instantiate(BeingHitSplashVFX, splashPos, Quaternion.identity);
        newSplashVFS.SendEvent("OnPlay");
        Destroy(newSplashVFS.gameObject,10f);
    }
}
