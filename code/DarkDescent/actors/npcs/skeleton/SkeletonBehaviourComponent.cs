using System.Threading.Tasks;
using DarkDescent.Actor.Damage;
using DarkDescent.Actor.Marker;
using DarkDescent.Items;
using Sandbox;

namespace DarkDescent.Actor;

public class SkeletonBehaviourComponent : BehaviourComponent, IDamageTakenListener, IDeathListener
{
	private enum SkeletonHoldTypes
	{
		None = 0,
		Longsword = 1,
		GreatSword = 2
	}
	
	[Property]
	private WeaponComponent Weapon { get; set; }
	
	[Property]
	private GameObject HoldR { get; set; }
	
	[Property]
	private SkeletonHoldTypes HoldType { get; set; }

	[Property, Range( 0, 300 )] 
	private float AttackRange { get; set; } = 100f;
	
	private bool isAttacking;
	private bool hitBoxesActive;

	private AttackEvent AttackEvent;
	private Vector3 WishVelocity = Vector3.Zero;

	protected override void OnStart()
	{
		base.OnStart();
		
		ActorComponent.Body.Set( "eHoldType", (int)HoldType );
	}

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
				Weapon.SwordTrail?.StartTrail();
				var sound = Sound.Play( Weapon.SwingSound.ResourceName, Weapon.Transform.Position );
				sound.Pitch = ActorComponent.Stats.ActionSpeed.Remap( 0, 2, 0.5f, 1.5f );
				break;
			}
			
			case BehaviourAnimationEvents.DeactivateHitBoxes:
			{
				hitBoxesActive = false;
				Weapon.SwordTrail?.StopTrail();
				Weapon.StopScrapeEffect(); 
				break;
			}
		}
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
		
		TargetingComponent.UpdateTargetFromDistance();

		MoveUpdate();

		WishVelocity = Vector3.Zero;

		ActorComponent.Body.Set( "fActionSpeed", ActorComponent.Stats.ActionSpeed );
		ActorComponent.Body.Set( "fMoveSpeed", ActorComponent.Stats.MoveSpeedMultiplier );
		
		if ( Target is null )
		{
			ActorComponent.Body.Set( "bMoving", false );
			return;
		}
		
		var distance = Target.Transform.Position.Distance( Transform.Position );

		if ( isAttacking )
		{
			AttackUpdate();
			return;
		}
		
		if (distance < AttackRange)
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

	private TimeSince lastScrapeEvent;
	private void AttackUpdate()
	{
		FaceTarget();

		if (!hitBoxesActive)
			return;

		var hit = AttackEvent.CheckForHit();

		if ( hit is null )
		{
			Weapon.StopScrapeEffect();
			return;
		}

		var hitEvent = hit.Value;
		
		var damage = Weapon.GetDamage( ActorComponent );
		
		var knockback = ActorComponent.Stats.KnockBack;
		
		var damageEvent = new DamageEventData()
			.WithOriginator( ActorComponent )
			.WithTarget( hitEvent.Damageable )
			.UsingTraceResult( hitEvent.TraceResult )
			.WithDirection( Weapon.GetImpactDirection() )
			.WithKnockBack( knockback * Weapon.KnockBackMultiplier )
			.WithDamage( damage )
			.WithType( Weapon.GetDamageType() )
			.AsCritical( false );
		
		if ( hitEvent.WasBlocked )
		{
			damageEvent.WasBlocked = true;
			damageEvent = hitEvent.Blocker.BlockedHit( damageEvent );
		}
		
		if ( hitEvent.HitWorld ) //impacted the world?
		{
			Weapon.PlayScrapeEffect( hitEvent.TraceResult );

			if ( lastScrapeEvent > 0.02f )
			{
				damageEvent.CreateScrapeEffect();
				lastScrapeEvent = 0; 
			}
			return; 
		}
		
		Weapon.StopScrapeEffect();

		if ( damageEvent.DamageResult <= 0 )
			return;
		
		if (hitEvent.Damageable.PlayHitSound)
			Sound.Play( Weapon.ImpactSound.ResourceName, hitEvent.TraceResult.HitPosition );
		
		hitEvent.Damageable?.TakeDamage( damageEvent );
	}

	private void FaceTarget()
	{
		if ( RotationLocked )
			return;
		
		var dir = Target.Transform.Position - Transform.Position;
		dir = dir.WithZ( 0 ).Normal;
		
		ActorComponent.Body.Transform.Rotation = Rotation.Lerp(ActorComponent.Body.Transform.Rotation, Rotation.LookAt( dir), Time.Delta * 8f);
	}
	
	private void FollowTarget()
	{
		var dir = Target.Transform.Position - Transform.Position;
		dir = dir.WithZ( 0 ).Normal;

		WishVelocity = dir * 50f * ActorComponent.Stats.MoveSpeedMultiplier;
		FaceTarget();
		
		ActorComponent.Body.Set( "bMoving", true );
	}

	private void AttackTarget()
	{
		ActorComponent.Body.Set( "bMoving", false );
		ActorComponent.Body.Set( "bAttack", true );

		isAttacking = true;

		AttackEvent = new AttackEvent()
			.WithInitiator( GameObject )
			.WithHurtBox( Components.GetInDescendantsOrSelf<HurtBoxComponent>(true ) );
	}

	public void OnDeath( DamageEventData damageEventData )
	{
		CharacterController.Velocity = 0f;
		
		if ( Weapon is null )
			return;

		Weapon.SwordTrail?.StopTrail();
		Weapon.AnimatedModelComponent.BoneMergeTarget = null;
		Weapon.GameObject.SetParent( Scene );

		if ( Weapon.Components.TryGet<ModelCollider>( out var modelCollider, FindMode.EverythingInSelf ) )
			modelCollider.Enabled = true;

		if ( Weapon.Components.TryGet<PhysicsComponent>( out var physicsComponent, FindMode.EverythingInSelf ) )
			physicsComponent.Enabled = true;

		Revive();
	}

	private async void Revive()
	{
		await Task.Delay( TimeSpan.FromSeconds( 2f ).Milliseconds );
		
		ActorComponent.Revive();
		
		if ( Weapon is null )
			return;
		
		Weapon.AnimatedModelComponent.BoneMergeTarget = ActorComponent.Body;
		Weapon.GameObject.SetParent( GameObject );

		if ( Weapon.Components.TryGet<ModelCollider>( out var modelCollider, FindMode.EverythingInSelf ) )
			modelCollider.Enabled = false;

		if ( Weapon.Components.TryGet<PhysicsComponent>( out var physicsComponent, FindMode.EverythingInSelf ) )
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
