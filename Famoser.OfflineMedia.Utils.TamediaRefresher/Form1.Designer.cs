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
            this.SuspendLayout();
            // 
            // resultJson
            // 
            this.resultJson.AutoSize = true;
            this.resultJson.Location = new System.Drawing.Point(607, 7);
            this.resultJson.Name = "resultJson";
            this.resultJson.Size = new System.Drawing.Size(61, 13);
            this.resultJson.TabIndex = 9;
            this.resultJson.Text = "json Output";
            // 
            // outputTextBox
            // 
            this.outputTextBox.Location = new System.Drawing.Point(610, 24);
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.Size = new System.Drawing.Size(577, 627);
            this.outputTextBox.TabIndex = 8;
            this.outputTextBox.Text = "";
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(529, 24);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 45);
            this.startButton.TabIndex = 7;
            this.startButton.Text = "correct JSON";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "json Input";
            // 
            // inputTextBox
            // 
            this.inputTextBox.Location = new System.Drawing.Point(10, 24);
            this.inputTextBox.Name = "inputTextBox";
            this.inputTextBox.Size = new System.Drawing.Size(508, 627);
            this.inputTextBox.TabIndex = 10;
            this.inputTextBox.Text = resources.GetString("inputTextBox.Text");
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1205, 663);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.inputTextBox);
            this.Controls.Add(this.resultJson);
            this.Controls.Add(this.outputTextBox);
            this.Controls.Add(this.startButton);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
    }
}

