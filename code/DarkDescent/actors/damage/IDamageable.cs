namespace DarkDescent.Actor.Damage;

public interface IDamageable
{
	public GameObject GameObject { get; }
	public bool CauseHitBounce { get; set; }
	public void TakeDamage( DamageEventData data );
}
