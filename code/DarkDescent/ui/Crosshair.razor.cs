﻿using System;
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

	public override void Tick()
	{
		InteractFailureOverlay.SetClass( "visible", !InteractPossible );
		InteractPossible = true;
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( Action );
	}
}
