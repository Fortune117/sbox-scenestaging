using System.Threading.Tasks;
using DarkDescent.Actor.Damage;
using DarkDescent.Actor.Marker;
using Sandbox;

namespace DarkDescent.Actor;

public class SkeletonBehaviourComponent : BehaviourComponent, IDamageTakenListener, IDeathListener
{
	[Property]
	private GameObject Weapon { get; set; }
	
	[Property]
	private GameObject HoldR { get; set; }
	
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

		Body.Set( "fActionSpeed", ActorComponent.Stats.ActionSpeed );
		Body.Set( "fMoveSpeed", ActorComponent.Stats.MoveSpeedMultiplier );
		
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
			CharacterController.ApplyFriction( 3.0f );
		}
		
		CharacterController.Move();

		if ( !CharacterController.IsOnGround )
		{
			CharacterController.Velocity += Scene.PhysicsWorld.Gravity * Time.Delta;
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

		if ( hit is null)
			return;

		var hitEvent = hit.Value;
		
		var knockback = ActorComponent.Stats.KnockBack;
		var damage = new DamageEventData()
			.WithOriginator( ActorComponent )
			.WithTarget( hitEvent.Damageable )
			.UsingTraceResult( hitEvent.TraceResult )
			.WithDirection( hitEvent.HitDirection )
			.WithKnockBack( knockback )
			.WithDamage( 5f )
			.WithType( DamageType.Physical )
			.AsCritical( false );
		
		if ( hitEvent.WasBlocked )
		{
			damage.WasBlocked = true;
			damage = hitEvent.Blocker.BlockedHit( damage );
		}

		if ( damage.DamageResult <= 0 )
			return;
		
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

		WishVelocity = dir * 50f * ActorComponent.Stats.MoveSpeedMultiplier;
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

	public void OnDeath( DamageEventData damageEventData )
	{
		CharacterController.Velocity = 0f;
		
		if ( Weapon is null )
			return;
		
		Weapon.SetParent( Scene );

		if ( Weapon.TryGetComponent<ModelCollider>( out var modelCollider, false ) )
			modelCollider.Enabled = true;

		if ( Weapon.TryGetComponent<PhysicsComponent>( out var physicsComponent, false ) )
			physicsComponent.Enabled = true;

		Revive();
	}

	private async void Revive()
	{
		await Task.Delay( TimeSpan.FromSeconds( 2f ) );
		
		ActorComponent.Revive();
		
		if ( Weapon is null )
			return;
		
		Weapon.SetParent( HoldR );

		if ( Weapon.TryGetComponent<ModelCollider>( out var modelCollider, false ) )
			modelCollider.Enabled = false;

		if ( Weapon.TryGetComponent<PhysicsComponent>( out var physicsComponent, false ) )
			physicsComponent.Enabled = false;

		Weapon.Transform.LocalPosition = 0;
		Weapon.Transform.LocalRotation = Rotation.Identity;
	}

	public void OnDamageTaken( DamageEventData damageEventData, bool isLethal )
	{
		ApplyKnockBack( damageEventData );
	}

	private void ApplyKnockBack( DamageEventData damageEventData )
	{
		var knockback = damageEventData.Direction * damageEventData.KnockBackResult * 3f;
		CharacterController.Punch( knockback );
	}
}
