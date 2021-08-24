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

namespace FSBndAnimationRegister
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Brush ForegroundCodeBehind
        {
            get
            {
                return App.Current.MainWindow.Foreground;
            }
        }
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
        }
        public string LoadedBndPath { get; set; }

        private void Button_Click_LoadBinderFile(object sender, RoutedEventArgs e)
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
                LoadedBndPath = BndPath;
                DataContextViewModel.Binder4 = binder4Selected;
                var bndRec = new BinderPathFolder(binder4Selected.Files.First().Name.Substring(0, binder4Selected.Files.First().Name.IndexOf('\\')));
                foreach (var item in binder4Selected.Files)
                {
                    bndRec.AddBinderFileAndCreateFileStructure(item);
                }
                DataContextViewModel.BinderPathFolderRoot = bndRec;

                TreeViewItem rootTreeViewItem = new TreeViewItem() { Header = bndRec.Name, DataContext = bndRec, Foreground = ForegroundCodeBehind };
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
                LoadedBndPath = BndPath;
                DataContextViewModel.Binder3 = binder3Selected;
                var bndRec = new BinderPathFolder(binder3Selected.Files.First().Name.Substring(0, binder3Selected.Files.First().Name.IndexOf('\\')));
                foreach (var item in binder3Selected.Files)
                {
                    bndRec.AddBinderFileAndCreateFileStructure(item);
                }
                DataContextViewModel.BinderPathFolderRoot = bndRec;

                TreeViewItem rootTreeViewItem = new TreeViewItem() { Header = bndRec.Name, DataContext = bndRec, Foreground = ForegroundCodeBehind };
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
            TreeViewItem rootTreeViewItem = new TreeViewItem() { Foreground = ForegroundCodeBehind, Header = DataContextViewModel.BinderPathFolderRoot.Name, DataContext = DataContextViewModel.BinderPathFolderRoot };
            FileBrowser.Items.Add(rootTreeViewItem);
            LoadBinderPathFolderToFileBrowser(DataContextViewModel.BinderPathFolderRoot, rootTreeViewItem);
        }

        public void LoadBinderPathFolderToFileBrowser(BinderPathFolder currentBinderPath, TreeViewItem currentTreeViewItem)
        {
            foreach (var (name, childFolder) in currentBinderPath.childFolders)
            {
                StackPanel stack = new StackPanel() { Orientation = System.Windows.Controls.Orientation.Horizontal };
                var textBlockName = new TextBlock() { Foreground = ForegroundCodeBehind };
                var textBlockID = new TextBlock() { Margin = new Thickness(5, 0, 0, 0), Foreground = ForegroundCodeBehind };
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
        public void CreateTreeViewChildFromBinderPathFolder(BinderPathFolder currentBinderPath, TreeViewItem treeViewItemParent)
        {
            StackPanel stack = new StackPanel() { Orientation = System.Windows.Controls.Orientation.Horizontal };
            var textBlockName = new TextBlock() { Foreground = ForegroundCodeBehind };
            var textBlockID = new TextBlock() { Margin = new Thickness(5, 0, 0, 0), Foreground = ForegroundCodeBehind };
            stack.Children.Add(textBlockName);
            stack.Children.Add(textBlockID);
            Binding nameBinding = new Binding("Name");
            Binding idBinding = new Binding("ID");
            BindingOperations.SetBinding(textBlockName, TextBlock.TextProperty, nameBinding);
            BindingOperations.SetBinding(textBlockID, TextBlock.TextProperty, idBinding);
            TreeViewItem newTreeViewItem = new TreeViewItem() { Header = stack, DataContext = currentBinderPath };
            treeViewItemParent.Items.Add(newTreeViewItem);
        }

        public void CreateTreeViewChildFromBinderPathFolder(BinderPathFolder currentBinderPath, TreeViewItem treeViewItemParent, int insertIndex)
        {
            StackPanel stack = new StackPanel() { Orientation = System.Windows.Controls.Orientation.Horizontal };
            var textBlockName = new TextBlock() { Foreground = ForegroundCodeBehind };
            var textBlockID = new TextBlock() { Margin = new Thickness(5, 0, 0, 0), Foreground = ForegroundCodeBehind };
            stack.Children.Add(textBlockName);
            stack.Children.Add(textBlockID);
            Binding nameBinding = new Binding("Name");
            Binding idBinding = new Binding("ID");
            BindingOperations.SetBinding(textBlockName, TextBlock.TextProperty, nameBinding);
            BindingOperations.SetBinding(textBlockID, TextBlock.TextProperty, idBinding);
            TreeViewItem newTreeViewItem = new TreeViewItem() { Header = stack, DataContext = currentBinderPath };
            treeViewItemParent.Items.Insert(insertIndex, newTreeViewItem);
        }

        private void FileBrowser_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
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

        private void MenuItem_Click_DebugPrintRecursion(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("DebugRecursionPrintout");
            foreach (var item in DataContextViewModel.BinderPathFolderRoot.ToSFBinderFileList())
            {
                Debug.WriteLine(item.Name);
                Debug.WriteLine(item.ID);
            }
        }

        private void MenuItem_Click_Remove(object sender, RoutedEventArgs e)
        {

            if (FileBrowser.SelectedItem != null)
            {
                TreeViewItem treeViewItem = (TreeViewItem)FileBrowser.SelectedItem;

                var binderPathFolder = (BinderPathFolder)treeViewItem.DataContext;
                if (binderPathFolder.Parent != null)
                {
                    foreach (var item in binderPathFolder.Parent.childFolders.Where(a => a.Key == binderPathFolder.Name))
                    {
                        binderPathFolder.Parent.childFolders.Remove(item);
                    }
                }
                if (treeViewItem.Parent != null && treeViewItem.Parent is TreeViewItem)
                {
                    TreeViewItem treeViewItemParent = (TreeViewItem)treeViewItem.Parent;
                    treeViewItemParent.Items.Remove(treeViewItem);
                }
            }
        }

        private void MenuItem_Click_Add_File(object sender, RoutedEventArgs e)
        {
            if (FileBrowser.SelectedItem is TreeViewItem)
            {
                var treeViewItemSelected = (TreeViewItem)FileBrowser.SelectedItem;
                var binderPathFolderSelected = (BinderPathFolder)treeViewItemSelected.DataContext;
                if (!binderPathFolderSelected.isFile)
                {
                    List<BinderFile> binderFileList = DataContextViewModel.BinderPathFolderRoot.ToSFBinderFileList();
                    if (binderFileList.Any())
                    {
                        var openFileDialog1 = new OpenFileDialog() { Title = "Select the file to add to the selected Folder" };
                        if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            byte[] selectedFileBytes = File.ReadAllBytes(openFileDialog1.FileName);
                            string selectedFileName = Path.GetFileName(openFileDialog1.FileName);
                            string addedFileInternalPath = binderPathFolderSelected.FullPath + "\\" + selectedFileName;

                            var dialog = new IDRequestDialog("Choose an ID for the file you're adding.", binderFileList.First().ID.ToString());
                            dialog.Show();
                            dialog.Closing += (sender, e) =>
                            {
                                var d = sender as IDRequestDialog;
                                if (!d.Canceled)
                                {
                                    if (Int32.TryParse(d.InputText, out int newID))
                                    {
                                        var newBinderFile = new BinderFile(binderFileList.First()) { Bytes = selectedFileBytes, ID = newID, Name = addedFileInternalPath };
                                        var newBinderPathFolder = new BinderPathFolder(selectedFileName, binderPathFolderSelected) { File = newBinderFile };
                                        binderPathFolderSelected.childFolders.Add(new KeyValuePair<string, BinderPathFolder>(selectedFileName, newBinderPathFolder));
                                        //ReloadFileBrowser();
                                        CreateTreeViewChildFromBinderPathFolder(newBinderPathFolder, treeViewItemSelected);
                                        System.Windows.MessageBox.Show("File Added Successfully to the binder");
                                    }
                                }
                            };
                        }
                    }
                }
            }
        }

        private void MenuItem_Click_Add_Folder(object sender, RoutedEventArgs e)
        {

        }

        private void SaveBinderToPath(string savePath)
        {
            var binder4 = DataContextViewModel.Binder4;
            var binder3 = DataContextViewModel.Binder3;
            if (binder4 != null)
            {
                binder4.Files = DataContextViewModel.BinderPathFolderRoot.ToSFBinderFileList();
                DataContextViewModel.Binder4.Write(savePath);
            }
            else
            {
                binder3.Files = DataContextViewModel.BinderPathFolderRoot.ToSFBinderFileList();
                DataContextViewModel.Binder3.Write(savePath);
            }
        }

        private void Menu_Item_Save_Simple(object sender, RoutedEventArgs e)
        {
            SaveBinderToPath(LoadedBndPath);
        }

        private void Menu_Item_Duplicate(object sender, RoutedEventArgs e)
        {
            if (FileBrowser.SelectedItem is TreeViewItem)
            {
                var treeViewItemSelected = (TreeViewItem)FileBrowser.SelectedItem;
                var binderPathFolderSelected = (BinderPathFolder)treeViewItemSelected.DataContext;
                if (treeViewItemSelected.Parent is TreeViewItem && binderPathFolderSelected.isFile)
                {
                    var treeViewItemSelectedParent = (TreeViewItem)treeViewItemSelected.Parent;
                    var binderPathFolderSelectedParent = (BinderPathFolder)treeViewItemSelectedParent.DataContext;
                    var indexOfSelectedTreeViewItem = treeViewItemSelectedParent.Items.IndexOf(treeViewItemSelected);
                    List<BinderFile> binderFileList = DataContextViewModel.BinderPathFolderRoot.ToSFBinderFileList();
                    if (binderFileList.Any())
                    {
                        var newBinderFile = new BinderFile(binderPathFolderSelected.File) { ID = binderPathFolderSelected.File.ID + 1, Name = binderPathFolderSelected.File.Name + "Copy" };
                        var newBinderPathFolder = new BinderPathFolder(binderPathFolderSelected.Name + "Copy", binderPathFolderSelectedParent) { File = newBinderFile };
                        binderPathFolderSelectedParent.childFolders.Add(new KeyValuePair<string, BinderPathFolder>(binderPathFolderSelected.Name, newBinderPathFolder));

                        CreateTreeViewChildFromBinderPathFolder(newBinderPathFolder, treeViewItemSelectedParent, indexOfSelectedTreeViewItem + 1);
                        System.Windows.MessageBox.Show($"Successfully Duplicated File {binderPathFolderSelected.Name}");
                    }
                }
            }
        }

        private void Menu_Item_Edit(object sender, RoutedEventArgs e)
        {
            var selectedTreeViewItem = FileBrowser.SelectedItem as TreeViewItem;
            var selectedBinderPathFolder = selectedTreeViewItem.DataContext as BinderPathFolder;
            if (selectedTreeViewItem.Parent is TreeViewItem && selectedBinderPathFolder.isFile)
            {
                var treeViewItemSelectedParent = (TreeViewItem)selectedTreeViewItem.Parent;
                var binderPathFolderSelectedParent = (BinderPathFolder)treeViewItemSelectedParent.DataContext;
                var indexOfSelectedTreeViewItem = treeViewItemSelectedParent.Items.IndexOf(selectedTreeViewItem);
                string ID = selectedBinderPathFolder.ID.ToString();
                string Name = selectedBinderPathFolder.Name;
                string Flags = selectedBinderPathFolder.File.Flags.ToString();
                var dialog = new EditDialog("Edit File Popup", ID, Name, Flags);
                dialog.ShowDialog();
                if (!dialog.Canceled)
                {
                    treeViewItemSelectedParent.Items.Remove(selectedTreeViewItem);
                    if (dialog.InputTextID != ID)
                    {
                        selectedBinderPathFolder.ID = Int32.Parse(dialog.InputTextID);
                    }
                    if (dialog.InputTextName != Name)
                    {
                        selectedBinderPathFolder.Name = dialog.InputTextName;
                        selectedBinderPathFolder.File.Name = Path.GetDirectoryName(selectedBinderPathFolder.File.Name) + dialog.InputTextName;
                    }
                    CreateTreeViewChildFromBinderPathFolder(selectedBinderPathFolder, treeViewItemSelectedParent, indexOfSelectedTreeViewItem);
                }
            }
        }

        private void Menu_Item_DebugReset(object sender, RoutedEventArgs e)
        {
            ReloadFileBrowser();
        }
    }
}
