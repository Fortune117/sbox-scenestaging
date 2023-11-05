using Sandbox;

namespace DarkDescent.Components;

public class QuickParticle : ParticleSystem
{
	public static QuickParticle Create(string effect, Vector3 position)
	{
		var go = new GameObject();
		var particles = go.AddComponent<QuickParticle>();
		//particles.GameObject.Parent = GameManager.ActiveScene;
		particles.Particles = Sandbox.ParticleSystem.Load( effect );
		particles.Transform.Position = position;
		
		return particles;
	}

	public override void Update()
	{
		base.Update();
		
		if (SceneObject is null || SceneObject.Finished)
			GameObject.Destroy();
	}
}
