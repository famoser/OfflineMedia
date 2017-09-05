namespace Famoser.OfflineMedia.Utils.TamediaRefresher
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.resultJson = new System.Windows.Forms.Label();
            this.outputTextBox = new System.Windows.Forms.RichTextBox();
            this.startButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.inputTextBox = new System.Windows.Forms.RichTextBox();
            this.browser = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // resultJson
            // 
            this.resultJson.AutoSize = true;
            this.resultJson.Location = new System.Drawing.Point(534, 9);
            this.resultJson.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.resultJson.Name = "resultJson";
            this.resultJson.Size = new System.Drawing.Size(81, 17);
            this.resultJson.TabIndex = 9;
            this.resultJson.Text = "json Output";
            // 
            // outputTextBox
            // 
            this.outputTextBox.Location = new System.Drawing.Point(537, 30);
            this.outputTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.Size = new System.Drawing.Size(408, 510);
            this.outputTextBox.TabIndex = 8;
            this.outputTextBox.Text = "";
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(429, 30);
            this.startButton.Margin = new System.Windows.Forms.Padding(4);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(100, 55);
            this.startButton.TabIndex = 7;
            this.startButton.Text = "correct JSON";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 17);
            this.label1.TabIndex = 11;
            this.label1.Text = "json Input";
            // 
            // inputTextBox
            // 
            this.inputTextBox.Location = new System.Drawing.Point(13, 30);
            this.inputTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.inputTextBox.Name = "inputTextBox";
            this.inputTextBox.Size = new System.Drawing.Size(408, 510);
            this.inputTextBox.TabIndex = 10;
            this.inputTextBox.Text = resources.GetString("inputTextBox.Text");
            // 
            // browser
            // 
            this.browser.Location = new System.Drawing.Point(13, 548);
            this.browser.MinimumSize = new System.Drawing.Size(20, 20);
            this.browser.Name = "browser";
            this.browser.Size = new System.Drawing.Size(932, 250);
            this.browser.TabIndex = 12;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(963, 816);
            this.Controls.Add(this.browser);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.inputTextBox);
            this.Controls.Add(this.resultJson);
            this.Controls.Add(this.outputTextBox);
            this.Controls.Add(this.startButton);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label resultJson;
        private System.Windows.Forms.RichTextBox outputTextBox;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox inputTextBox;
        private System.Windows.Forms.WebBrowser browser;
    }
}

