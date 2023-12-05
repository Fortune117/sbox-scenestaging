using DarkDescent.Actor.StatSystem;
using DarkDescent.Actor.StatusEffects;
using Sandbox;

namespace DarkDescent.Actor;

/// <summary>
/// An Actor is anything in the game that the various NPCs and their systems should interact with.
/// i.e. all npcs are Actors, the player is an Actor, etc
/// </summary>
public partial class ActorComponent : BaseComponent
{
	[Property, ToggleGroup("Attributes")] 
	public Attributes PrimaryAttribute { get; set; } = Attributes.Strength;

	[Property, ToggleGroup("Attributes")] 
	public Attributes SpellCastingAttribute { get; set; } = Attributes.Intelligence;
	
	[Property, ToggleGroup("Attributes")]
	public StatsResource BaseStats { get; set; }
	
	[Property] 
	public SkinnedModelRenderer Body { get; set; }
	
	public StatusEffectManagerComponent StatusEffectsManager { get; set; }
	
	public Stats Stats { get; private set; }

	protected override void OnAwake()
	{
		StatusEffectsManager = Components.Get<StatusEffectManagerComponent>( FindMode.EverythingInSelfAndDescendants );
	}

	protected override void OnEnabled()
	{
		base.OnEnabled();

		Stats = new Stats( this );

		//TODO: networking

		BaseStats.OnReload += ReloadBaseStats;
		InitializeStats();

		Alive = true;
		
		//TODO: world panels
		CreateInfoPanel();
		
		Blackboard.Register( this );
	}
	

	protected override void OnDisabled()
	{
		base.OnDisabled();
		
		//TODO: networking
		BaseStats.OnReload -= ReloadBaseStats;
		
		//TODO: world panels
		DestroyInfoPanel();
		
		Blackboard.UnRegister( this );
	}

	protected override void OnDestroy()
	{
		DestroyInfoPanel();
		
		Blackboard.UnRegister( this );
	}

	protected override void OnUpdate()
	{
		UpdateStats();
		UpdateInfoPanel();
	}
}
