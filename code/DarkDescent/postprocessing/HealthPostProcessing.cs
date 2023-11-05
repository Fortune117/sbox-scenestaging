using DarkDescent.Actor.Damage;
using DarkDescent.Actor.Marker;
using Sandbox;

namespace DarkDescent.PostProcessing;

public class HealthPostProcessing : BaseComponent, BaseComponent.ExecuteInEditor, IDamageTakenListener
{
		/// <summary>
	/// The color of the vignette or the "border"
	/// </summary>
	[Property] public Color Color { get; set; } = Color.Black;

	/// <summary>
	/// How strong the vignette is. This is a value between 0 -> 1
	/// </summary>
	[Property] public float Intensity { get; set; } = 1.0f;

	/// <summary>
	/// How much fall off or how blurry the vignette is
	/// </summary>
	[Property] public float Smoothness { get; set; } = 1.0f;

	/// <summary>
	/// How circular or round the vignette is
	/// </summary>
	[Property] public float Roundness { get; set; } = 1.0f;

	/// <summary>
	/// The center of the vignette in relation to UV space. This means
	/// a value of {0.5, 0.5} is the center of the screen
	/// </summary>
	[Property] public Vector2 Center { get; set; } = new Vector2( 0.5f, 0.5f );
	
	private float maximumSaturationLoss => 0.65f;
	private float healthEffectsThreshold => 0.6f;
	private float lowHealthMaxIntensity => 0.2f;

	private float damageIntensity = 0f;
	
	IDisposable renderHook;
	
	public override void OnEnabled()
	{
		renderHook?.Dispose();

		var cc = GetComponent<CameraComponent>( false, false );
		renderHook = cc.AddHookBeforeOverlay( "HealthPostProcess", 900, RenderEffect );
	}

	public override void OnDisabled()
	{
		renderHook?.Dispose();
		renderHook = null;
	}

	public override void Update()
	{
		base.Update();

		//damageIntensity = damageIntensity.LerpTo( 0, Time.Delta * 2f );
		Intensity = Intensity.LerpTo( 0, Time.Delta * 5f );
	}

	RenderAttributes attributes = new RenderAttributes();

	public void RenderEffect( SceneCamera camera )
	{
		if ( !camera.EnablePostProcessing )
			return;

		if ( Intensity.AlmostEqual( 0.0f ) )
			return;

		attributes.Set( "standard.vignette.color", Color );
		attributes.Set( "standard.vignette.intensity", Intensity );
		attributes.Set( "standard.vignette.smoothness", Smoothness );
		attributes.Set( "standard.vignette.roundness", Roundness );
		attributes.Set( "standard.vignette.center", Center );

		Graphics.GrabFrameTexture( "ColorBuffer", attributes );
		Graphics.GrabDepthTexture( "DepthBuffer", attributes );
		Graphics.Blit( Material.Load( "materials/postprocess/standard_pass3.vmat" ), attributes );
	}

	public void OnDamageTaken( DamageEventData damageEvent, bool isLethal )
	{
		if ( isLethal )
			return;

		Intensity = 0.75f;
	}
}
