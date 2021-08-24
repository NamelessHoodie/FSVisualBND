using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FSBndAnimationRegister
{
    /// <summary>
    /// Interaction logic for MyDialog.xaml
    /// </summary>
    public partial class EditDialog : Window
    {
        public EditDialog()
        {
            InitializeComponent();
        }

        public EditDialog(string title, string ID, string Name, string Flags)
        {
            InitializeComponent();
            TitleText = title;
            InputTextID = ID;
            InputTextName = Name;
            InputTextFlags = Flags;
        }

        public string TitleText
        {
            get { return TitleTextBox.Text; }
            set { TitleTextBox.Text = value; }
        }

        public string InputTextID
        {
            get { return InputTextBoxID.Text; }
            set { InputTextBoxID.Text = value; }
        }
        public string InputTextName
        {
            get { return InputTextBoxName.Text; }
            set { InputTextBoxName.Text = value; }
        }
        public string InputTextFlags
        {
            get { return InputTextBoxFlags.Text; }
            set { InputTextBoxFlags.Text = value; }
        }

        public bool Canceled { get; set; }

        private void BtnCancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Canceled = true;
            Close();
        }

        private void BtnSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Canceled = false;
            Close();
        }
    }
}
