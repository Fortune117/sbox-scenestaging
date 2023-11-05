using DarkDescent.Actor.Damage;

namespace DarkDescent.Actor.Marker;

public interface IBlockListener
{
	void OnBlock( DamageEventData damageEvent, bool isParry );
}
