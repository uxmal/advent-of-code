namespace Advent2024.Day06.Win;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        pictureBox = new PictureBox();
        ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
        SuspendLayout();
        // 
        // pictureBox
        // 
        pictureBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        pictureBox.Location = new Point(0, 0);
        pictureBox.Name = "pictureBox";
        pictureBox.Size = new Size(838, 654);
        pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
        pictureBox.TabIndex = 0;
        pictureBox.TabStop = false;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(13F, 32F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(899, 668);
        Controls.Add(pictureBox);
        Name = "Form1";
        Text = "Form1";
        Load += Form1_Load;
        ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private PictureBox pictureBox;
}
