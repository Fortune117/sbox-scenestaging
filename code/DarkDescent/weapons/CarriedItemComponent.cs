using System.Linq;
using DarkDescent.Actor;
using DarkDescent.Actor.Damage;
using Sandbox;

namespace DarkDescent.Weapons;

public class CarriedItemComponent : BaseComponent, BaseComponent.ExecuteInEditor
{
	[Property]
	private bool FollowBoneMerge { get; set; }
	
	[Property]
	private HurtBoxComponent HurtBox { get; set; }
	
	private AnimatedModelComponent AnimatedModelComponent { get; set; }

	public override void OnEnabled()
	{
		base.OnEnabled();

		AnimatedModelComponent = GetComponent<AnimatedModelComponent>();
	}

	public override void OnStart()
	{
		AnimatedModelComponent = GetComponent<AnimatedModelComponent>();
	}

	public override void OnDisabled()
	{
		base.OnDisabled();

		AnimatedModelComponent = null;
	}

	private TimeUntil TimeUntilCanHit;
	public override void Update()
	{
		base.Update();

		if ( !FollowBoneMerge || AnimatedModelComponent?.SceneObject is null )
			return;
		
		var transform = AnimatedModelComponent.SceneObject.GetBoneWorldTransform( 0 );
		GameObject.Transform.World = transform;

		if ( Scene.IsEditor || !TimeUntilCanHit)
			return;
		
		var tr = HurtBox.PerformTrace();
		
		Gizmo.Transform = Scene.Transform.World;
		Gizmo.Draw.Color = Color.White.WithAlpha( 0f );
		Gizmo.Draw.SolidSphere( tr.EndPosition, 4 );
		
		Gizmo.Draw.Color = Color.Blue.WithAlpha( 1f );
		Gizmo.Draw.LineSphere( new Sphere( tr.EndPosition, 4 ) );
		
		if ( !tr.Hit )
			return;

		var gameObject = tr.Body.GameObject;
		if ( gameObject is not GameObject hitGameObject )
			return;

		var hitActor = hitGameObject.GetComponentInParent<ActorComponent>( true, true );
		if ( hitActor is null )
			return;

		var actor = GetComponentInParent<ActorComponent>();
		var knockback = actor is not null ? actor.Stats.KnockBack : 0;

		var damage = new DamageEventData()
			.WithOriginator( actor )
			.WithTarget( hitActor )
			.WithPosition( tr.HitPosition + tr.Normal * 5f )
			.WithDirection( tr.Direction )
			.WithKnockBack( knockback*5f )
			.WithDamage( 1f )
			.WithType( DamageType.Physical )
			.AsCritical( false );

		hitActor.TakeDamage( damage );

		TimeUntilCanHit = 0.5f;
	}
}
