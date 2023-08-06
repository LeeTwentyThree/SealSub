namespace SealSubMod.MonoBehaviours;

internal class SealDamageManager : MonoBehaviour, IOnTakeDamage
{
    public void OnTakeDamage(DamageInfo damageInfo)
    {
        if (damageInfo.damage <= 0f)
        {
            return;
        }
        if (damageInfo.type == DamageType.Normal || damageInfo.type == DamageType.Electrical)
        {
            BroadcastMessage("OnTakeCreatureDamage", null, SendMessageOptions.DontRequireReceiver);
        }
        else if (damageInfo.type == DamageType.Collide)
        {
            BroadcastMessage("OnTakeCollisionDamage", damageInfo.damage, SendMessageOptions.DontRequireReceiver);
        }
    }
}
