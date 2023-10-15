using Sandbox;
using Sandbox.Effects;

namespace DarkDescent.PostProcessing;

[SceneCamera.AutomaticRenderHook]
public class StaminaPostProcessHook : ScreenEffects
{
	public static StaminaPostProcessHook Instance { get; set; }

	private float vignetteIntensity = 0f;
	

	public StaminaPostProcessHook()
	{
		Instance = this;
		Initialize();
	}
	
	[Event.Hotload]
	private void Initialize()
	{
		Vignette.Smoothness = 1f;
		Vignette.Roundness = 0f;
		Vignette.Color = Color.Black;
	}
	

	public override void OnFrame( SceneCamera target )
	{
		UpdateVignette();
	}
	

	private float GetLocalPlayerStaminaFraction()
	{
		return 1f;
	}

	private float cutOff => 0.4f;
	private void UpdateVignette()
	{
		var strength = 1 - GetLocalPlayerStaminaFraction();

		if ( strength > (1 - cutOff) )
		{
			vignetteIntensity = vignetteIntensity.LerpTo( strength.Remap( 1 - cutOff, 1f, 0f, 0.8f ), Time.Delta * 5f );
		}
		else
		{
			vignetteIntensity = vignetteIntensity.LerpTo( 0f, Time.Delta );
		}

		Vignette.Intensity = vignetteIntensity;
	}
}
