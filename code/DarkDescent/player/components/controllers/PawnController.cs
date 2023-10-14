using System.Collections.Generic;
using Sandbox;

namespace DarkDescent;

public partial class PawnController : EntityComponent<Player>, ISingletonComponent
{
	[ConVar.Replicated( "debug_playercontroller" )]
	public static bool Debug { get; set; } = false;
	
	public Vector3 Position { get; set; }
	public Rotation Rotation { get; set; }
	public Vector3 Velocity { get; set; }
	public Rotation EyeRotation { get; set; }
	
	[Net, Predicted]
	public Vector3 EyeLocalPosition { get; set; }
	public Vector3 BaseVelocity { get; set; }
	public Entity GroundEntity { get; set; }
	public Vector3 GroundNormal { get; set; }

	public Vector3 WishVelocity { get; set; }
	
	[Net, Predicted]
	public bool IsDucking { get; set; } // replicate
	public float DuckFraction { get; set; } = 1;
	protected bool Swimming { get; set; } = false;
	
	/// <summary>
	/// Any bbox traces we do will be offset by this amount.
	/// todo: this needs to be predicted
	/// </summary>
	public Vector3 TraceOffset;
	
	private HashSet<string> Events;
	private HashSet<string> Tags;

	private void UpdateFromEntity()
	{
		Position = Entity.Position;
		Rotation = Entity.Rotation;
		Velocity = Entity.Velocity;
		
		BaseVelocity = Entity.BaseVelocity;
		GroundEntity = Entity.GroundEntity;
		WishVelocity = Entity.Velocity;
	}

	private void FinalizeSimulation()
	{
		Entity.Position = Position;
		Entity.Velocity = Velocity;
		Entity.Rotation = Rotation;
		Entity.GroundEntity = GroundEntity;
		Entity.BaseVelocity = BaseVelocity;
	}
	
	public void Update()
	{
		Events?.Clear();
		Tags?.Clear();
		
		UpdateFromEntity();

		Simulate();
		FinalizeSimulation();
	}

	public void FrameUpdate()
	{
		UpdateFromEntity();
		
		FrameSimulate();
		FinalizeSimulation();
	}
	
	/// <summary>
	/// This is what your logic should be going in
	/// </summary>
	public virtual void Simulate()
	{
		// Nothing
	}

	/// <summary>
	/// This is called every frame on the client only
	/// </summary>
	public virtual void FrameSimulate()
	{
		Game.AssertClient();
	}

	/// <summary>
	/// Call OnEvent for each event
	/// </summary>
	public virtual void RunEvents( PawnController additionalController )
	{
		if ( Events == null ) return;

		foreach ( var e in Events )
		{
			OnEvent( e );
			additionalController?.OnEvent( e );
		}
	}

	/// <summary>
	/// An event has been triggered - maybe handle it
	/// </summary>
	public virtual void OnEvent( string name )
	{

	}

	/// <summary>
	/// Returns true if we have this event
	/// </summary>
	public bool HasEvent( string eventName )
	{
		if ( Events == null ) return false;
		return Events.Contains( eventName );
	}

	/// <summary>
	/// </summary>
	public bool HasTag( string tagName )
	{
		if ( Tags == null ) return false;
		return Tags.Contains( tagName );
	}


	/// <summary>
	/// Allows the controller to pass events to other systems
	/// while staying abstracted.
	/// For example, it could pass a "jump" event, which could then
	/// be picked up by the playeranimator to trigger a jump animation,
	/// and picked up by the player to play a jump sound.
	/// </summary>
	public void AddEvent( string eventName )
	{
		// TODO - shall we allow passing data with the event?

		if ( Events == null ) Events = new HashSet<string>();

		if ( Events.Contains( eventName ) )
			return;

		Events.Add( eventName );
	}


	/// <summary>
	/// </summary>
	public void SetTag( string tagName )
	{
		// TODO - shall we allow passing data with the event?

		Tags ??= new HashSet<string>();

		if ( Tags.Contains( tagName ) )
			return;

		Tags.Add( tagName );
	}

	/// <summary>
	/// Allow the controller to tweak input. Empty by default
	/// </summary>
	public virtual void BuildInput()
	{

	}

	/// <summary>
	/// Traces the bbox and returns the trace result.
	/// LiftFeet will move the start position up by this amount, while keeping the top of the bbox at the same 
	/// position. This is good when tracing down because you won't be tracing through the ceiling above.
	/// </summary>
	public virtual TraceResult TraceBBox( Vector3 start, Vector3 end, Vector3 mins, Vector3 maxs, float liftFeet = 0.0f )
	{
		if ( liftFeet > 0 )
		{
			start += Vector3.Up * liftFeet;
			maxs = maxs.WithZ( maxs.z - liftFeet );
		}

		var tr = Trace.Ray( start + TraceOffset, end + TraceOffset )
			.Size( mins, maxs )
			.WithAnyTags( "solid", "playerclip", "passbullets", "player" )
			.Ignore( Entity )
			.Run();
		
		tr.EndPosition -= TraceOffset;
		return tr;
	}

	/// <summary>
	/// This calls TraceBBox with the right sized bbox. You should derive this in your controller if you 
	/// want to use the built in functions
	/// </summary>
	public virtual TraceResult TraceBBox( Vector3 start, Vector3 end, float liftFeet = 0.0f )
	{
		return TraceBBox( start, end, Vector3.One * -1, Vector3.One, liftFeet );
	}

	/// <summary>
	/// This is temporary, get the hull size for the player's collision
	/// </summary>
	public virtual BBox GetHull()
	{
		return new BBox( -10, 10 );
	}
}

