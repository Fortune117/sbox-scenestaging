using Sandbox;

public sealed class CopyTransforms : Component
{
	[Property] private GameObject Child { get; set; }
	
	protected override void OnUpdate()
	{
		Transform.Position = Child.Transform.Position;
		Transform.Rotation = Child.Transform.Rotation;
	}
}
