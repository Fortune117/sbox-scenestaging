using System;
using Sandbox;
using Sandbox.UI;

namespace DarkDescent.UI;


public partial class Crosshair : Panel
{
	public static Crosshair Instance;
	public static bool InteractPossible { get; set; }
	
	public Panel AimPip { get; set; }
	
	private Panel InteractFailureOverlay { get; set; }
	public Panel CrosshairInternal { get; set; }
	private string Action { get; set; }

	public Crosshair()
	{
		Instance = this;
	}

	public static void SetVisible(bool b)
	{
		Instance.CrosshairInternal?.SetClass("visible", b);
	}

	public static void SetAimPipVisible( bool b )
	{
		Instance.AimPip?.SetClass( "visible", b );
	}

	public static void SetAimPipVector(Vector2 inputAngle)
	{
		if ( Instance?.AimPip is null )
			return;
		
		var angle = inputAngle.Degrees;
		
		if ( angle > 0 && angle < 180 )
		{
			angle = angle.Clamp( 35, 145 );
		}
		else
		{
			angle = angle.Clamp(215, 325);
		}

		angle -= 90;
			
		var transform = new PanelTransform();
		transform.AddRotation( new Vector3( 0, 0, angle ) );
		transform.AddTranslate( Length.Percent( -50 ), Length.Percent( -50 ) );

		Instance.AimPip.Style.TransformOriginX = Length.Percent( -100 );
		Instance.AimPip.Style.TransformOriginY = Length.Percent( -100 );
		Instance.AimPip.Style.Transform = transform;
	}

	public override void Tick()
	{
		InteractFailureOverlay.SetClass( "visible", !InteractPossible );
		InteractPossible = true;
	}

	protected override void OnAfterTreeRender( bool firstTime )
	{
		base.OnAfterTreeRender( firstTime );

		if ( firstTime )
			SetAimPipVisible( false );
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( Action );
	}
}
