using Sandbox;

namespace DarkDescent.Actor;

/// <summary>
/// An Actor is anything in the game that the various NPCs and their systems should interact with.
/// i.e. all npcs are Actors, the player is an Actor, etc
/// </summary>
[Prefab]
public partial class ActorComponent : EntityComponent
{
	[Prefab, Net, Category("Attributes")] 
	public Attributes PrimaryAttribute { get; set; } = Attributes.Strength;

	[Prefab, Net, Category("Attributes")] 
	public Attributes SpellCastingAttribute { get; set; } = Attributes.Intelligence;
	
	[Prefab, Net, Category("Attributes")]
	public StatsResource BaseStats { get; set; }
	
	public Stats Stats { get; private set; }
	
	protected override void OnActivate()
	{
		base.OnActivate();

		Stats = new Stats( this );

		if ( Game.IsServer )
		{
			BaseStats.OnReload += ReloadBaseStats;
			InitializeStats();
		}

		if ( Game.IsClient )
		{
			CreateInfoPanel();
		}
	}
	

	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		
		if ( Game.IsServer )
		{
			BaseStats.OnReload -= ReloadBaseStats;
		}

		if ( Game.IsClient )
		{
			DestroyInfoPanel();
		}
	}

	public void Simulate()
	{
		SimulateStats();
		SimulateStamina();
	}
}
