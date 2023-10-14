using Sandbox;

namespace DarkDescent.Actor.Damage;

/// <summary>
/// Data struct for damage after it has been processed by an actor component.
/// Can be networked to the client.
/// </summary>
public struct DamageEventData
{
	/// <summary>
	/// The final damage actually received, post modifiers.
	/// </summary>
	public float DamageResult { get; set; }
	
	/// <summary>
	/// The original damage value dealt before modifiers.
	/// </summary>
	public float DamageOriginal { get; set; }
	
	/// <summary>
	/// The strength of the knockback this damage stat should have.
	/// </summary>
	public float KnockBack { get; set; }

	/// <summary>
	/// How much resistance we are penetrating with this damage.
	/// </summary>
	public float ResistancePenetration { get; set; }
	
	/// <summary>
	/// Whether or not this damage was from a critical hit.
	/// </summary>
	public bool IsCritical { get; set; }
	
	/// <summary>
	/// Where the damage was dealt in world space.
	/// Used for damage number spawning.
	/// </summary>
	public Vector3 Position { get; set; }
	
	/// <summary>
	/// The direction this damage was dealt in.
	/// </summary>
	public Vector3 Direction { get; set; }
	
	/// <summary>
	/// The various types this damage is.
	/// </summary>
	public DamageType DamageTypes { get; set; }
	
	/// <summary>
	/// The various flags this damage has assigned to it.
	/// </summary>
	public DamageFlags DamageFlags { get; set; }
	

	/// <summary>
	/// Who this damage event was originated by.
	/// </summary>
	public GameObject Originator { get; set; }

	/// <summary>
	/// Target
	/// </summary>
	public GameObject Target { get; set; }

	/// <summary>
	/// Get the ActorComponent of the Target.
	/// Can be null.
	/// </summary>
	public ActorComponent TargetActor => Target?.GetComponent<ActorComponent>();
	
	/// <summary>
	/// Get the ActorComponent of the Originator.
	/// Can be null.
	/// </summary>
	public ActorComponent OriginatorActor => Originator?.GetComponent<ActorComponent>();

	public bool HasDamageType( DamageType damageType )
	{
		return DamageTypes.HasFlag( damageType );
	}

	public DamageEventData WithTarget( GameObject target )
	{
		Target = target;
		return this;
	}

	public DamageEventData WithOriginator( GameObject originator )
	{
		Originator = originator;
		return this;
	}

	public bool HasFlag( DamageFlags flags )
	{
		return DamageFlags.HasFlag( flags );
	}
}
