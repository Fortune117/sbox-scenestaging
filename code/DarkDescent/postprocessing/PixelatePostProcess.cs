using Sandbox;

namespace DarkDescent.PostProcessing;

[SceneCamera.AutomaticRenderHook]
public class PixelatePostProcess : RenderHook
{
	public static PixelatePostProcess Instance { get; set; }
	
	private readonly SceneCamera SceneCamera = new();
	private readonly RenderAttributes renderAttributes = new();
	private Texture texture;

	public PixelatePostProcess()
	{
		Instance = this;
	}

	public override void OnStage( SceneCamera target, Stage renderStage )
	{
		base.OnStage( target, renderStage );
		
		if ( renderStage == Stage.BeforePostProcess )
		{
			var ratio = target.Size.y / target.Size.x;
			var res = 850;
			
			texture = Texture.CreateRenderTarget( "pixel_test", ImageFormat.RGBA8888, new Vector2( res, res * ratio ), texture );

			SceneCamera.Position = target.Position;
			SceneCamera.Angles = target.Angles;
			SceneCamera.AntiAliasing = false;
			SceneCamera.World = target.World;
			SceneCamera.FieldOfView = target.FieldOfView;
			SceneCamera.ZNear = target.ZNear;
			SceneCamera.ZFar = target.ZFar;
			SceneCamera.Size = target.Size;
			
			Graphics.RenderToTexture( SceneCamera, texture );
			
			renderAttributes.Set("render.target", texture);

			Graphics.Blit( Material.FromShader( "shaders/postprocessing/dd_low_res.shader" ), renderAttributes);
			
			Log.Info( "test" );
		}
	}
}
