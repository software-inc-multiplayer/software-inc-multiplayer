using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Installer
{
    public partial class MainWindow : Form
    {
        public static ReleaseTag selectedRelease { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            if (String.IsNullOrWhiteSpace(Properties.Settings.Default.installDir))
            {
                installationDirTag.Text = "Game installation not set.";
            }
            else
            {
                installationDirTag.Text = Properties.Settings.Default.installDir;
            }
            IniteAsync();
        }
        public async void IniteAsync()
        {
            await GitHub.GetReleases();
            foreach (KeyValuePair<string, ReleaseTag> pair in GitHub.allowedReleases)
            {
                releasesList.Items.Add(pair.Key);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog f = new FolderBrowserDialog();   
            f.ShowNewFolderButton = false;
            if(f.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.installDir = f.SelectedPath;
                Properties.Settings.Default.Save();
                installationDirTag.Text = f.SelectedPath;
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (releasesList.SelectedItem == null) {
                releasesList.SelectedIndex = 0;
                return; 
            }
            string releasee = (string) releasesList.SelectedItem;
            bool exists = GitHub.allowedReleases.TryGetValue(releasee, out ReleaseTag release);
            if (!exists) return;
            var stable = "stable";
            if (release.PreRelease) stable = "unstable";
            if (release.Latest) stable = "latest";
            versionInfo.Text = $"{release.Tag} - {stable}";
            selectedRelease = release;
        }

        private async void InstallButton_Click(object sender, EventArgs e)
        {
            if(installationDirTag.Text == "Game installation not set.")
            {
                MessageBox.Show("Please set your game installation directory.", "No install directory set.");
                return;
            }
            if(selectedRelease == null)
            {
                MessageBox.Show("Please select a valid release from the dropdown next to the install button.", "No release selected.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            InstallButton.Enabled = false;
            releasesList.Enabled = false;
            button1.Enabled = false;
            await GitHub.InstallReleaseAsync(selectedRelease);
            InstallButton.Enabled = true;
            releasesList.Enabled = true;
            button1.Enabled = true;
            MessageBox.Show("Installed!", "Successfully installed the Multiplayer Mod");
        }
    }
}
