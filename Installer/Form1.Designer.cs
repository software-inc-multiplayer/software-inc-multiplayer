namespace Installer
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.installationDirTag = new System.Windows.Forms.Label();
            this.releasesList = new System.Windows.Forms.ComboBox();
            this.InstallButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.versionInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Yu Gothic", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(519, 47);
            this.label1.TabIndex = 0;
            this.label1.Text = "Multiplayer Mod Closed Beta";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Yu Gothic", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(15, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(249, 27);
            this.label2.TabIndex = 1;
            this.label2.Text = "Software Inc Installation";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(264, 80);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(102, 34);
            this.button1.TabIndex = 3;
            this.button1.Text = "Set Installation";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // installationDirTag
            // 
            this.installationDirTag.AutoSize = true;
            this.installationDirTag.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.installationDirTag.Location = new System.Drawing.Point(17, 117);
            this.installationDirTag.Name = "installationDirTag";
            this.installationDirTag.Size = new System.Drawing.Size(93, 13);
            this.installationDirTag.TabIndex = 4;
            this.installationDirTag.Text = "No installation set.";
            // 
            // releasesList
            // 
            this.releasesList.FormattingEnabled = true;
            this.releasesList.Location = new System.Drawing.Point(309, 190);
            this.releasesList.Name = "releasesList";
            this.releasesList.Size = new System.Drawing.Size(121, 21);
            this.releasesList.TabIndex = 5;
            this.releasesList.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // InstallButton
            // 
            this.InstallButton.Location = new System.Drawing.Point(437, 190);
            this.InstallButton.Name = "InstallButton";
            this.InstallButton.Size = new System.Drawing.Size(96, 21);
            this.InstallButton.TabIndex = 6;
            this.InstallButton.Text = "Install!";
            this.InstallButton.UseVisualStyleBackColor = true;
            this.InstallButton.Click += new System.EventHandler(this.InstallButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label3.Location = new System.Drawing.Point(261, 194);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Version";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 149);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(225, 65);
            this.label4.TabIndex = 8;
            this.label4.Text = "The mod is really experimental, and should be \r\nused without any other mods accom" +
    "panying it.\r\nThe closed beta is to test the backend, and \r\nprobably won\'t have g" +
    "ameplay.\r\n\r\n";
            // 
            // versionInfo
            // 
            this.versionInfo.AutoSize = true;
            this.versionInfo.ForeColor = System.Drawing.SystemColors.AppWorkspace;
            this.versionInfo.Location = new System.Drawing.Point(306, 163);
            this.versionInfo.Name = "versionInfo";
            this.versionInfo.Size = new System.Drawing.Size(104, 13);
            this.versionInfo.TabIndex = 9;
            this.versionInfo.Text = "No version selected.";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(564, 223);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.versionInfo);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.InstallButton);
            this.Controls.Add(this.releasesList);
            this.Controls.Add(this.installationDirTag);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label installationDirTag;
        private System.Windows.Forms.ComboBox releasesList;
        private System.Windows.Forms.Button InstallButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label versionInfo;
    }
}

