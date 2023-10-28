using System.Collections.Generic;
using Sandbox;

namespace DarkDescent.Components;

public class PhysicsTagger : BaseComponent
{
	[Property] public TagSet Tags { get; set; }

	[Property]
	public bool DiscardOriginalTags { get; set; }

	private void AddTags()
	{
		foreach ( var collider in GetComponents<Collider>() )
		{
			foreach (var physicsShape in collider.Shapes)
			{
				if (DiscardOriginalTags)
					physicsShape.ClearTags();
				
				foreach ( var tag in Tags.TryGetAll() )
				{
					physicsShape.AddTag( tag );
				}
			}
		}
	}

	private void RemoveTags()
	{
		foreach ( var collider in GetComponents<Collider>() )
		{
			foreach (var physicsShape in collider.Shapes)
			{
				foreach ( var tag in Tags.TryGetAll() )
				{
					physicsShape.RemoveTag( tag );
				}
			}
		}
	}

	public override void OnEnabled()
	{
		AddTags();
	}

	public override void OnStart()
	{
		AddTags();
	}

	public override void OnDisabled()
	{
		RemoveTags();
	}
}
