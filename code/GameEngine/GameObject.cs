﻿using Sandbox;
using Sandbox.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public enum GameObjectFlags
{
	None = 0,

	/// <summary>
	/// Hide this object in heirachy/inspector
	/// </summary>
	Hidden = 1,

	/// <summary>
	/// Don't save this object to disk, or when duplicating
	/// </summary>
	NotSaved = 2,
}

public partial class GameObject
{
	protected Scene _scene;

	public Scene Scene => _scene;

	public Guid Id { get; private set; }

	[Property]
	public string Name { get; set; } = "Untitled Object";

	public PrefabFile PrefabSource { get; set; }


	public GameObjectFlags Flags { get; set; } = GameObjectFlags.None;

	bool _enabled = true;

	/// <summary>
	/// Is this gameobject enabled?
	/// </summary>
	[Property]
	public bool Enabled
	{
		get => _enabled;
		set
		{
			if ( _enabled == value )
				return;

			_enabled = value;
			UpdateEnabledStatus();
		}
	}

	Transform _transform = Transform.Zero;

	[Property]
	public Transform Transform
	{
		get => _transform;
		set
		{
			if ( _transform == value )
				return;

			_transform = value;
			OnLocalTransformChanged?.Invoke( value );
		}
	}

	internal GameObject( bool enabled, string name, Scene scene )
	{
		_enabled = enabled;
		_scene = scene;
		Id = Guid.NewGuid();
		Name = name;
	}

	public static GameObject Create( bool enabled = true, string name = "GameObject" )
	{
		if ( GameManager.ActiveScene is null )
			throw new System.ArgumentNullException( "Trying to create a GameObject without an active scene" );

		return new GameObject( enabled, name, GameManager.ActiveScene );
	}

	public Transform WorldTransform
	{
		get
		{
			if ( Parent is not null )
			{
				return Parent.WorldTransform.ToWorld( Transform );
			}

			return Transform;
		}

		set
		{
			if ( Parent is not null )
			{
				Transform = Parent.WorldTransform.ToLocal( value );
				return;
			}

			Transform = value;
		}
	}

	public Action<Transform> OnLocalTransformChanged;

	public override string ToString()
	{
		return $"GO - {Name}";
	}

	public List<BaseComponent> Components = new List<BaseComponent>();

	GameObject _parent;

	public GameObject Parent
	{
		get => _parent;
		set
		{
			if ( _parent == value ) return;

			if ( value is not null )
			{
				if ( value.IsAncestor( this ) )
					return;
			}

			var oldParent = _parent;

			if ( oldParent is not null )
			{
				oldParent.Children.Remove( this );
			}

			_parent = value;

			if ( _parent is not null )
			{
				Assert.True( Scene == _parent.Scene, "Can't parent to a gameobject in a different scene" );
				_parent.Children.Add( this );
			}

			if ( isDestroying )
				return;

			if ( Scene is not null )
			{
				Scene.OnParentChanged( this, oldParent, _parent );
			}
		}
	}

	public List<GameObject> Children { get; } = new List<GameObject>();

	/// <summary>
	/// Is this gameobject active. For it to be active, it needs to be enabled, all of its ancestors
	/// need to be enabled, and it needs to be in a scene.
	/// </summary>
	public bool Active => Enabled && Scene is not null && (Parent?.Active ?? true);

	internal void OnCreate()
	{
		foreach ( var component in Components )
		{
			if ( component is BaseComponent goc )
			{
				goc.GameObject = this;
			}
		}

		foreach ( var child in Children )
		{
			child.OnCreate();
		}
	}

	internal void OnDestroy()
	{
		foreach ( var component in Components )
		{
			component.OnDisabled();

			if ( component is BaseComponent goc )
			{
				goc.GameObject = null;
			}
		}

		foreach ( var child in Children )
		{
			child.OnDestroy();
		}
	}

	internal void PostPhysics()
	{
		//Gizmo.Draw.LineSphere( new Sphere( WorldTransform.Position, 3 ) );

		foreach ( var component in Components )
		{
			component.PostPhysics();
		}

		foreach ( var child in Children )
		{
			child.PostPhysics();
		}
	}

	internal void PreRender()
	{

		foreach ( var component in Components )
		{
			component.PreRender();
		}

		foreach ( var child in Children )
		{
			child.PreRender();
		}
	}

	public T AddComponent<T>( bool enabled = true ) where T : BaseComponent, new()
	{
		var t = new T();

		t.GameObject = this;
		Components.Add( t );

		t.Enabled = enabled;
		return t;
	}

	public T GetComponent<T>( bool enabledOnly = true, bool deep = false ) where T : BaseComponent
	{
		return GetComponents<T>( enabledOnly, deep ).FirstOrDefault();
	}

	public IEnumerable<T> GetComponents<T>( bool enabledOnly = true, bool deep = false ) where T : BaseComponent
	{
		var q = Components.OfType<T>();
		if ( enabledOnly ) q = q.Where( x => x.Active );

		foreach ( var c in q )
		{
			yield return c;
		}

		if ( deep )
		{
			foreach ( var child in Children )
			{
				foreach ( var found in child.GetComponents<T>( enabledOnly, deep ) )
				{
					yield return found;
				}
			}
		}
	}

	public BaseComponent AddComponent( TypeDescription type, bool enabled = true )
	{
		if ( !type.TargetType.IsAssignableTo( typeof( BaseComponent ) ) )
			return null;

		var t = type.Create<BaseComponent>( null );

		t.GameObject = this;
		Components.Add( t );

		t.Enabled = enabled;

		return t;
	}

	internal virtual void Tick()
	{
		if ( !Enabled )
			return;

		OnUpdate();

		for ( int i=0; i < Children.Count; i++ )
		{
			Children[i].Tick();
		}
	}

	bool isDestroying;
	bool isDestroyed;

	public void Destroy()
	{
		isDestroying = true;
		Scene?.QueueDelete( this );
	}

	/// <summary>
	/// Should be called whenever we change anything that we suspect might
	/// cause the active status to change on us, or our components.
	/// </summary>
	internal void UpdateEnabledStatus()
	{
		foreach ( var component in Components )
		{
			component.GameObject = this;
			component.UpdateEnabledStatus();
		}

		foreach ( var child in Children )
		{
			child.UpdateEnabledStatus();
		}
	}

	public void DestroyImmediate()
	{
		bool isRoot = Parent == null;
		var scene = Scene;

		DestroyRecursive();

		if ( isRoot && scene is not null )
		{
			scene.Remove( this );
		}
	}

	/// <summary>
	/// We are now disconnected from the scene, we can tell all of our children to disconnect too.
	/// </summary>
	void DestroyRecursive()
	{
		isDestroying = true;
		Parent = null;
		Enabled = false;
		_scene = null;
		isDestroyed = true;

		foreach ( var child in Children.ToArray() )
		{
			child.DestroyRecursive();
		}
	}

	public bool IsDescendant( GameObject o )
	{
		return o.IsAncestor( this );
	}

	public bool IsAncestor( GameObject o )
	{
		if ( o == this ) return true;

		if ( Parent is not null )
		{
			return Parent.IsAncestor( o );
		}

		return false;
	}

	public void AddSibling( GameObject go, bool before, bool keepWorldPosition = true )
	{
		go.SetParent( Parent, keepWorldPosition );

		go.Parent.Children.Remove( go );
		var targetIndex = go.Parent.Children.IndexOf( this );
		if ( !before ) targetIndex++;
		go.Parent.Children.Insert( targetIndex, go );
	}

	public void SetParent( GameObject value, bool keepWorldPosition = true )
	{
		if ( Parent == value ) return;

		if ( keepWorldPosition )
		{
			var wp = WorldTransform;
			Parent = value;
			WorldTransform = wp;
		}
		else
		{
			Parent = value;
		}
	}

	/// <summary>
	/// Find component on this gameobject, or its parents
	/// </summary>
	public T GetComponentInParent<T>( bool enabledOnly = true ) where T : BaseComponent
	{
		var t = GetComponent<T>( enabledOnly, false );
		if ( t is not null )
			return t;

		if ( Parent is not null )
		{
			return Parent.GetComponentInParent<T>( enabledOnly );
		}

		return null;
	}

	IEnumerable<GameObject> GetSiblings()
	{
		if ( Parent is not null )
		{
			return Parent.Children.Where( x => x != this );
		}

		return Enumerable.Empty<GameObject>();
	}

	// todo - this should be internal
	public void MakeNameUnique()
	{
		var names = GetSiblings().Select( x => x.Name ).ToHashSet();

		if ( !names.Contains( Name ) )
			return;

		var targetName = Name;

		// todo regex (number)

		if ( targetName.Contains( '(' ) )
		{
			targetName = targetName.Substring( 0, targetName.LastIndexOf( '(' ) ).Trim();
		}

		for ( int i = 1; i < 500; i++ )
		{
			var indexedName = $"{targetName} ({i})";

			if ( !names.Contains( indexedName ) )
			{
				Name = indexedName;
				return;
			}
		}
	}

	void OnUpdate()
	{
		for ( int i=0; i< Components.Count; i++ )
		{
			Components[i].InternalUpdate();
		}
	}

	/// <summary>
	/// Find a GameObject by Guid
	/// </summary>
	public GameObject FindObjectByGuid( Guid guid )
	{
		if ( guid == Id )
			return this;

		return Children.Select( x => x.FindObjectByGuid( guid ) )
								.Where( x => x is not null )
								.FirstOrDefault();
	}

	public virtual void EditLog( string name, object source, Action undo )
	{
		if ( Parent == null ) return;

		Parent.EditLog( name, source, undo );
	}
}
