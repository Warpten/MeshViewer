using MeshViewer.Geometry;
using MeshViewer.Interface.Controls;
using MeshViewer.Memory;
using MeshViewer.Memory.Entities;
using MeshViewer.Memory.Enums;
using MeshViewer.Properties;
using MeshViewer.Rendering;
using MeshViewer.Rendering.Textures;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MeshViewer.Interface
{
    public partial class MainForm : Form
    {
        private CancellationTokenSource _clientUpdaterToken;
        private Process _game;

        #region Terrain Rendering
        private GeometryLoader GeometryLoader { get; set; }
        public Camera Camera { get; private set; }
        public TerrainTexture Texture { get; private set; }
        #endregion

        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Called when the user wants to start reading a process's memory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAttachRequest(object sender, EventArgs e)
        {
            if (_wowComboBox.SelectedIndex == -1)
                return;

            var selectedProcess = _wowComboBox.Items[_wowComboBox.SelectedIndex] as ProcessIdentifier;
            if (selectedProcess == null)
                return;

            _clientUpdaterToken?.Cancel();
            _clientUpdaterToken = new CancellationTokenSource();

            _game = new Process(selectedProcess.ID);
            _game.Camera = new CGCamera_C(_game);
            _game.Manager = new ObjectMgr(_game);
            _game.Manager.OnUpdateTick += OnUpdateTick;

            _game.Manager.OnDespawn += _playerExplorer.OnDespawn;
            _game.Manager.OnUpdate += _playerExplorer.OnUpdate;

            Task.Factory.StartNew(() => {
                while (!_clientUpdaterToken.IsCancellationRequested)
                {
                    _game.Manager.Update();

                    // if (_game.Manager.InGame && _game.Manager.LocalPlayer != null)
                    //     BeginInvoke((Action)(() => toolStripStatusLabel1.Text = $"Logged in as {_game.Manager.LocalPlayer.Name} (Map #{_game.Manager.CurrentMap})"));

                    glControl1.Invalidate();
                    _clientUpdaterToken.Token.WaitHandle.WaitOne(ObjectMgr.UpdateFrequency);
                }

            }, _clientUpdaterToken.Token);
        }

        /// <summary>
        /// Detaches from the client.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFormClose(object sender, FormClosingEventArgs e)
        {
            if (_game != null)
                _game.Manager.OnUpdateTick -= OnUpdateTick;

            _clientUpdaterToken?.Token.Register(() => {
                _game = null;
            });
            _clientUpdaterToken?.Cancel();
        }

        /// <summary>
        /// Called when the form is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFormLoad(object sender, EventArgs e)
        {
            #region Process List
            _wowComboBox.BeginUpdate();
            _wowComboBox.Items.Clear();
            var processList = System.Diagnostics.Process.GetProcessesByName("Wow").Select(p => new ProcessIdentifier { Name = p.ProcessName, ID = p.Id }).ToArray();
            _wowComboBox.Items.AddRange(processList);
            _wowComboBox.EndUpdate();
            #endregion

            #region Object viewers
            _gameObjectExplorer.SetRenderer<ObjectListRenderer<CGGameObject_C>, CGGameObject_C>(new GameobjectListRenderer());
            _gameObjectExplorer.SetFilterSource<GameObjectType>();
            _gameObjectExplorer.SetFilter<CGGameObject_C>((gameobject) =>
            {
                if (string.IsNullOrEmpty(_gameObjectExplorer.FilterValue))
                    return true;
                return gameobject.ObjectType.ToString() == _gameObjectExplorer.FilterValue;
            });

            _unitExplorer.SetRenderer<ObjectListRenderer<CGUnit_C>, CGUnit_C>(new EntityListRenderer());
            
            _playerExplorer.SetRenderer<ObjectListRenderer<CGUnit_C>, CGUnit_C>(new EntityListRenderer());
            _playerExplorer.SetFilterSource<Class>();
            _playerExplorer.SetFilter<CGUnit_C>((player) =>
            {
                if (string.IsNullOrEmpty(_playerExplorer.FilterValue))
                    return true;
                return player.Class.ToString() == _playerExplorer.FilterValue;
            });
            #endregion

            #region Tab images
            tabControl1.ImageList = new ImageList() {
                ImageSize = new Size(20, 20)
            };
            tabControl1.ImageList.Images.AddRange(new[]
            {
                Resources.Map,
                Resources.Person,
                Resources.Monster
            });
            #endregion

            #region Rendering
#if DEBUG
            glControl1.Context.ErrorChecking = true;
#endif
            glControl1.Paint += (_, __) => Render();
            glControl1.Resize += (_, __) => {
                GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
                Camera?.SetViewport(glControl1.Width, glControl1.Height);
            };

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.ClearColor(0.3f, 0.3f, 0.32f, 1.0f);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            GL.Enable(EnableCap.DepthTest);
            GL.CullFace(CullFaceMode.Back);

            var terrainProgram = new ShaderProgram();
            terrainProgram.AddVertexShader("./shaders/terrain.vertex.shader");
            terrainProgram.AddFragmentShader("./shaders/terrain.fragment.shader");
            terrainProgram.Link();
            ShaderProgramCache.Instance.Add("terrain", terrainProgram);

            var wmoProgram = new ShaderProgram();
            wmoProgram.AddVertexShader("./shaders/wmo.vertex.shader");
            wmoProgram.AddFragmentShader("./shaders/wmo.fragment.shader");
            wmoProgram.Link();
            ShaderProgramCache.Instance.Add("wmo", wmoProgram);

            Texture = new TerrainTexture();
            #endregion
        }

        private void OnLoadGeometryRequest(object sender, EventArgs e)
        {
            if (_game != null)
                LoadMap();
        }

        private void OnOpenSettingsRequest(object sender, EventArgs e) => new SettingsForm().ShowDialog();

        private void OnUpdateTick()
        {
            _playerExplorer.SetDataSource(_game.Manager.Players);
            _unitExplorer.SetDataSource(_game.Manager.Units);
            _gameObjectExplorer.SetDataSource(_game.Manager.GameObjects);
        }

        #region Terrain Rendering
        private async void LoadMap()
        {
            GeometryLoader = await Task.Factory.StartNew(() => {
                return new GeometryLoader(@"D:\Repositories\omfg.gg\Build\bin\RelWithDebInfo", _game.Manager.CurrentMap);
            });

            var orientation = Matrix3.CreateFromQuaternion(_game.Camera.Matrix.ExtractRotation());
            Camera = new Camera(new Vector3(_game.Camera.X, _game.Camera.Y, _game.Camera.Z),
                Vector3.Transform(Vector3.UnitZ, orientation),
                glControl1.Width, glControl1.Height);

            glControl1.Invalidate();
        }

        private void Render()
        {
            if (InvokeRequired)
                BeginInvoke((Action)(() => Render()));
            else
            {
                if (Camera != null)
                    Camera.Update();

                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, Texture.TextureID);

                if (_game?.Manager != null && GeometryLoader != null)
                {
                    var tileX = (int)Math.Floor(32 - _game.LocalPlayer.X / 533.3333f);
                    var tileY = (int)Math.Floor(32 - _game.LocalPlayer.Y / 533.3333f);
                    GeometryLoader.Render(tileY, tileX, Camera);
                }

                glControl1.SwapBuffers();
            }
        }
        #endregion

        private void OnScreenshotRequest(object sender, EventArgs e)
        {
            var openFileDialog = new SaveFileDialog() {
                AddExtension = true,
                Filter = "PNG File (*.png)|*.png|JPEG File (*.jpeg, *.jpg)|*.jpeg|BMP File (*.bmp)|*.bmp"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var bmp = new Bitmap(glControl1.ClientSize.Width, glControl1.ClientSize.Height);
                var data = bmp.LockBits(glControl1.ClientRectangle, ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                GL.ReadPixels(0, 0, glControl1.ClientSize.Width, glControl1.ClientSize.Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
                bmp.UnlockBits(data);
                bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

                switch (Path.GetExtension(openFileDialog.FileName))
                {
                    case "bmp":
                        bmp.Save(openFileDialog.FileName, ImageFormat.Bmp);
                        return;
                    case "jpeg":
                    case "jpg":
                        bmp.Save(openFileDialog.FileName, ImageFormat.Jpeg);
                        return;
                    case "png":
                        bmp.Save(openFileDialog.FileName, ImageFormat.Png);
                        return;
                }
            }
        }

        private class ProcessIdentifier
        {
            public string Name { get; set; }
            public int ID { get; set; }

            public override string ToString() => $"{Name} [PID: {ID}]";
        }
    }
}
