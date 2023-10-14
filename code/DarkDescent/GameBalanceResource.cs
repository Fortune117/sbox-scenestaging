using Sandbox;

namespace DarkDescent;

[GameResource("Game Balance", "balance", "Contains data about how certain core gameplay features work.")]
public class GameBalanceResource : GameResource
{
	public static GameBalanceResource ActiveBalance => ResourceLibrary.Get<GameBalanceResource>("data/balance/default.balance");

	[Category("Combat Balance"), Range(0, 3, 0.25f)]
	public float PlayerDamageMultiplier { get; set; } = 1;
	
	[Category("Combat Balance"), Range(0, 3, 0.25f)]
	public float EnemyDamageMultiplier { get; set; } = 1;

	[Category( "Combat Balance" )] 
	public bool FriendlyFire { get; set; } = false;

	[Category( "Combat Balance" ), ShowIf( nameof(FriendlyFire), true ), Range( 0, 2f, 0.1f )]
	public float FriendlyFireDamageMultiplier { get; set; } = 0.5f;
	
	[Category( "Progression Balance" ), Range(0, 2f, 0.1f)] 
	public float ExperienceMultiplier { get; set; }
	
	/// <summary>
	/// Our curve for how much our damage resistance stats should actually multiply damage by.
	/// Balanced from -300 - 300.
	/// </summary>
	[Category("Combat Balance")]
	public Curve ResistanceScalingCurve { get; set; }
	
	//Strength Curves
	[Category("Strength Scaling")]
	public Curve StrengthToStaminaCostMultiplierCurve { get; set; }
	
	[Category("Strength Scaling")]
	public Curve StrengthToCarryWeightCurve { get; set; }
	
	[Category("Strength Scaling")]
	public Curve StrengthToJumpHeightCurve { get; set; }
	
	[Category("Strength Scaling")]
	public Curve StrengthToKnockBackCurve { get; set; }
	
	//Dexterity Curves
	[Category("Dexterity Scaling")]
	public Curve DexterityToStaminaRegenCurve{ get; set; }
	
	[Category("Dexterity Scaling")]
	public Curve DexterityToActionSpeedCurve { get; set; }
	
	[Category("Dexterity Scaling")]
	public Curve DexterityToMoveSpeedCurve { get; set; }
	
	[Category("Dexterity Scaling"), Title("Stamina Regen Delay")]
	public Curve DexterityToStaminaRegenDelayCurve { get; set; }
	
	//Constitution curves
	[Category("Constitution Scaling")]
	public Curve ConstitutionToMaxHealthCurve { get; set; }
	
	[Category("Constitution Scaling")]
	public Curve ConstitutionToHealthRegenCurve { get; set; }
	
	[Category("Constitution Scaling")]
	public Curve ConstitutionToHealMultiplierCurve { get; set; }
	
	[Category("Constitution Scaling")]
	public Curve ConstitutionToMaxStaminaCurve { get; set; }
	
	[Category("Constitution Scaling")]
	public Curve ConstitutionToHealthRegenDelayCurve { get; set; }
	
	//Primary Attribute
	[Category("Primary Attribute Scaling")]
	public Curve PrimaryAttributeToPhysicalPowerCurve { get; set; }
	
	//Magic
	[Category("Spell Casting Attribute Scaling"), Title("SCA To Spell Power")]
	public Curve SpellCastingAttributeToSpellPowerCurve { get; set; }
	
	[Category("Spell Casting Attribute Scaling"), Title("SCA To Spell Casting Speed")]
	public Curve SpellCastingAttributeToSpellCastingSpeedCurve { get; set; }
}
