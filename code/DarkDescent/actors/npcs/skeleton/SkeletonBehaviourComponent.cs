using DarkDescent.Actor.Damage;
using Sandbox;

namespace DarkDescent.Actor;

public class SkeletonBehaviourComponent : BehaviourComponent
{
	private bool isAttacking;
	private bool hitBoxesActive;

	private AttackEvent AttackEvent;
	private Vector3 WishVelocity = Vector3.Zero;
	
	protected override void OnGenericAnimEvent( SceneModel.GenericEvent genericEvent )
	{
		base.OnGenericAnimEvent( genericEvent );

		switch ( genericEvent.Type )
		{
			case BehaviourAnimationEvents.FinishAttack:
			{
				isAttacking = false;
				break;
			}

			case BehaviourAnimationEvents.ActivateHitBoxes:
			{
				hitBoxesActive = true;
				break;
			}
			
			case BehaviourAnimationEvents.DeactivateHitBoxes:
			{
				hitBoxesActive = false;
				break;
			}
		}
	}

	public override void Update()
	{
		base.Update();
		
		TargetingComponent.UpdateTargetFromDistance();

		MoveUpdate();

		WishVelocity = Vector3.Zero;
		
		if ( Target is null )
		{
			Body.Set( "bMoving", false );
			return;
		}
		
		var distance = Target.Transform.Position.Distance( Transform.Position );

		if ( isAttacking )
		{
			AttackUpdate();
			return;
		}
		
		if (distance < 50)
			AttackTarget();
		else
			FollowTarget();
	}

	private void MoveUpdate()
	{
		if ( CharacterController.IsOnGround )
		{
			CharacterController.Velocity = CharacterController.Velocity.WithZ( 0 );
			CharacterController.Accelerate( WishVelocity );
			CharacterController.ApplyFriction( 1.0f );
		}
		
		CharacterController.Move();

		if ( !CharacterController.IsOnGround )
		{
			CharacterController.Velocity -= Scene.PhysicsWorld.Gravity * Time.Delta * 0.5f;
		}
		else
		{
			CharacterController.Velocity = CharacterController.Velocity.WithZ( 0 );
		}
	}

	private void AttackUpdate()
	{
		FaceTarget();

		if (!hitBoxesActive)
			return;

		var hit = AttackEvent.CheckForHit();

		if ( hit is null )
			return;

		var hitEvent = hit.Value;

		var knockback = ActorComponent.Stats.KnockBack;
		
		var damage = new DamageEventData()
			.WithOriginator( ActorComponent )
			.WithPosition( hitEvent.TraceResult.HitPosition + hitEvent.TraceResult.Normal * 5f )
			.WithDirection( hitEvent.HitDirection )
			.WithKnockBack( knockback )
			.WithDamage( 1f )
			//.WithTags( tr.Shape.Tags ) //commented out cause we can't get tags like this :<
			.WithType( DamageType.Physical )
			.AsCritical( false );

		hitEvent.Damageable.TakeDamage( damage );
	}

	private void FaceTarget()
	{
		if ( RotationLocked )
			return;
		
		var dir = Target.Transform.Position - Transform.Position;
		dir = dir.WithZ( 0 ).Normal;
		
		Body.Transform.Rotation = Rotation.Lerp(Body.Transform.Rotation, Rotation.LookAt( dir), Time.Delta * 8f);
	}
	
	private void FollowTarget()
	{
		var dir = Target.Transform.Position - Transform.Position;
		dir = dir.WithZ( 0 ).Normal;

		WishVelocity = dir * 35f;
		FaceTarget();
		
		Body.Set( "bMoving", true );
	}

	private void AttackTarget()
	{
		Body.Set( "bMoving", false );
		Body.Set( "bAttack", true );

		isAttacking = true;

		AttackEvent = new AttackEvent()
			.WithInitiator( GameObject )
			.WithHurtBox( GetComponent<HurtBoxComponent>( true, true ) );
	}
}
