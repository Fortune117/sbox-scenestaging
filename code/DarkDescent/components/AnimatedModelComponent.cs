using Sandbox;
using Sandbox.Diagnostics;

[Title( "Animated Model Renderer" )]
[Category( "Rendering" )]
[Icon( "visibility", "red", "white" )]
[Alias( "ModelComponentMate" )]
public class AnimatedModelComponent : BaseComponent, BaseComponent.ExecuteInEditor
{
	Model _model;

	private BBox _bounds = new BBox( Vector3.Zero, 300f );
	[Property] public BBox Bounds
	{
		get
		{
			return _bounds;
		}
		set
		{
			_bounds = value;
			
			if (_sceneModel.IsValid())
				_sceneModel.Bounds = value;
		}
	}

	[Property] public Model Model 
	{
		get => _model;
		set
		{
			if ( _model == value ) return;
			_model = value;

			if ( _sceneModel is not null )
			{
				_sceneModel.Model = _model;
			}
		}
	}


	Color _tint = Color.White;

	[Property]
	public Color Tint
	{
		get => _tint;
		set
		{
			if ( _tint == value ) return;

			_tint = value;

			if ( _sceneModel is not null )
			{
				_sceneModel.ColorTint = Tint;
			}
		}
	}

	Material _material;
	[Property] public Material MaterialOverride
	{
		get => _material;
		set
		{
			if ( _material == value ) return;
			_material = value;

			if ( _sceneModel is not null )
			{
				_sceneModel.SetMaterialOverride( _material );
			}
		}
	}

	bool _castShadows = true;
	[Property]
	public bool ShouldCastShadows
	{
		get => _castShadows;
		set
		{
			if ( _castShadows == value ) return;
			_castShadows = value;

			if ( _sceneModel is not null )
			{
				_sceneModel.Flags.CastShadows = _castShadows;
			}
		}
	}

	private Vector3 oldPosition = Vector3.Zero;
	private Rotation oldRotation = Rotation.Identity;
	
	bool _boneMerge = false;
	[Property]
	public bool BoneMerge
	{
		get => _boneMerge;
		set
		{
			if ( _boneMerge == value ) return;
			_boneMerge = value;

			if ( _sceneModel is not null )
			{
				SetBoneMergeStatus();
			}
		}
	}

	SceneModel _sceneModel;
	public SceneModel SceneModel => _sceneModel;


	public override void DrawGizmos()
	{
		if ( Model is null )
			return;

		Gizmo.Hitbox.Model( Model );
		
		Gizmo.Draw.Color = Color.White.WithAlpha( 0.1f );

		if ( Gizmo.IsSelected )
		{
			Gizmo.Draw.Color = Color.Green.WithAlpha( 0.1f );
			Gizmo.Draw.LineBBox( Bounds );
			
			Gizmo.Draw.Color = Color.White.WithAlpha( 0.9f );
			Gizmo.Draw.LineBBox( Model.Bounds );
		}
		
		if ( Gizmo.IsHovered )
		{
			Gizmo.Draw.Color = Color.White.WithAlpha( 0.4f );
			Gizmo.Draw.LineBBox( Model.Bounds );
		}
	}

	private void SetBoneMergeStatus()
	{
		var parent = GetComponentInParent<AnimatedModelComponent>();
		if ( parent is not null )
		{
			if ( BoneMerge )
			{
				oldPosition = Transform.LocalPosition;
				oldRotation = Transform.LocalRotation;
				parent.SceneModel.AddChild( "", SceneModel );
			}
			else
			{
				parent.SceneModel.RemoveChild( SceneModel );
				Transform.LocalPosition = oldPosition;
				Transform.LocalRotation = oldRotation;
			}
		}
	}
	
	public override void OnEnabled()
	{
		Assert.True( _sceneModel == null );
		Assert.NotNull( Scene );

		_sceneModel = new SceneModel( Scene.SceneWorld, Model ?? Model.Load( "models/dev/box.vmdl" ), Transform.World );
		_sceneModel.Transform = Transform.World;
		_sceneModel.SetMaterialOverride( MaterialOverride );
		_sceneModel.ColorTint = Tint;
		_sceneModel.Flags.CastShadows = _castShadows;
	}

	public override void OnStart()
	{
		SetBoneMergeStatus();
	}

	public override void OnDisabled()
	{
		_sceneModel?.Delete();
		_sceneModel = null;
	}

	public override void Update()
	{
		base.Update();

		SceneModel.Update( Time.Delta );

		if ( BoneMerge )
		{
			var transform = SceneModel.GetBoneWorldTransform( 0 );
			GameObject.Transform.Position = transform.Position;
			GameObject.Transform.Rotation = transform.Rotation;
		}
	}

	protected override void OnPreRender()
	{
		if ( !_sceneModel.IsValid() )
			return;

		_sceneModel.Transform = Transform.World;
		_sceneModel.Bounds = new BBox(_bounds.Mins + Transform.Position, _bounds.Maxs + Transform.Position);
	}

	public void SetAnimParameter( string name, float value )
	{
		SceneModel?.SetAnimParameter( name, value );
	}
	
	public void SetAnimParameter( string name, bool value )
	{
		SceneModel?.SetAnimParameter( name, value );
	}

	public Transform? GetAttachment( string name, bool worldSpace = true )
	{
		return SceneModel.GetAttachment( name, worldSpace );
	}

	public void SetBodyGroup( string name, int value )
	{
		SceneModel?.SetBodyGroup( name, value );
	}
}
