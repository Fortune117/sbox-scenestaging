using System.Collections.Generic;
using Sandbox;

namespace DarkDescent.Components;

public class PhysicsTagger : BaseComponent
{
	private string _tags;

	[Property, Editor( "tags" )]
	public string Tags
	{
		get => _tags;
		set
		{
			_tags = value;
			UpdateTagList();
		}

	}

	private readonly HashSet<string> TagSet = new();

	private void UpdateTagList()
	{
		TagSet.Clear();

		foreach ( var tag in Tags.Split( " " ) )
		{
			TagSet.Add( tag );
		}
	}

	private void AddTags()
	{
		foreach ( var collider in GetComponents<PhysicsComponent>() )
		{
			var body = collider.GetBody();
			
			if (body is null)
				continue;
			
			foreach (var physicsShape in body.Shapes)
			{
				foreach ( var tag in TagSet )
				{
					physicsShape.AddTag( tag );
				}
			}
		}
	}

	private void RemoveTags()
	{
		foreach ( var collider in GetComponents<PhysicsComponent>() )
		{
			var body = collider.GetBody();
			if (body is null)
				continue;
			
			foreach (var physicsShape in body.Shapes)
			{
				foreach ( var tag in TagSet )
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
