namespace SIL.WordWorks.GAFAWS.FW70Converter
{
	partial class Fw70PosSelectorDlg
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
			this._tvPoses = new System.Windows.Forms.TreeView();
			this.label1 = new System.Windows.Forms.Label();
			this._btnClose = new System.Windows.Forms.Button();
			this.SuspendLayout();
			//
			// _tvPoses
			//
			this._tvPoses.FullRowSelect = true;
			this._tvPoses.HideSelection = false;
			this._tvPoses.Location = new System.Drawing.Point(13, 38);
			this._tvPoses.Name = "_tvPoses";
			this._tvPoses.Size = new System.Drawing.Size(259, 257);
			this._tvPoses.TabIndex = 0;
			this._tvPoses.DoubleClick += new System.EventHandler(this.TvPosesDoubleClick);
			//
			// label1
			//
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(83, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Parts of Speech";
			//
			// _btnClose
			//
			this._btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this._btnClose.Location = new System.Drawing.Point(20, 313);
			this._btnClose.Name = "_btnClose";
			this._btnClose.Size = new System.Drawing.Size(75, 23);
			this._btnClose.TabIndex = 2;
			this._btnClose.Text = "Close";
			this._btnClose.UseVisualStyleBackColor = true;
			this._btnClose.Click += new System.EventHandler(this.BtnCloseClick);
			//
			// Fw70PosSelectorDlg
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 356);
			this.Controls.Add(this._btnClose);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._tvPoses);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Fw70PosSelectorDlg";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Select Part of Speech";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TreeView _tvPoses;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button _btnClose;
	}
}