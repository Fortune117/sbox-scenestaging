﻿public static class EditorScene
{
	public static string PlayMode { get; set; } = "scene";
	public static Scene Active { get; private set; }
	public static List<Scene> OpenScenes { get; set; } = new();

	internal static Gizmo.Instance GizmoInstance { get; private set; }

	public static SelectionSystem Selection => GizmoInstance.Selection;

	static EditorScene()
	{
		GizmoInstance = new Gizmo.Instance();

		NewScene();
	}

	public static void NewScene()
	{
		Active = new Scene();
		Active.Name = "Untitled Scene";
		Active.IsEditor = true;

		OpenScenes.Add( Active );

		// create default scene

		{
			var go = Active.CreateObject();
			go.Name = "Main Camera";
			go.Transform = new Transform( Vector3.Up * 100 + Vector3.Backward * 300 );
			go.AddComponent<CameraComponent>();
		}

		{
			var go = Active.CreateObject();
			go.Name = "Directional Light";
			go.Transform = new Transform( Vector3.Up * 200, Rotation.From( 80, 45, 0 ) );
			go.AddComponent<DirectionalLightComponent>();
		}

		EditorEvent.Run( "scene.open" );
	}

	public static bool SwitchActive( Scene scene )
	{
		if ( Active == scene )
			return false;

		Active = scene;
		EditorWindow.DockManager.RaiseDock( "Scene" );
		UpdateEditorTitle();
		return true;
	}

	static Scene previousActiveScene;

	public static void Play()
	{
		GameManager.IsPlaying = true;

		if ( PlayMode == "scene" )
		{
			var current = EditorScene.Active.Save();
			GameManager.ActiveScene = new Scene();
			GameManager.ActiveScene.Load( current );
		}
		else
		{
			Program.Main();
		}

		// switch editor to active scene
		previousActiveScene = EditorScene.Active;
		EditorScene.Active = GameManager.ActiveScene;

		Camera.Main.World = GameManager.ActiveScene.SceneWorld;
		Camera.Main.Worlds.Clear();
		Camera.Main.Worlds.Add( GameManager.ActiveScene.DebugSceneWorld );
		Camera.Main.Position = 0;
		Camera.Main.Rotation = Rotation.From( 0, 0, 0 );
		Camera.Main.ZNear = 1;
		Camera.Main.Tonemap.Enabled = true;
		Camera.Main.Tonemap.MinExposure = 0.1f;
		Camera.Main.Tonemap.MaxExposure = 2.0f;
		Camera.Main.Tonemap.Rate = 1.0f;
		Camera.Main.Tonemap.Fade = 1.0f;

		EditorWindow.DockManager.RaiseDock( "GameFrame" );
	}

	public static void Stop()
	{
		GameManager.IsPlaying = false;
		GameManager.ActiveScene = null;

		if ( OpenScenes.Contains( previousActiveScene ) )
		{
			EditorScene.Active = previousActiveScene;
		}
		else
		{
			EditorScene.Active = OpenScenes.FirstOrDefault();
		}

		EditorWindow.DockManager.RaiseDock( "Scene" );
		SceneEditorTick();
	}

	/// <summary>
	/// Called once a frame to keep the game camera in sync with the main camera in the editor scene
	/// </summary>
	[EditorEvent.Frame]
	public static void SceneEditorTick()
	{
		if ( Active is null ) return;
		if ( Camera.Main is null ) return;

		//
		// If we're not playing, then position the game's main camera where the first CameraComponent is
		//
		if ( !GameManager.IsPlaying )
		{
			var camera = Active.FindAllComponents<CameraComponent>( false ).FirstOrDefault();

			if ( camera is not null )
			{
				camera.UpdateCamera( Camera.Main );
			}
		}

		//
		// If this is an editor scene, tick it to flush deleted objects etc
		//
		if ( Active.IsEditor )
		{
			Active.GameTick();
		}
	}

	/// <summary>
	/// This is called when the user wants to open a new scene
	/// </summary>

	[EditorForAssetType( "scene" )]
	public static void LoadFromScene( SceneFile resource )
	{
		Assert.NotNull( resource, "SceneFile should not be null" );

		// 
		// TODO: Unsaved changes test
		//

		// is this scene already in OpenScenes?

		Active = new Scene();
		using ( Active.Push() )
		{
			Active.Name = resource.ResourceName.ToTitleCase();
			Active.IsEditor = true;

			Active.Load( resource );

			OpenScenes.Add( Active );

			UpdateEditorTitle();

			EditorEvent.Run( "scene.open" );

		}
	}

	/// <summary>
	/// This is called when the user wants to open a new scene
	/// </summary>

	[EditorForAssetType( PrefabFile.FileExtension )]
	public static void LoadFromPrefab( PrefabFile resource )
	{
		// 
		// TODO: Unsaved changes test
		//

		var prefabScene = new PrefabScene();

		OpenScenes.Add( prefabScene );
		Active = prefabScene;

		new SceneSunLight( Active.SceneWorld, Rotation.From( 80, 45, 0 ), Color.White * 0.5f );

		using ( Active.Push() )
		{
			prefabScene.Name = resource.ResourceName.ToTitleCase();
			prefabScene.IsEditor = true;
			prefabScene.Load( resource );

			UpdateEditorTitle();

			EditorWindow.DockManager.RaiseDock( "Scene" );
			EditorEvent.Run( "scene.open" );
		}
	}

	public static void CloseScene( Scene scene )
	{
		// SAVE CHANGES???

		if ( scene == Active )
		{
			var index = OpenScenes.IndexOf( scene );
			if ( index >= 0 && OpenScenes.Count > 1 )
			{
				if ( index > 0 ) index--;
				else index++;

				Active = OpenScenes[index];
			}
		}

		OpenScenes.Remove( scene );
	}

	static void UpdateEditorTitle()
	{
		if ( Active is not null )
		{
			var name = Active.Name;
			if ( Active.HasUnsavedChanges ) name += "*";

			EditorWindow.UpdateEditorTitle( name );
			return;
		}

		EditorWindow.UpdateEditorTitle( "Smile Face" );
	}
}
