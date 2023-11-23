using Sandbox;

namespace DarkDescent.Actor.StatusEffects;

public class StatusEffectComponent : BaseComponent
{
	/// <summary>
	/// The internal 'name' for the effect. Effects that share IDs are used for stacking and exclusivity.
	/// </summary>
	[Property, ToggleGroup("Status")]
	public string EffectID { get; set; }
	
	/// <summary>
	/// If a status effect is unique, only one of it can be assigned to a status effect manager at a time.
	/// This means that newer effects with the same effect ID as an older status effect will replace it.
	/// </summary>
	[Property, ToggleGroup("Status")]
	public bool IsUnique { get; set; }
	
	/// <summary>
	/// Whether or not this effect should be able to stack.
	/// </summary>
	[Property, ToggleGroup("Status")]
	public bool CanStack { get; set; }

	[Property, ToggleGroup("Status")] 
	public int MaxStacks { get; set; } = 1;
	
	/// <summary>
	/// Determines if the duration should be reset when new stacks are applied.
	/// </summary>
	[Property, ToggleGroup("Status")] 
	public bool StacksResetDuration { get; set; }
	
	[Property, Range(0, 60), ToggleGroup("Status")]
	public float MaxDuration { get; set; }

	/// <summary>
	/// How many times per second this status effect ticks.
	/// </summary>
	[Property, Range( 1, 60 ), ToggleGroup("Status")]
	public float TickRate { get; set; } = 1;

	protected StatusEffectManagerComponent ManagerComponent { get; private set; }
	
	protected int Stacks { get; set; } = 1;
	
	protected float TimeUntilDurationExpires { get; set; }
	protected float TimeSinceApplied { get; set; }
	protected float TimeSinceLastTick { get; set; }

	public void Apply( StatusEffectManagerComponent managerComponent )
	{
		ManagerComponent = managerComponent;
		TimeSinceApplied = 0;
		TimeUntilDurationExpires = MaxDuration;

		OnApplied();
	}

	public override void Update()
	{
		TimeSinceLastTick += Time.Delta;
		
		if ( TimeSinceLastTick < 1 / TickRate )
			return;

		Tick(TimeSinceLastTick);

		TimeUntilDurationExpires -= TimeSinceLastTick;
		TimeSinceLastTick = 0;
		
		if ( TimeUntilDurationExpires <= 0)
		{
			ManagerComponent.RemoveStatusEffect( this );
		}
	}

	protected virtual void Tick( float delta ) {}

	protected virtual void OnApplied() {}

	public virtual void OnRemoved() {}
}
