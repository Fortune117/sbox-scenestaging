using DarkDescent.Actor.Damage;

namespace DarkDescent.Actor.Marker;

public interface IDeathListener
{
	void OnDeath( DamageEventData damageEventData );
}
