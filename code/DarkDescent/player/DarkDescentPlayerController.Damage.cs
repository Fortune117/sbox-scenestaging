using DarkDescent.Actor.Damage;
using DarkDescent.Actor.Marker;
using DarkDescent.Weapons;
using Sandbox;

namespace DarkDescent;

public partial class DarkDescentPlayerController : IDamageTakenListener
{
	[Property] public AttackBlockerComponent BlockerComponent { get; set; }
	
	
	public void OnBlock(DamageEventData damageEvent, bool isParry)
	{
		Body.Set( "bBlockImpact", true );
		
		foreach ( var blockListener in GetComponents<IBlockListener>(true, true) )
		{
			blockListener.OnBlock( damageEvent, isParry );	
		}
	}

	public void OnDamageTaken( DamageEventData damageEventData, bool isLethal )
	{
		if ( isLethal )
			return;
		
		//InterruptAttack(); 

		ApplyKnockBack( damageEventData );
	}

	private void ApplyKnockBack( DamageEventData damageEventData )
	{
		var knockback = damageEventData.Direction * damageEventData.KnockBackResult;
		CharacterController.Punch( knockback );
	}
}
