using Sandbox;

namespace DarkDescent.Components;

public class QuickParticle : LegacyParticleSystem
{
	public static QuickParticle Create(string effect, Vector3 position)
	{
		var go = new GameObject();
		var particles = go.Components.Create<QuickParticle>();
		//particles.GameObject.Parent = GameManager.ActiveScene;
		particles.Particles = Sandbox.ParticleSystem.Load( effect );
		particles.Transform.Position = position;
		
		return particles;
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
		
		if (SceneObject is null || SceneObject.Finished)
			GameObject.Destroy();
	}
}
