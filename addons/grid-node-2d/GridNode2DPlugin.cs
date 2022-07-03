#if TOOLS

using Godot;
using System.ComponentModel;

namespace Picalines.Godot.GridNode
{
    [Tool]
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal sealed class GridNode2DPlugin : EditorPlugin
    {
        public override void _EnterTree()
        {
            base._EnterTree();

            var script = GD.Load<Script>("res://addons/grid-node-2d/GridNode2D.cs");

            var icon = GD.Load<Texture>("res://addons/grid-node-2d/GridNode2D.svg");

            AddCustomType(nameof(GridNode2D), nameof(Node2D), script, icon);
        }

        public override void _ExitTree()
        {
            base._ExitTree();

            RemoveCustomType(nameof(GridNode2D));
        }
    }
}

#endif