using DarkDescent.Actor.Damage;
using DarkDescent.Actor.Marker;
using Sandbox;

namespace DarkDescent.Actor;

public class FlameSkullBehaviourComponent : BehaviourComponent, IDamageTakenListener, BaseComponent.ITriggerListener
{
	[Property]
	private PhysicsComponent RigidBody { get; set; }
	
	[Property]
	private ParticleSystem FlameParticles { get; set; }

	[Property, Range( 50f, 300f )] 
	private float HoverDistance { get; set; } = 100f;
	
	private bool KnockedOut;
	private TimeUntil TimeUntilWakeUp;
	
	public void OnDamageTaken( DamageEventData damageEvent, bool isLethal )
	{
		if ( isLethal )
			return;

		if ( damageEvent.KnockBackResult > 10 )
		{
			KnockOut();
		}
	}

	private void KnockOut()
	{
		KnockedOut = true;
		TimeUntilWakeUp = 1.5f;

		RigidBody.Gravity = true;
		FlameParticles.SceneObject.EmissionStopped = true;
		//FlameParticles.Enabled = false;
	}

	private void WakeUp()
	{
		KnockedOut = false;

		RigidBody.Gravity = false;
		FlameParticles.SceneObject.EmissionStopped = false;
		//FlameParticles.Enabled = true;
	}

	public override void Update()
	{
		base.Update();
		
		if (KnockedOut && TimeUntilWakeUp)
			WakeUp();

		if ( KnockedOut )
			return;

		var tr = Physics.Trace.Ray( Transform.Position, Transform.Position + Vector3.Down * HoverDistance )
			.WithoutTags( "actor", "trigger", "player" )
			.Run();

		var length = HoverDistance - tr.Distance;

		Transform.Position = Transform.Position.LerpTo( tr.EndPosition + Vector3.Up * length, Time.Delta * 5f );
		Transform.Rotation = Rotation.Lerp( Transform.Rotation, Rotation.FromYaw( Transform.Rotation.Yaw() ), Time.Delta * 60f );
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		
		if ( KnockedOut )
			return;
		
		RigidBody.Velocity = RigidBody.Velocity.LerpTo( Vector3.Zero, Time.Delta * 2.5f );
		RigidBody.AngularVelocity = RigidBody.AngularVelocity.LerpTo( Vector3.Zero, Time.Delta * 4f );
	}

	public void OnTriggerEnter( Collider other )
	{
		Log.Info( other.GameObject.Name );
	}

	public void OnTriggerExit( Collider other )
	{
		Log.Info( other.GameObject.Name );
	}
}
