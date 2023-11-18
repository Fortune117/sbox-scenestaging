using Sandbox;

namespace DarkDescent.Actor;

[Prefab]
public class ActorEntity : AnimatedEntity
{
	public override void Spawn()
	{
		base.Spawn();
		
		EnableAllCollisions = true;
		EnableDrawing = true;
		EnableHitboxes = true;
		SurroundingBoundsMode = SurroundingBoundsType.Hitboxes;
	}
}
