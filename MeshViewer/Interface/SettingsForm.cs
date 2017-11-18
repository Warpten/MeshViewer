using System.Windows.Forms;

namespace MeshViewer.Interface
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();

            _pollingFrequency.Value = 5;
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            const int baseSpeed = 500;
            float pct = (float)_pollingFrequency.Value * 10 - 50;
            var pollingSpeed = baseSpeed * pct;
        }
    }
}
