using Sandbox;

namespace DarkDescent.Components;

public class SwordTrail : Component, Component.ExecuteInEditor
{
	[Property]
	private GameObject Normal { get; set; }
	
	public ParticleSystem ParticleSystem { get; set; }
	
	protected override void OnEnabled()
	{
		ParticleSystem = Components.Get<ParticleSystem>();
	}

	protected override void OnDisabled()
	{
		ParticleSystem = null;
	}

	protected override void OnStart()
	{
		ParticleSystem = Components.Get<ParticleSystem>();
	}

	protected override void OnUpdate()
	{
		if ( Normal is null )
			return;
		
		ParticleSystem.Set( "Normal", Normal.Transform.Rotation.Forward );
	}

	public void StartTrail()
	{
		ParticleSystem.EmissionStopped = false;
		ParticleSystem.PlayEffect();
	}

	public void StopTrail()
	{
		ParticleSystem.EmissionStopped = true;
	}
}
