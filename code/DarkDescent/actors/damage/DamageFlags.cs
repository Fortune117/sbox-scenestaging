using System;

namespace DarkDescent.Actor.Damage;

[Flags]
public enum DamageFlags
{
	None = 0,
	IgnoreResistance = 1,
	DamageOverTime = 2,
	NoFlinch = 4,
}
