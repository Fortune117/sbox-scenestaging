using Sandbox;

namespace DarkDescent.Actor.StatSystem;

public struct Combat
{
	public Combat() { }

	/// <summary>
	/// Our base physical power, improves the effectiveness of our physical attacks.
	/// Increased by our primary attribute (usually Str/Dex).
	/// </summary>
	[Range( 0, 300f, 1f )]
	public float PhysicalPower { get; set; } = 0f;
	
	/// <summary>
	/// Our base spell power, improves the effectiveness of our spells.
	/// Increased by our spell casting attribute (usually Will/Cha/Int).
	/// </summary>
	[Range( 0, 300f, 1f )]
	public float SpellPower { get; set; } = 0f;
	
		
	/// <summary>
	/// Improves the effectiveness of abilities that scale off FirePower.
	/// Doesn't scale with anything.
	/// </summary>
	[Range( 0, 300f, 1f )]
	public float FirePower { get; set; } = 0f;
	
	/// <summary>
	/// Improves the effectiveness of abilities that scale off FrostPower.
	/// Doesn't scale with anything.
	/// </summary>
	[Range( 0, 300f, 1f )]
	public float FrostPower { get; set; } = 0f;
	
	/// <summary>
	/// Improves the effectiveness of abilities that scale off ElectricPower.
	/// Doesn't scale with anything.
	/// </summary>
	[Range( 0, 300f, 1f )]
	public float ElectricPower { get; set; } = 0f;
	
	/// <summary>
	/// Improves the effectiveness of abilities that scale off PoisonPower.
	/// Doesn't scale with anything.
	/// </summary>
	[Range( 0, 300f, 1f )]
	public float PoisonPower { get; set; } = 0f;
	
	/// <summary>
	/// Improves the effectiveness of abilities that scale off NecroticPower.
	/// Doesn't scale with anything.
	/// </summary>
	[Range( 0, 300f, 1f )]
	public float NecroticPower { get; set; } = 0f;
	
	/// <summary>
	/// Improves the effectiveness of abilities that scale off ArcanePower.
	/// Doesn't scale with anything.
	/// </summary>
	[Range( 0, 300f, 1f )]
	public float ArcanePower { get; set; } = 0f;
	
	/// <summary>
	/// Improves the effectiveness of abilities that scale off DivinePower.
	/// Doesn't scale with anything.
	/// </summary>
	[Range( 0, 300f, 1f )]
	public float DivinePower { get; set; } = 0f;
	
	/// <summary>
	/// Improves the effectiveness of abilities that scale off OccultPower.
	/// Doesn't scale with anything.
	/// </summary>
	[Range( 0, 300f, 1f )]
	public float OccultPower { get; set; } = 0f;

	/// <summary>
	/// How much armor we ignore when attacking.
	/// </summary>
	[Range( 0, 300, 1f )]
	public float ArmorPenetration { get; set; } = 0;

	/// <summary>
	/// How much warding we ignore when attacking.
	/// </summary>
	[Range( 0, 300, 1f )]
	public float WardingPenetration { get; set; } = 0;
	
	/// <summary>
	/// How much fire resistance we ignore when attacking.
	/// </summary>
	[Range( 0, 300, 1f )]
	public float FirePenetration { get; set; } = 0;
	
	/// <summary>
	/// How much frost resistance we ignore when attacking.
	/// </summary>
	[Range( 0, 300, 1f )]
	public float FrostPenetration { get; set; } = 0;
	
	/// <summary>
	/// How much electric resistance we ignore when attacking.
	/// </summary>
	[Range( 0, 300, 1f )]
	public float ElectricPenetration { get; set; } = 0;
	
	/// <summary>
	/// How much poison resistance we ignore when attacking.
	/// </summary>
	[Range( 0, 300, 1f )]
	public float PoisonPenetration { get; set; } = 0;
	
	/// <summary>
	/// How much necrotic resistance we ignore when attacking.
	/// </summary>
	[Range( 0, 300, 1f )]
	public float NecroticPenetration { get; set; } = 0;
	
	/// <summary>
	/// How much arcane resistance we ignore when attacking.
	/// </summary>
	[Range( 0, 300, 1f )]
	public float ArcanePenetration { get; set; } = 0;
	
	/// <summary>
	/// How much divine resistance we ignore when attacking.
	/// </summary>
	[Range( 0, 300, 1f )]
	public float DivinePenetration { get; set; } = 0;
	
	/// <summary>
	/// How much occult resistance we ignore when attacking.
	/// </summary>
	[Range( 0, 300, 1f )]
	public float OccultPenetration { get; set; } = 0;

	/// <summary>
	/// How much our damage is multiplied when we crit.
	/// </summary>
	[Range( 0, 5, 1f )]
	public float CriticalMultiplier { get; set; } = 1.5f;
	
	/// <summary>
	/// How much knockback we apply on each hit.
	/// </summary>
	[Range(0, 300, 1f)]
	public float KnockBack { get; set; } = 0;
}
