using Sandbox;

namespace DarkDescent.Components;

public class SwordTrail : BaseComponent, BaseComponent.ExecuteInEditor
{
	[Property]
	private GameObject Normal { get; set; }
	
	public ParticleSystem ParticleSystem { get; set; }
	
	public override void OnEnabled()
	{
		ParticleSystem = GetComponent<ParticleSystem>();
	}

	public override void OnDisabled()
	{
		ParticleSystem = null;
	}

	public override void OnStart()
	{
		ParticleSystem = GetComponent<ParticleSystem>();
	}

	public override void Update()
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
