using Sandbox;

public sealed class CopyTransforms : BaseComponent
{
	[Property] private GameObject Child { get; set; }
	
	public override void Update()
	{
		Transform.Position = Child.Transform.Position;
		Transform.Rotation = Child.Transform.Rotation;
	}
}
