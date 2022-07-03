using Godot;
using System.ComponentModel;

namespace Picalines.Godot.GridNode
{
    public enum GridDirection2D
    {
        Up,
        UpLeft,
        UpRight,

        Down,
        DownLeft,
        DownRight,

        Left,
        Right,
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class GridDirection2DExtensions
    {
        private static readonly Vector2 _UpLeft = Vector2.Up + Vector2.Left;
        private static readonly Vector2 _UpRight = Vector2.Up + Vector2.Right;

        private static readonly Vector2 _DownLeft = Vector2.Down + Vector2.Left;
        private static readonly Vector2 _DownRight = Vector2.Down + Vector2.Right;

        /// <summary>
        /// Converts <see cref="GridDirection2D"/> member to <see cref="Vector2"/> instance.
        /// <para>
        ///     For example, <see cref="GridDirection2D.Up"/> is converted to <see cref="Vector2.Up"/>. Note that for
        ///     <see cref="GridDirection2D.UpRight"/> the method will return the sum of <see cref="Vector2.Up"/> and
        ///     <see cref="Vector2.Right"/>, so it will not be normalized.
        /// </para>
        /// </summary>
        public static Vector2 ToVector(this GridDirection2D direction) => direction switch
        {
            GridDirection2D.Up => Vector2.Up,
            GridDirection2D.UpLeft => _UpLeft,
            GridDirection2D.UpRight => _UpRight,

            GridDirection2D.Down => Vector2.Down,
            GridDirection2D.DownLeft => _DownLeft,
            GridDirection2D.DownRight => _DownRight,

            GridDirection2D.Left => Vector2.Left,
            GridDirection2D.Right => Vector2.Right,

            _ => throw new System.NotImplementedException(),
        };

        // Dot(Vector((0, 1)), Vector((Cos(3pi/8), Sin(3pi/8))))
        private const float MinUpDot = 0.9238795325113f;

        // Dot(Vector((0, 1)), Vector((Cos(pi/8), Sin(pi/8))))
        private const float MaxRightDot = 0.3826834323651f;

        /// <summary>
        /// Converts <see cref="Vector2"/> to <see cref="GridDirection2D"/> member.
        /// <para>
        ///     Method computes the dot product of the normalized <paramref name="vector2"/> and <see cref="Vector2.Up"/>.
        ///     Using the dot product, the method determines in which of the eight regions of the circle the <paramref name="vector2"/> "lies".
        /// </para>
        /// <para>
        ///     For <see cref="Vector2.Zero"/> the method will return <see cref="GridDirection2D.Up"/>.
        /// </para>
        /// </summary>
        public static GridDirection2D ToGridDirection(this Vector2 vector2)
        {
            if (vector2.x == 0)
            {
                return vector2.y <= 0 ? GridDirection2D.Up : GridDirection2D.Down;
            }

            if (vector2.x < 0)
            {
                return new Vector2(-vector2.x, vector2.y).ToGridDirection().MirrorHorizontal();
            }

            return Vector2.Up.Dot(vector2.Normalized()) switch
            {
                <= -MinUpDot => GridDirection2D.Down,
                <= -MaxRightDot => GridDirection2D.DownRight,
                <= MaxRightDot => GridDirection2D.Right,
                <= MinUpDot => GridDirection2D.UpRight,
                _ => GridDirection2D.Up,
            };
        }

        /// <summary>
        /// Returns the opposite <see cref="GridDirection2D"/>.
        /// <para>
        ///     For example for <see cref="GridDirection2D.Right"/> the method will return <see cref="GridDirection2D.Left"/>,
        ///     for <see cref="GridDirection2D.UpRight"/> it will return <see cref="GridDirection2D.DownLeft"/> and so on.
        /// </para>
        /// </summary>
        public static GridDirection2D Mirror(this GridDirection2D direction) => direction switch
        {
            GridDirection2D.Up => GridDirection2D.Down,
            GridDirection2D.UpLeft => GridDirection2D.DownRight,
            GridDirection2D.UpRight => GridDirection2D.DownLeft,
            GridDirection2D.Down => GridDirection2D.Up,
            GridDirection2D.DownLeft => GridDirection2D.UpRight,
            GridDirection2D.DownRight => GridDirection2D.UpLeft,
            GridDirection2D.Left => GridDirection2D.Right,
            GridDirection2D.Right => GridDirection2D.Left,

            _ => throw new System.NotImplementedException(),
        };

        /// <summary>
        /// Returns another <see cref="GridDirection2D"/> mirrored on the horizontal axis.
        /// <para>
        ///     For example for <see cref="GridDirection2D.Right"/> the method will return <see cref="GridDirection2D.Left"/>,
        ///     but for <see cref="GridDirection2D.Up"/> it will return the same <see cref="GridDirection2D.Up"/>.
        /// </para>
        /// </summary>
        public static GridDirection2D MirrorHorizontal(this GridDirection2D direction) => direction switch
        {
            (GridDirection2D.Up or GridDirection2D.Down) and var vertical => vertical,

            GridDirection2D.UpLeft => GridDirection2D.UpRight,
            GridDirection2D.UpRight => GridDirection2D.UpLeft,
            GridDirection2D.DownLeft => GridDirection2D.DownRight,
            GridDirection2D.DownRight => GridDirection2D.DownLeft,
            GridDirection2D.Left => GridDirection2D.Right,
            GridDirection2D.Right => GridDirection2D.Left,

            _ => throw new System.NotImplementedException(),
        };

        /// <summary>
        /// Returns another <see cref="GridDirection2D"/> mirrored on the vertical axis.
        /// <para>
        ///     For example for <see cref="GridDirection2D.Up"/> the method will return <see cref="GridDirection2D.Down"/>,
        ///     but for <see cref="GridDirection2D.Right"/> it will return the same <see cref="GridDirection2D.Right"/>.
        /// </para>
        /// </summary>
        public static GridDirection2D MirrorVertical(this GridDirection2D direction) => direction switch
        {
            (GridDirection2D.Right or GridDirection2D.Left) and var horizontal => horizontal,

            GridDirection2D.Up => GridDirection2D.Down,
            GridDirection2D.UpLeft => GridDirection2D.DownLeft,
            GridDirection2D.UpRight => GridDirection2D.DownRight,
            GridDirection2D.Down => GridDirection2D.Up,
            GridDirection2D.DownLeft => GridDirection2D.UpLeft,
            GridDirection2D.DownRight => GridDirection2D.UpRight,

            _ => throw new System.NotImplementedException(),
        };
    }
}
