using DarkDescent.Actor.Damage;

namespace DarkDescent.Actor.Marker;

public interface IDamageTakenListener
{
	void OnDamageTaken( DamageEventData damageEventData, bool isLethal );
}
