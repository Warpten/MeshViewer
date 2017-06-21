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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MeshViewer.Interface
{
    public partial class MainForm : Form
    {
        private CancellationTokenSource _clientUpdaterToken;

        #region Terrain Rendering
        private GeometryLoader GeometryLoader { get; set; }
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

            Game.Open(selectedProcess.ID);
            Game.OnUpdateTick += OnUpdateTick;

            Game.OnDespawn += _playerExplorer.OnDespawn;
            Game.OnUpdate += _playerExplorer.OnUpdate;

            Task.Factory.StartNew(() => {
                while (!_clientUpdaterToken.IsCancellationRequested)
                {
                    Game.Update();

                    if (Game.InGame && Game.LocalPlayer != null)
                        BeginInvoke((Action)(() => toolStripStatusLabel1.Text = $"Logged in as {Game.LocalPlayer.Name} (Map #{Game.CurrentMap})"));

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
            Game.OnUpdateTick -= OnUpdateTick;

            _clientUpdaterToken?.Token.Register(Game.Close);
            _clientUpdaterToken?.Cancel();
        }

        /// <summary>
        /// Called when the form is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private unsafe void OnFormLoad(object sender, EventArgs e)
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

            // GL.Enable(EnableCap.DebugOutput);
            // GL.Enable(EnableCap.DebugOutputSynchronous);
            /*GL.DebugMessageCallback((DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr messagePtr, IntPtr errorParam) =>
            {
                if (id == 131169 || id == 131185 || id == 131218 || id == 131204)
                    return;

                var oldForegroundColor = Console.ForegroundColor;

                var message = new string((sbyte*)messagePtr, 0, length, Encoding.ASCII);

                switch (severity)
                {
                    case DebugSeverity.DebugSeverityNotification:
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        break;
                    case DebugSeverity.DebugSeverityHigh:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case DebugSeverity.DebugSeverityMedium:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        break;
                    case DebugSeverity.DebugSeverityLow:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                }

                Console.WriteLine(message);
                Console.ForegroundColor = oldForegroundColor;
            }, IntPtr.Zero);*/

#endif
            glControl1.Paint += (_, __) => Render();
            glControl1.Resize += (_, __) => {
                GL.Viewport(0, 0, glControl1.Width, glControl1.Height);

                if (Game.Camera != null)
                    Game.Camera.AspectRatio = glControl1.Width / (float)glControl1.Height;
            };

            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Multisample);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            var terrainProgram = new ShaderProgram();
            terrainProgram.AddShader(ShaderType.VertexShader,   "./shaders/terrain.vert");
            terrainProgram.AddShader(ShaderType.FragmentShader, "./shaders/mixed.frag");
            terrainProgram.AddShader(ShaderType.GeometryShader, "./shaders/mixed.geom");
            terrainProgram.Link();
            ShaderProgramCache.Instance.Add("terrain", terrainProgram);

            var wmoProgram = new ShaderProgram();
            wmoProgram.AddShader(ShaderType.VertexShader,   "./shaders/wmo.vert");
            wmoProgram.AddShader(ShaderType.FragmentShader, "./shaders/mixed.frag");
            wmoProgram.AddShader(ShaderType.GeometryShader, "./shaders/mixed.geom");
            wmoProgram.Link();
            ShaderProgramCache.Instance.Add("wmo", wmoProgram);

            Texture = new TerrainTexture();
            #endregion
        }

        private void OnLoadGeometryRequest(object sender, EventArgs e) => LoadMap();

        private void OnOpenSettingsRequest(object sender, EventArgs e) => new SettingsForm().ShowDialog();

        private void OnUpdateTick()
        {
            _playerExplorer.SetDataSource(Game.Players);
            _unitExplorer.SetDataSource(Game.Units);
            _gameObjectExplorer.SetDataSource(Game.GameObjects);
        }

        #region Terrain Rendering
        private async void LoadMap()
        {
            GeometryLoader = await Task.Factory.StartNew(() => {
                return new GeometryLoader(@"D:\CATACLYSM DATA\", Game.CurrentMap);
            });

            glControl1.Invalidate();
        }

        private void Render()
        {
            if (InvokeRequired)
                BeginInvoke((Action)(() => Render()));
            else
            {
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, Texture.TextureID);

                if (Game.IsValid && GeometryLoader != null)
                {
                    var tileX = (int)Math.Floor(32 - Game.LocalPlayer.X / 533.3333f);
                    var tileY = (int)Math.Floor(32 - Game.LocalPlayer.Y / 533.3333f);
                    GeometryLoader.Render(tileY, tileX);
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
