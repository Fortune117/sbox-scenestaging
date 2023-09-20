﻿
using Editor;
using Sandbox;
using System;
using System.Linq;

public partial class GameObjectNode : TreeNode<GameObject>
{
	public GameObjectNode( GameObject o ) : base ( o )
	{

	}

	public override bool HasChildren => Value.Children.Any();

	protected override void BuildChildren()
	{
		var children = Children.ToList();

		foreach ( var child in Value.Children )
		{
			var c = children.OfType<GameObjectNode>().FirstOrDefault( x => x.Value == child );
			if ( c == null )
			{
				AddItem( new GameObjectNode( child ) );
			}
			else
			{
				children.Remove( c );
			}
		}

		foreach ( var child in children )
		{
			RemoveItem( child );
		}

	}

	public override int ValueHash => HashCode.Combine( Value, Value.Children.Count, Value.Name );

	public override void OnPaint( VirtualWidget item )
	{
		var fullSpanRect = item.Rect;
		fullSpanRect.Left = 0;
		fullSpanRect.Right = TreeView.Width;

		if ( item.Selected )
		{
			//item.PaintBackground( Color.Transparent, 3 );
			Paint.ClearPen();
			Paint.SetBrush( Theme.Blue.WithAlpha( 0.4f ) );
			Paint.DrawRect( fullSpanRect );

			Paint.SetPen( Color.White );
		}
		else
		{
			Paint.SetPen( Theme.ControlText );
		}

		var name = Value.Name;
		if ( string.IsNullOrWhiteSpace( name ) ) name = "Untitled GameObject";

		var r = item.Rect;
		r.Left += 4;
		 
		if ( !item.Selected ) Paint.SetPen( Theme.Blue );
		Paint.DrawIcon( r, "circle", 14, TextFlag.LeftCenter );
		r.Left += 22;

		Paint.SetPen( item.Selected ? Theme.White : Theme.ControlText );
		Paint.SetDefaultFont( 9 );
		Paint.DrawText( r, name, TextFlag.LeftCenter );
	}

	public override void OnSelectionChanged( bool state )
	{
		base.OnSelectionChanged( state );

		if ( state )
		{
			EditorUtility.InspectorObject = Value;
		}
	}

	public override bool OnDragStart()
	{
		var drag = new Drag( TreeView );
		//drag.Data.Text = Json.Serialize( Value.Serialize() );
		drag.Data.Object = Value;
		drag.Execute();

		return true;
	}

	public override void OnDragHover( Widget.DragEvent ev )
	{
		if ( ev.Data.Object is GameObject go )
		{
			ev.Action = DropAction.Move;
			return;
		}

		base.OnDragHover( ev );
	}

	public override void OnDrop( Widget.DragEvent ev )
	{
		if ( ev.Data.Object is GameObject go )
		{
			ev.Action = DropAction.Move;
			go.Parent = Value;
			return;
		}

		base.OnDrop( ev );
	}
}

