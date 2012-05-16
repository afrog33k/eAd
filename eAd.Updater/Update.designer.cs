namespace eAd.Updater
{
partial class Update
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
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.line1 = new System.Windows.Forms.Label();
            this.line2 = new System.Windows.Forms.Label();
            this.lblPercentage = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 127);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(1039, 23);
            this.progressBar1.TabIndex = 0;
            // 
            // line1
            // 
            this.line1.AutoSize = true;
            this.line1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.line1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.line1.Location = new System.Drawing.Point(24, 9);
            this.line1.Name = "line1";
            this.line1.Size = new System.Drawing.Size(11, 16);
            this.line1.TabIndex = 3;
            this.line1.Text = " ";
            // 
            // line2
            // 
            this.line2.AutoSize = true;
            this.line2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.line2.Location = new System.Drawing.Point(12, 9);
            this.line2.Name = "line2";
            this.line2.Size = new System.Drawing.Size(0, 16);
            this.line2.TabIndex = 4;
            // 
            // lblPercentage
            // 
            this.lblPercentage.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lblPercentage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblPercentage.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPercentage.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblPercentage.Location = new System.Drawing.Point(1057, 108);
            this.lblPercentage.Name = "lblPercentage";
            this.lblPercentage.Size = new System.Drawing.Size(58, 52);
            this.lblPercentage.TabIndex = 5;
            this.lblPercentage.Text = "0%";
            this.lblPercentage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblPercentage.UseCompatibleTextRendering = true;
            // 
            // Update
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(1115, 162);
            this.ControlBox = false;
            this.Controls.Add(this.lblPercentage);
            this.Controls.Add(this.line2);
            this.Controls.Add(this.line1);
            this.Controls.Add(this.progressBar1);
            this.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "Update";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "eAd Update";
            this.Load += new System.EventHandler(this.Form1Load);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ProgressBar progressBar1;
    private System.Windows.Forms.Label line1;
    private System.Windows.Forms.Label line2;
    private System.Windows.Forms.Label lblPercentage;
}
}

