using Sandbox.Razor;
using Sandbox.UI;
using Sandbox.UI.Tests;

namespace DarkDescent.GameLog.UI;

public partial class GameLogPanel
{
	private static GameLogPanel Instance { get; set; }
	private VirtualScrollPanel Entries { get; set; }
	
	public GameLogPanel()
	{
		Instance = this;
	}
	
	protected override void OnAfterTreeRender( bool firstTime )
	{
		base.OnAfterTreeRender( firstTime );

		if ( !firstTime )
			return;
		
		Entries.Layout.ItemHeight = 25;
		Entries.OnCreateCell = ( cell, data ) =>
		{
			var entry = new Label();
			entry.SetClass( "entry", true );
			entry.Parent = cell;
			entry.Text = (string)data;
		};
	}

	public static void AddEntry( string message )
	{
		Instance.Entries.AddItem( message );
		Instance.Entries.TryScrollToBottom();
		Instance.Entries.StateHasChanged();
	}
}
