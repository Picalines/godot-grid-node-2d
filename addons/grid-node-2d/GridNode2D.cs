using Godot;

namespace Picalines.Godot.GridNode
{
    [Tool]
    public class GridNode2D : Node2D
    {
        [Signal] public delegate void movement_started();
        [Signal] public delegate void movement_finished();

        /// <summary>
        /// The vector responsible for the shift of <see cref="Node2D"/> in
        /// <see cref="StartMoving(GridDirection2D)"/> and <see cref="MoveInstantly(GridDirection2D)"/>.
        /// <para>
        ///     Note that changing its value during movement will not affect
        ///     the target node position.
        /// </para>
        /// </summary>
        [Export]
        public Vector2 GridSize { get; set; } = new(16, 16);

        [Export(PropertyHint.Range, "0, 10, or_greater")]
        private float _MovementDuration = 1; // seconds

        /// <summary>
        /// The curve responsible for changing the position of the node. X and Y should range from 0 to 1
        /// <para>
        ///     Note that regardless of <see cref="MovementCurve"/> the <see cref="Node2D.Position"/>
        ///     will be assigned to <see cref="TargetPosition"/> when the movement is ended.
        /// </para>
        /// </summary>
        [Export]
        public Curve MovementCurve { get; set; } = CreateDefaultMovementCurve();

        /// <summary>
        /// Is true at the call of <see cref="movement_started"/> signal and
        /// before call of <see cref="movement_finished"/> signal.
        /// </summary>
        public bool IsMoving { get; private set; } = false;

        /// <summary>
        /// Direction of last started movement, value of argument passed to
        /// <see cref="StartMoving(GridDirection2D)"/> or <see cref="MoveInstantly(GridDirection2D)"/>.
        /// <para>
        ///     Equals <see cref="GridDirection2D.Up"/> before the first method call.
        /// </para>
        /// </summary>
        public GridDirection2D MovementDirection { get; private set; } = GridDirection2D.Up;

        /// <summary>
        /// Starting point of movement. Set in <see cref="StartMoving(GridDirection2D)"/>
        /// and <see cref="MoveInstantly(GridDirection2D)"/>.
        /// <para>
        ///     Set to <see cref="Node2D.Position"/> in <see cref="_Ready"/>
        /// </para>
        /// </summary>
        public Vector2 StartPosition { get; private set; }

        /// <summary>
        /// Target point of movement. Set in <see cref="StartMoving(GridDirection2D)"/>
        /// and <see cref="MoveInstantly(GridDirection2D)"/>.
        /// <para>
        ///     Set to <see cref="Node2D.Position"/> in <see cref="_Ready"/>
        /// </para>
        /// </summary>
        public Vector2 TargetPosition { get; private set; }

        /// <summary>
        /// Duration of the movement in seconds. Cannot be set to negative number.
        /// <para>
        ///     Even with a value of 0, the movement will end only in <see cref="_Process(float)"/>.
        ///     See <see cref="MoveInstantly(GridDirection2D)"/>.
        /// </para>
        /// </summary>
        public float MovementDuration
        {
            get => _MovementDuration;
            set => _MovementDuration = Mathf.Min(value, 0);
        }

        private float _TimeFromStart;

        /// <summary>
        /// Assigns the initial values for <see cref="StartPosition"/> and <see cref="TargetPosition"/>
        /// </summary>
        public override void _Ready()
        {
            base._Ready();

            StartPosition = TargetPosition = Position;
        }

        /// <summary>
        /// Returns the offset vector along which the node would move in given <paramref name="direction"/>.
        /// </summary>
        public Vector2 GetMovementOffset(GridDirection2D direction)
        {
            return direction.ToVector() * GridSize;
        }

        /// <summary>
        /// Starts the movement in given <paramref name="direction"/>. Ignored when <see cref="IsMoving"/> is true.
        /// Emits the <see cref="movement_started"/> signal.
        /// </summary>
        public void StartMoving(GridDirection2D direction)
        {
            if (IsMoving)
            {
                return;
            }

            MovementDirection = direction;
            StartPosition = Position;
            TargetPosition = Position + GetMovementOffset(direction);

            _TimeFromStart = 0;

            IsMoving = true;
            EmitSignal(nameof(movement_started));
        }

        /// <summary>
        /// Stops the current movement if present. Keeps the current <see cref="Node2D.Position"/>.
        /// Emits the <see cref="movement_finished"/> signal.
        /// </summary>
        public void StopMoving()
        {
            if (!IsMoving)
            {
                return;
            }

            IsMoving = false;
            EmitSignal(nameof(movement_finished));
        }

        /// <summary>
        /// Puts the node in <see cref="TargetPosition"/> if <see cref="IsMoving"/>.
        /// Emits the <see cref="movement_finished"/> signal.
        /// </summary>
        public void SkipMovement()
        {
            if (!IsMoving)
            {
                return;
            }

            Position = TargetPosition;
            StopMoving();
        }

        /// <summary>
        /// Performs the movement inside one method call.
        /// </summary>
        public void MoveInstantly(GridDirection2D direction)
        {
            StartMoving(direction);
            SkipMovement();
        }

        /// <summary>
        /// Sets <see cref="Node2D.Position"/> based on <see cref="MovementCurve"/>.
        /// </summary>
        public override void _Process(float delta)
        {
            base._Process(delta);

            if (!IsMoving)
            {
                return;
            }

            if ((_TimeFromStart += delta) >= _MovementDuration)
            {
                SkipMovement();
                return;
            }

            float progress = _TimeFromStart / _MovementDuration;
            float weight = MovementCurve.Interpolate(progress);

            Position = StartPosition.LinearInterpolate(TargetPosition, weight);
        }

        private static Curve CreateDefaultMovementCurve()
        {
            var curve = new Curve();

            curve.AddPoint(Vector2.Zero, rightTangent: 1);
            curve.AddPoint(Vector2.One, leftTangent: 1);

            return curve;
        }
    }
}
