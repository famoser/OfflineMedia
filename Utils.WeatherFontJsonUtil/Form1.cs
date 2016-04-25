using System;
using System.Collections.Generic;

namespace Utils.WeatherFontJsonUtil
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var css = richTextBox1.Text;
            var dic = new Dictionary<string, string>();
            while (css.Contains(".owf-"))
            {
                css = css.Substring(css.IndexOf(".owf-", StringComparison.Ordinal));
                var key = css.Substring(5, css.IndexOf(":", StringComparison.Ordinal) -5);
                css = css.Substring(css.IndexOf("content: \"", StringComparison.Ordinal));
                var val = css.Substring("content: \"".Length + 1, css.IndexOf(";", StringComparison.Ordinal) - "content: \"".Length - 2);
                css = css.Substring(css.IndexOf("}", StringComparison.Ordinal));

                dic.Add(key, val);
            }

            richTextBox2.Text = JsonConvert.SerializeObject(dic);
        }
    }
}
