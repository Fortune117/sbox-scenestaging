using DarkDescent.Actor.StatSystem;
using Sandbox;

namespace DarkDescent.Actor.StatusEffects;

public class StatModifierEffect : StatusEffectComponent
{
	[Property, ToggleGroup("Stat Modifier")]
	public StatModifierResource StatModifierResource { get; set; }
	
	[Property, ToggleGroup("Stat Modifier")]
	public bool UseStacks { get; set; }

	private StatModifier StatModifier;
	protected override void OnApplied()
	{
		StatModifier = StatModifierResource.StatModifier;
		
		ManagerComponent.ActorComponent.AddStatModifier( StatModifier );
	}

	public override void OnRemoved()
	{
		for ( var i = 0; i < Stacks; i++ )
		{
			ManagerComponent.ActorComponent.RemoveStatModifier( StatModifier );
		}
	}

	public override void OnStacksChanged( int oldStacks )
	{
		var dif = Stacks - oldStacks;

		if ( dif <= 0 )
			return;

		for ( var i = 0; i < dif; i++ )
		{
			ManagerComponent.ActorComponent.AddStatModifier( StatModifier );
		}
	}
}
