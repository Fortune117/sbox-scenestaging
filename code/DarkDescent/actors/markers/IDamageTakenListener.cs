using DarkDescent.Actor.Damage;

namespace DarkDescent.Actor.Marker;

public interface IDamageTakenListener
{
	void OnDamageTaken( DamageEventData damageEvent, bool isLethal );
}
