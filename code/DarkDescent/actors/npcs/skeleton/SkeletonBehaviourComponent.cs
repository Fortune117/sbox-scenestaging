using DarkDescent.Actor.Damage;
using Sandbox;

namespace DarkDescent.Actor;

public class SkeletonBehaviourComponent : BehaviourComponent
{
	private bool isAttacking;
	private bool hitBoxesActive;

	private AttackEvent AttackEvent;
	
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

		Transform.Position += dir * 25f * Time.Delta;
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
