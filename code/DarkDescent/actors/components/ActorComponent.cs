﻿using DarkDescent.Actor.StatSystem;
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
	public AnimatedModelComponent Body { get; set; }
	
	public StatusEffectManagerComponent StatusEffectsManager { get; set; }
	
	public Stats Stats { get; private set; }

	public override void OnAwake()
	{
		StatusEffectsManager = GetComponent<StatusEffectManagerComponent>( false, true );
	}

	public override void OnEnabled()
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
	

	public override void OnDisabled()
	{
		base.OnDisabled();
		
		//TODO: networking
		BaseStats.OnReload -= ReloadBaseStats;
		
		//TODO: world panels
		DestroyInfoPanel();
		
		Blackboard.UnRegister( this );
	}

	public override void OnDestroy()
	{
		DestroyInfoPanel();
		
		Blackboard.UnRegister( this );
	}

	public override void Update()
	{
		UpdateStats();
		UpdateStamina();
		UpdateInfoPanel();
	}
}
