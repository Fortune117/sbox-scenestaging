using System;

namespace DarkDescent.Actor.Damage;

[Flags]
public enum DamageType
{
	None = 0,
	Physical = 1,
	Magical = 2,
	
	Fire = 4,
	Frost = 8,
	Electric = 16,
	Poison = 32,
	Necrotic = 64,
	
	Arcane = 128,
	Divine = 256,
	Occult = 512,
	
	Absolute = 1024,
	Chaotic = 2048
}
