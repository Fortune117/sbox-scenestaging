using System;
using Sandbox;

namespace DarkDescent.PostProcessing;

[Title( "Dark Descent Downscaler" )]
[Category( "Post Processing" )]
[Icon( "apps" )]
public class PixelatePostProcess : BaseComponent, BaseComponent.ExecuteInEditor
{
	[Property] 
	private float Resolution { get; set; } = 850;
	
	private readonly SceneCamera SceneCamera = new();
	private readonly RenderAttributes renderAttributes = new();
	private Texture texture;

	IDisposable renderHook;

	public override void OnEnabled()
	{
		renderHook?.Dispose();

		var cc = GetComponent<CameraComponent>( false, false );
		renderHook = cc.AddHookBeforeOverlay( "DDDownscaler", 500, RenderEffect );
	}

	public override void OnDisabled()
	{
		renderHook?.Dispose();
		renderHook = null;
	}

	RenderAttributes attributes = new RenderAttributes();

	public void RenderEffect( SceneCamera camera )
	{
		if ( !camera.EnablePostProcessing )
			return;
		
		var ratio = camera.Size.y / camera.Size.x;
			
		texture = Texture.CreateRenderTarget( "pixel_test", ImageFormat.RGBA8888, new Vector2( Resolution, Resolution * ratio ), texture );

		SceneCamera.Position = camera.Position;
		SceneCamera.Angles = camera.Angles;
		SceneCamera.AntiAliasing = false;
		SceneCamera.World = camera.World;
		SceneCamera.FieldOfView = camera.FieldOfView;
		SceneCamera.ZNear = camera.ZNear;
		SceneCamera.ZFar = camera.ZFar;
		SceneCamera.Size = camera.Size;
		SceneCamera.BackgroundColor = camera.BackgroundColor;
		SceneCamera.VolumetricFog.Enabled = true;
		SceneCamera.VolumetricFog.ContinuousMode = camera.VolumetricFog.ContinuousMode;
		SceneCamera.VolumetricFog.DrawDistance = camera.VolumetricFog.DrawDistance;
		SceneCamera.VolumetricFog.FadeInStart = camera.VolumetricFog.FadeInStart;
		SceneCamera.VolumetricFog.FadeInEnd = camera.VolumetricFog.FadeInEnd;
		SceneCamera.VolumetricFog.IndirectStrength = camera.VolumetricFog.IndirectStrength;
		SceneCamera.VolumetricFog.Anisotropy = camera.VolumetricFog.Anisotropy;
		SceneCamera.VolumetricFog.Scattering = camera.VolumetricFog.Scattering;
			
		Graphics.RenderToTexture( SceneCamera, texture );
		
		//Graphics.GrabFrameTexture( "render.target", renderAttributes, false );
			
		renderAttributes.Set("render.target", texture);

		Graphics.Blit( Material.FromShader( "shaders/postprocessing/dd_low_res.shader" ), renderAttributes);
	}
}
