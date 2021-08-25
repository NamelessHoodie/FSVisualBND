using FSBndAnimationRegister.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using SoulsFormats;
using System.IO;
using FSBndAnimationRegister.MVVM.View;
using Path = System.IO.Path;
using TreeView = System.Windows.Controls.TreeView;
using Binding = System.Windows.Data.Binding;
using Prism.Commands;

namespace FSBndAnimationRegister
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel DataContextViewModel
        {
            get
            {
                return (MainViewModel)this.DataContext;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            DataContextViewModel.ParentWindow = this;
        }

        private void Button_Click_LoadBinderFile(object sender, RoutedEventArgs e)
        {
            LoadBinderFile();
        }
        private void FileBrowser_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DataContextViewModel.TreeViewItemSelected = ((TreeView)sender).SelectedItem;
            if (DebugMenuItem.Visibility == Visibility.Visible)
            {
                var treeView = (TreeView)sender;
                if (treeView.SelectedItem != null)
                {
                    BinderPathFolder binderPath = (BinderPathFolder)((TreeViewItem)treeView.SelectedItem).DataContext;
                    Debug.WriteLine($"Hierarchy Path = {binderPath.GetBinderPath()}");
                    if (binderPath.isFile)
                    {
                        Debug.WriteLine($"Internal Path = {binderPath.File.Name}");
                    }
                }
            }
        }
        private void MenuItem_Click_DebugPrintRecursion(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("DebugRecursionPrintout");
            foreach (var item in DataContextViewModel.BinderPathFolderRoot.ToSFBinderFileList())
            {
                Debug.WriteLine(item.Name);
                Debug.WriteLine(item.ID);
            }
        }
        private void MenuItem_Click_Remove(object sender, RoutedEventArgs e) => DataContextViewModel.BeginRemoveSelectedItem();
        private void MenuItem_Click_Add_File(object sender, RoutedEventArgs e) => DataContextViewModel.BeginAddFile();
        private void MenuItem_Click_Add_Folder(object sender, RoutedEventArgs e) => DataContextViewModel.BeginAddDirectory();
        private void Menu_Item_Save_Simple(object sender, RoutedEventArgs e) => DataContextViewModel.SaveBinderToPath();
        private void Menu_Item_Duplicate(object sender, RoutedEventArgs e) => DataContextViewModel.BeginDuplicateSelectedItem();
        private void Menu_Item_Edit(object sender, RoutedEventArgs e) => DataContextViewModel.BeginSelectedFileEdit();
        private void Menu_Item_DebugReset(object sender, RoutedEventArgs e) => ReloadFileBrowser();
        private void Menu_Item_ReplaceFile(object sender, RoutedEventArgs e) => DataContextViewModel.BeginReplaceSelectedFile();

        public void LoadBinderFile()
        {
            var openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadBinderFile(openFileDialog1.FileName);
            }
        }
        public void LoadBinderFile(string BndPath)
        {
            if (BND4.IsRead(BndPath, out BND4 binder4Selected))
            {
                EditMenuItem.IsEnabled = true;
                DataContextViewModel.LoadedBndPath = BndPath;
                DataContextViewModel.Binder4 = binder4Selected;
                var bndRec = new BinderPathFolder(binder4Selected.Files.First().Name.Substring(0, binder4Selected.Files.First().Name.IndexOf('\\')));
                foreach (var item in binder4Selected.Files)
                {
                    bndRec.AddBinderFileAndCreateFileStructure(item);
                }
                DataContextViewModel.BinderPathFolderRoot = bndRec;

                TreeViewItem rootTreeViewItem = new TreeViewItem() { Header = bndRec.Name, DataContext = bndRec, Foreground = DataContextViewModel.ForegroundCodeBehind };
                for (int i = 0; i < FileBrowser.Items.Count; i++)
                {
                    var currentItem = (TreeViewItem)FileBrowser.Items[i];
                    FileBrowser.Items.Remove(currentItem);
                }

                FileBrowser.Items.Add(rootTreeViewItem);

                LoadBinderPathFolderToFileBrowser(bndRec, rootTreeViewItem);
            }
            else if (BND3.IsRead(BndPath, out BND3 binder3Selected))
            {
                EditMenuItem.IsEnabled = true;
                DataContextViewModel.LoadedBndPath = BndPath;
                DataContextViewModel.Binder3 = binder3Selected;
                var bndRec = new BinderPathFolder(binder3Selected.Files.First().Name.Substring(0, binder3Selected.Files.First().Name.IndexOf('\\')));
                foreach (var item in binder3Selected.Files)
                {
                    bndRec.AddBinderFileAndCreateFileStructure(item);
                }
                DataContextViewModel.BinderPathFolderRoot = bndRec;

                TreeViewItem rootTreeViewItem = new TreeViewItem() { Header = bndRec.Name, DataContext = bndRec, Foreground = DataContextViewModel.ForegroundCodeBehind };
                for (int i = 0; i < FileBrowser.Items.Count; i++)
                {
                    var currentItem = (TreeViewItem)FileBrowser.Items[i];
                    FileBrowser.Items.Remove(currentItem);
                }

                FileBrowser.Items.Add(rootTreeViewItem);

                LoadBinderPathFolderToFileBrowser(bndRec, rootTreeViewItem);
            }
        }
        public void ReloadFileBrowser()
        {
            for (int i = 0; i < FileBrowser.Items.Count; i++)
            {
                var currentItem = (TreeViewItem)FileBrowser.Items[i];
                FileBrowser.Items.Remove(currentItem);
            }
            TreeViewItem rootTreeViewItem = new TreeViewItem() { Foreground = DataContextViewModel.ForegroundCodeBehind, Header = DataContextViewModel.BinderPathFolderRoot.Name, DataContext = DataContextViewModel.BinderPathFolderRoot };
            FileBrowser.Items.Add(rootTreeViewItem);
            LoadBinderPathFolderToFileBrowser(DataContextViewModel.BinderPathFolderRoot, rootTreeViewItem);
        }
        public void LoadBinderPathFolderToFileBrowser(BinderPathFolder currentBinderPath, TreeViewItem currentTreeViewItem)
        {
            foreach (var (name, childFolder) in currentBinderPath.childFolders)
            {
                StackPanel stack = new StackPanel() { Orientation = System.Windows.Controls.Orientation.Horizontal };
                var textBlockName = new TextBlock() { Foreground = DataContextViewModel.ForegroundCodeBehind };
                var textBlockID = new TextBlock() { Margin = new Thickness(5, 0, 0, 0), Foreground = DataContextViewModel.ForegroundCodeBehind };
                stack.Children.Add(textBlockName);
                stack.Children.Add(textBlockID);
                Binding nameBinding = new Binding("Name");
                Binding idBinding = new Binding("ID");
                BindingOperations.SetBinding(textBlockName, TextBlock.TextProperty, nameBinding);
                BindingOperations.SetBinding(textBlockID, TextBlock.TextProperty, idBinding);
                TreeViewItem newTreeViewItem = new TreeViewItem() { Header = stack, DataContext = childFolder };
                currentTreeViewItem.Items.Add(newTreeViewItem);
                LoadBinderPathFolderToFileBrowser(childFolder, newTreeViewItem);
            }
        }
    }
}
