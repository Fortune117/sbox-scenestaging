using DarkDescent.Actor.Damage;

namespace DarkDescent.Actor.Marker;

public interface IDamageDealtListener
{
	void OnDamageDealt( DamageEventData damageEvent, bool isLethal );
}
