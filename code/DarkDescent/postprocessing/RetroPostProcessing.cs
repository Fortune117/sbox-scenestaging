using Sandbox;

namespace DarkDescent.PostProcessing;

[Title( "Retro Post Process" )]
[Category( "Post Processing" )]
[Icon( "apps" )]
public class RetroPostProcessing : BaseComponent, BaseComponent.ExecuteInEditor
{
	private static Material effectMaterial = Material.Load( "materials/postprocessing/dark_descent_retro_pp.vmat" );
	
	IDisposable renderHook;

	protected override void OnEnabled()
	{
		renderHook?.Dispose();

		var cc = Components.Get<CameraComponent>( true );
		renderHook = cc.AddHookBeforeOverlay( "RetroShader", 500, RenderEffect );
	}

	protected override void OnDisabled()
	{
		renderHook?.Dispose();
		renderHook = null;
	}

	RenderAttributes attributes = new();

	public void RenderEffect( SceneCamera camera )
	{
		if ( !camera.EnablePostProcessing )
			return;

		Graphics.GrabFrameTexture( "ColorBuffer", attributes );
		Graphics.GrabDepthTexture( "DepthBuffer", attributes );

		Graphics.Blit( effectMaterial, attributes );
	}
}
