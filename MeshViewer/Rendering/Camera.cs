using System;
using OpenTK;
using OpenTK.Input;

namespace MeshViewer.Rendering
{
    public class Camera
    {
        protected Vector3 _position = new Vector3(0, 100, 0);
        protected Vector3 _up = Vector3.UnitZ;
        protected Vector3 _direction;

        private Quaternion _orientation = Quaternion.Identity;

        private float _viewportWidth;
        private float _viewportHeight;
        private float _farClip = 1000.0f;
        private float _nearClip = 0.01f;

        private const float _pitchLimit = 1.4f;

        private const float _speed = 3.5f;
        private const float _mouseSpeedX = 0.0045f;
        private const float _mouseSpeedY = 0.0025f;

        private const float _speedBoost = 5.0f;

        protected MouseState m_prevMouse;

        public delegate void MovementHandler();

        public event MovementHandler OnMovement;

        /// <summary>
        /// Creates the instance of the camera.
        /// </summary>
        public Camera(float viewportWidth, float viewportHeight)
        {
            // Create the direction vector and normalize it since it will be used for movement
            _direction = Vector3.Zero - _position;
            _direction.Normalize();

            // Create default camera matrices
            View = CreateLookAt();
            SetViewport(_viewportWidth, _viewportHeight);
        }

        public void SetViewport(float width, float height)
        {
            _viewportHeight = height;
            _viewportWidth = width;
            Projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver2, width / height, _nearClip, _farClip);
        }

        public void SetFarClip(float farclip)
        {
            _farClip = farclip;
            Projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver2, _viewportWidth / _viewportHeight, _nearClip, _farClip);
        }

        /// <summary>
        /// Creates the instance of the camera at the given location.
        /// </summary>
        /// <param name="position">Position of the camera.</param>
        /// <param name="target">The target towards which the camera is pointing.</param>
        public Camera(Vector3 position, Vector3 target, int viewportWidth = 800, int viewPortHeight = 600)
        {
            _position = position;
            _direction = target - _position;
            _direction.Normalize();

            View = CreateLookAt();
            SetViewport(viewportWidth, viewPortHeight);
        }

        public void SetPosition(float x, float y, float z)
        {
            _position = new Vector3(x, y, z);
            View = CreateLookAt();
        }

        public void SetOrientation(Quaternion orientation)
        {
            if (_orientation == orientation)
                return;

            _orientation = orientation;
            _direction = Vector3.Transform(Vector3.UnitX, Matrix3.CreateFromQuaternion(orientation) * Matrix3.CreateFromAxisAngle(Vector3.UnitZ, (float)Math.PI / -2.0f));
            _direction.Normalize();

            View = CreateLookAt();
        }

        /// <summary>
        /// Handle the camera movement using user input.
        /// </summary>
        protected virtual bool ProcessInput()
        {
            var mouse = Mouse.GetState();
            var keyboard = Keyboard.GetState();

            var inputProcessed = false;

            // Move camera with WASD keys
            if (keyboard.IsKeyDown(Key.W))
            {
                _position += _direction * _speed * (keyboard.IsKeyDown(Key.LControl) ? _speedBoost : 1.0f);
                inputProcessed = true;
            }

            if (keyboard.IsKeyDown(Key.S))
            {
                _position -= _direction * _speed * (keyboard.IsKeyDown(Key.LControl) ? _speedBoost : 1.0f);
                inputProcessed = true;
            }

            if (keyboard.IsKeyDown(Key.A))
            {
                _position += Vector3.Cross(_up, _direction) * _speed * (keyboard.IsKeyDown(Key.LControl) ? _speedBoost : 1.0f); ;
                inputProcessed = true;
            }

            if (keyboard.IsKeyDown(Key.D))
            {
                _position -= Vector3.Cross(_up, _direction) * _speed * (keyboard.IsKeyDown(Key.LControl) ? _speedBoost : 1.0f); ;
                inputProcessed = true;
            }

            if (keyboard.IsKeyDown(Key.Space))
            {
                _position += _up * _speed * (keyboard.IsKeyDown(Key.LControl) ? _speedBoost : 1.0f); ;
                inputProcessed = true;
            }

            if (keyboard.IsKeyDown(Key.X))
            {
                _position -= _up * _speed * (keyboard.IsKeyDown(Key.LControl) ? _speedBoost : 1.0f); ;
                inputProcessed = true;
            }

            if (mouse.IsButtonDown(MouseButton.Left))
            {
                if (Math.Abs(mouse.X - m_prevMouse.X) > 0.01)
                {
                    _direction = Vector3.Transform(_direction,
                        Matrix3.CreateFromAxisAngle(_up,
                            -_mouseSpeedX * (mouse.X - m_prevMouse.X) * (keyboard.IsKeyDown(Key.LControl) ? _speedBoost : 1.0f)));
                    inputProcessed = true;
                }

                if (Math.Abs(mouse.Y - m_prevMouse.Y) > 0.01)
                {
                    var angle = _mouseSpeedY * (mouse.Y - m_prevMouse.Y) * (keyboard.IsKeyDown(Key.LControl) ? _speedBoost : 1.0f);
                    if ((Pitch < _pitchLimit || angle > 0) && (Pitch > -_pitchLimit || angle < 0))
                    {
                        _direction = Vector3.Transform(_direction, Matrix3.CreateFromAxisAngle(Vector3.Cross(_up, _direction), angle));
                        inputProcessed = true;
                    }
                }
            }

            m_prevMouse = mouse;
            return inputProcessed;
        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        public void Update()
        {
            // Handle camera movement
            if (ProcessInput())
            {
                View = CreateLookAt();
                OnMovement?.Invoke();
            }
        }


        /// <summary>
        /// Create a view (modelview) matrix using camera vectors.
        /// </summary>
        protected Matrix4 CreateLookAt()
        {
            return Matrix4.LookAt(_position, _position + _direction, _up);
        }


        /// <summary>
        /// Position vector.
        /// </summary>
        public Vector3 Position
        {
            get { return _position; }
        }

        /// <summary>
        /// Yaw of the camera in radians.
        /// </summary>
        public double Yaw
        {
            get { return Math.PI - Math.Atan2(_direction.X, _direction.Z); }
        }

        /// <summary>
        /// Pitch of the camera in radians.
        /// </summary>
        public double Pitch
        {
            get { return Math.Asin(_direction.Y); }
        }

        /// <summary>
        /// View (modelview) matrix accessor.
        /// </summary>
        public Matrix4 View;

        /// <summary>
        /// Projection matrix accessor.
        /// </summary>
        public Matrix4 Projection;

    }
}
