namespace DarkDescent.Actor.Damage;

public interface IDamageable
{
	public bool CauseHitBounce { get; set; }
	public void TakeDamage( DamageEventData data );
}
