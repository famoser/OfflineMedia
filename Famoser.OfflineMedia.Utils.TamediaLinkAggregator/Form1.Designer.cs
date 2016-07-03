namespace Famoser.OfflineMedia.Utils.TamediaLinkAggregator
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
            this.jsonInput = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.jsonOutput = new System.Windows.Forms.RichTextBox();
            this.json = new System.Windows.Forms.Label();
            this.resultJson = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.htmlInput = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // jsonInput
            // 
            this.jsonInput.Location = new System.Drawing.Point(12, 31);
            this.jsonInput.Name = "jsonInput";
            this.jsonInput.Size = new System.Drawing.Size(338, 160);
            this.jsonInput.TabIndex = 0;
            this.jsonInput.Text = resources.GetString("jsonInput.Text");
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(356, 189);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 45);
            this.button1.TabIndex = 1;
            this.button1.Text = "generate JSON";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // jsonOutput
            // 
            this.jsonOutput.Location = new System.Drawing.Point(437, 31);
            this.jsonOutput.Name = "jsonOutput";
            this.jsonOutput.Size = new System.Drawing.Size(436, 415);
            this.jsonOutput.TabIndex = 2;
            this.jsonOutput.Text = "";
            // 
            // json
            // 
            this.json.AutoSize = true;
            this.json.Location = new System.Drawing.Point(12, 15);
            this.json.Name = "json";
            this.json.Size = new System.Drawing.Size(26, 13);
            this.json.TabIndex = 5;
            this.json.Text = "json";
            // 
            // resultJson
            // 
            this.resultJson.AutoSize = true;
            this.resultJson.Location = new System.Drawing.Point(435, 15);
            this.resultJson.Name = "resultJson";
            this.resultJson.Size = new System.Drawing.Size(61, 13);
            this.resultJson.TabIndex = 6;
            this.resultJson.Text = "json Output";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 205);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "html";
            // 
            // htmlInput
            // 
            this.htmlInput.Location = new System.Drawing.Point(12, 221);
            this.htmlInput.Name = "htmlInput";
            this.htmlInput.Size = new System.Drawing.Size(338, 225);
            this.htmlInput.TabIndex = 7;
            this.htmlInput.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(885, 455);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.htmlInput);
            this.Controls.Add(this.resultJson);
            this.Controls.Add(this.json);
            this.Controls.Add(this.jsonOutput);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.jsonInput);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox jsonInput;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RichTextBox jsonOutput;
        private System.Windows.Forms.Label json;
        private System.Windows.Forms.Label resultJson;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox htmlInput;
    }
}

