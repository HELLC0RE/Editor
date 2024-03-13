using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Editor
{
    public partial class TextEditForm : Form
    {
        public TextEditForm()
        {
            InitializeComponent();
        }
      
        private Dictionary<string, string[]> selectedLanguage;
        private string[] javaKeywords = { "public", "private", "protected", "class", "void", "int", "if", "else", "for", "while", "return", "try", "catch", "finally", "throw", "throws", "true", "false", "null", "this", "super", "new", "instanceof", "static", "final", "abstract", "synchronized", "volatile", "transient", "implements", "extends", "interface", "enum", "strictfp", "assert", "byte", "short", "long", "double", "float", "char", "boolean", "default", "switch" };
        private string[] csharpKeywords = { "public", "private", "protected", "class", "void", "int", "if", "else", "for", "while", "return", "try", "catch", "finally", "throw", "true", "false", "null", "this", "base", "namespace", "using", "bool", "byte", "sbyte", "short", "ushort", "int", "uint", "long", "ulong", "float", "double", "decimal", "string", "object", "dynamic", "var", "const", "readonly", "delegate", "event", "enum", "struct" };
        
        private void CheckByKeyword(string keyword, Color color)
        {
            Regex regex = new Regex(Regex.Escape(keyword), RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(textArea.Text);

            int currentStart = textArea.SelectionStart;
            int currentLength = textArea.SelectionLength;
            Color currentColor = textArea.SelectionColor;

            foreach (Match match in matches)
            {
                textArea.SelectionStart = match.Index;
                textArea.SelectionLength = match.Length;
                textArea.SelectionColor = color;
            }

            textArea.SelectionStart = currentStart;
            textArea.SelectionLength = currentLength;
            textArea.SelectionColor = currentColor;
        }

        private void ShowSyntax()
        {
            string currentLang = languageComboBox.SelectedItem.ToString();
            if (currentLang != "Без подсветки")
            {
                string[] keywords = selectedLanguage[currentLang];

                foreach (string keyword in keywords)
                {
                    CheckByKeyword(keyword, Color.Blue);
                }
            }
        }

        private void openStripButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = "Текстовые файлы (*.txt)|*.txt"
            };

            if (dialog.ShowDialog() == DialogResult.OK && File.Exists(dialog.FileName))
            {
                textArea.LoadFile(dialog.FileName, RichTextBoxStreamType.PlainText);
            }
        }

        private void saveStripButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                InitialDirectory = Environment.CurrentDirectory,
                Filter = "Текстовые файлы(.txt) |.txt",
                RestoreDirectory = false
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                textArea.SaveFile(sfd.FileName, RichTextBoxStreamType.PlainText);
            }
        }

        private void copyStripButton_Click(object sender, EventArgs e)
        {
            if (textArea.SelectedText.Length > 0)
            {
                textArea.Copy();
            }
            else
            {
                MessageBox.Show("Выделите текст для копирования");
            }
        }

        private void insertStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                textArea.Paste();
            }
            catch
            {
                MessageBox.Show("Вставка данных такого формата недоступна");
            }
        }

        private void searchStripButton_Click(object sender, EventArgs e)
        {
            textArea.SelectAll();
            textArea.SelectionBackColor = textArea.BackColor;
            Regex regex = new Regex(Regex.Escape(searchText.Text), RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(textArea.Text);

            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    textArea.SelectionStart = match.Index;
                    textArea.SelectionLength = match.Length;
                    textArea.SelectionBackColor = Color.Yellow;
                }
            }
            else
            {
                MessageBox.Show("Текст не найден");
            }
        }

        private void replaceStripButton_Click(object sender, EventArgs e)
        {
            if (textReplace1.Text == "" || textReplace2.Text == "")
            {
                MessageBox.Show("Укажите все обязательные поля для замены");
                return;
            }

            textArea.Text = textArea.Text.Replace(textReplace1.Text, textReplace2.Text);
        }

        private void formatStripButton_Click(object sender, EventArgs e)
        {
            FontDialog fd = new FontDialog();
            if (fd.ShowDialog() == DialogResult.OK)
                {
                   if (textArea.SelectedText.Length > 0)
                   {
                      textArea.SelectionFont = fd.Font;
                   }
                   else
                   {
                      textArea.Font = fd.Font;
                   }
                }                        
        }

        private void boldStripButton8_Click(object sender, EventArgs e)
        {
            if (textArea.SelectionFont != null)
            {
                FontStyle currentStyle = textArea.SelectionFont.Style;
                FontStyle newStyle = currentStyle ^ FontStyle.Bold;
                textArea.SelectionFont = new Font(textArea.SelectionFont, newStyle);
            }
        }

        private void italicStripButton_Click(object sender, EventArgs e)
        {
            if (textArea.SelectionFont != null)
            {
                FontStyle currentStyle = textArea.SelectionFont.Style;
                FontStyle newStyle = currentStyle ^ FontStyle.Italic;
                textArea.SelectionFont = new Font(textArea.SelectionFont, newStyle);
            }
        }

        private void crossStripButton_Click(object sender, EventArgs e)
        {
            if (textArea.SelectionFont != null)
            {
                FontStyle currentStyle = textArea.SelectionFont.Style;
                FontStyle newStyle = currentStyle ^ FontStyle.Strikeout;
                textArea.SelectionFont = new Font(textArea.SelectionFont, newStyle);
            }
        }

        private void underlineStripButton_Click(object sender, EventArgs e)
        {
            if (textArea.SelectionFont != null)
            {
                FontStyle currentStyle = textArea.SelectionFont.Style;
                FontStyle newStyle = currentStyle ^ FontStyle.Underline;
                textArea.SelectionFont = new Font(textArea.SelectionFont, newStyle);
            }
        }

        private void TextEditForm_Load(object sender, EventArgs e)
        {
                selectedLanguage = new Dictionary<string, string[]>
            {
                {"Java", javaKeywords },
                {"C#", csharpKeywords },
            };

                languageComboBox.Items.Add("Без подсветки");
                languageComboBox.Items.AddRange(selectedLanguage.Keys.ToArray());
                languageComboBox.SelectedIndex = 0;
        }

        private void languageComboBox_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            textArea.SelectAll();
            textArea.SelectionColor = textArea.ForeColor;
            ShowSyntax();
        }

        private void textArea_TextChanged_1(object sender, EventArgs e)
        {
            ShowSyntax();
        }
    }
}
