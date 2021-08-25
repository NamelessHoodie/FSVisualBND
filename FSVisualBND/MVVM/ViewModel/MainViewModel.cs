using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using SoulsFormats;
using FSBndAnimationRegister.MVVM.View;
using System.Windows.Input;
using Prism.Commands;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Linq;
using Binding = System.Windows.Data.Binding;
using System.IO;
using System.Windows.Media;

namespace FSBndAnimationRegister.MVVM.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public BND4 Binder4 { get; set; } = null;
        public BND3 Binder3 { get; set; } = null;
        private BinderPathFolder _BinderPathFolderRoot;
        public BinderPathFolder BinderPathFolderRoot
        {
            get
            {
                return _BinderPathFolderRoot;
            }
            set
            {
                _BinderPathFolderRoot = value;
                NotifyPropertyChanged();
            }
        }
        public Brush ForegroundCodeBehind
        {
            get
            {
                return App.Current.MainWindow.Foreground;
            }
        }
        public MainWindow ParentWindow { get; set; }
        private ICommand _CommandHotkeyBeginEditFile;
        public ICommand CommandHotkeyBeginEditFile
        {
            get
            {
                if (_CommandHotkeyBeginEditFile == null)
                {
                    _CommandHotkeyBeginEditFile = new DelegateCommand(delegate ()
                    {
                        BeginSelectedFileEdit();
                    });
                }
                return _CommandHotkeyBeginEditFile;
            }
        }
        private ICommand _CommandHotkeyBeginDuplicateFile;
        public ICommand CommandHotkeyBeginDuplicateFile
        {
            get
            {
                if (_CommandHotkeyBeginDuplicateFile == null)
                {
                    _CommandHotkeyBeginDuplicateFile = new DelegateCommand(delegate ()
                    {
                        BeginDuplicateSelectedItem();
                    });
                }
                return _CommandHotkeyBeginDuplicateFile;
            }
        }
        private ICommand _CommandHotkeyBeginReplaceFile;
        public ICommand CommandHotkeyBeginReplaceFile
        {
            get
            {
                if (_CommandHotkeyBeginReplaceFile == null)
                {
                    _CommandHotkeyBeginReplaceFile = new DelegateCommand(delegate ()
                    {
                        BeginReplaceSelectedFile();
                    });
                }
                return _CommandHotkeyBeginReplaceFile;
            }
        }
        private ICommand _CommandHotkeyBeginDelete;
        public ICommand CommandHotkeyBeginDelete
        {
            get
            {
                if (_CommandHotkeyBeginDelete == null)
                {
                    _CommandHotkeyBeginDelete = new DelegateCommand(delegate ()
                    {
                        BeginRemoveSelectedItem();
                    });
                }
                return _CommandHotkeyBeginDelete;
            }
        }
        private ICommand _CommandHotkeyBeginAddFile;
        public ICommand CommandHotkeyBeginAddFile
        {
            get
            {
                if (_CommandHotkeyBeginAddFile == null)
                {
                    _CommandHotkeyBeginAddFile = new DelegateCommand(delegate ()
                    {
                        BeginAddFile();
                    });
                }
                return _CommandHotkeyBeginAddFile;
            }
        }
        private ICommand _CommandHotkeyBeginAddDirectory;
        public ICommand CommandHotkeyBeginAddDirectory
        {
            get
            {
                if (_CommandHotkeyBeginAddDirectory == null)
                {
                    _CommandHotkeyBeginAddDirectory = new DelegateCommand(delegate ()
                    {
                        BeginAddDirectory();
                    });
                }
                return _CommandHotkeyBeginAddDirectory;
            }
        }
        private string _LoadedBndPath = null;
        public string LoadedBndPath
        {
            get
            {
                return _LoadedBndPath;
            }
            set
            {
                _LoadedBndPath = value;
            }
        }
        private object _TreeViewItemSelected = null;
        public object TreeViewItemSelected
        {
            get
            {
                return _TreeViewItemSelected;
            }
            set
            {
                _TreeViewItemSelected = value;
                NotifyPropertyChanged();
            }
        }
        public bool FileBrowserHasSelectedItem
        {
            get
            {
                return TreeViewItemSelected != null;
            }
        }

        public MainViewModel()
        {
        }

        public void SaveBinderToPath()
        {
            SaveBinderToPath(LoadedBndPath);
        }
        private void SaveBinderToPath(string savePath)
        {
            if (Binder4 != null)
            {
                Binder4.Files = BinderPathFolderRoot.ToSFBinderFileList();
                Binder4.Write(savePath);
            }
            else
            {
                Binder3.Files = BinderPathFolderRoot.ToSFBinderFileList();
                Binder3.Write(savePath);
            }
        }
        public void BeginReplaceSelectedFile()
        {
            if (FileBrowserHasSelectedItem)
            {
                TreeViewItem selectedTreeViewItem = TreeViewItemSelected as TreeViewItem;
                BinderPathFolder selectedBinderPathFolder = selectedTreeViewItem.DataContext as BinderPathFolder;
                if (selectedTreeViewItem.Parent is TreeViewItem && selectedBinderPathFolder.isFile)
                {
                    var treeViewItemSelectedParent = (TreeViewItem)selectedTreeViewItem.Parent;
                    var binderPathFolderSelectedParent = (BinderPathFolder)treeViewItemSelectedParent.DataContext;
                    var replaceDialog = new OpenFileDialog() { Title = "Select a file to replace the currently selected file." };
                    if (replaceDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        byte[] fileBytes = File.ReadAllBytes(replaceDialog.FileName);
                        var newFileName = Path.GetFileName(replaceDialog.FileName);

                        selectedBinderPathFolder.File.Name = Path.Join(Path.GetDirectoryName(selectedBinderPathFolder.File.Name), newFileName);
                        selectedBinderPathFolder.File.Bytes = fileBytes;
                        selectedBinderPathFolder.Name = newFileName;
                        
                        var indexOfSelectedTreeViewItem = treeViewItemSelectedParent.Items.IndexOf(selectedTreeViewItem);
                        treeViewItemSelectedParent.Items.Remove(selectedTreeViewItem);
                        CreateTreeViewChildFromBinderPathFolder(selectedBinderPathFolder, treeViewItemSelectedParent, indexOfSelectedTreeViewItem);
                    }
                }
            }
        }
        public void BeginDuplicateSelectedItem()
        {
            if (FileBrowserHasSelectedItem)
            {
                if (TreeViewItemSelected is TreeViewItem)
                {
                    var treeViewItemSelected = (TreeViewItem)TreeViewItemSelected;
                    var binderPathFolderSelected = (BinderPathFolder)treeViewItemSelected.DataContext;
                    if (treeViewItemSelected.Parent != null && treeViewItemSelected.Parent is TreeViewItem && binderPathFolderSelected.isFile)
                    {
                        var treeViewItemSelectedParent = (TreeViewItem)treeViewItemSelected.Parent;
                        var binderPathFolderSelectedParent = (BinderPathFolder)treeViewItemSelectedParent.DataContext;
                        var indexOfSelectedTreeViewItem = treeViewItemSelectedParent.Items.IndexOf(treeViewItemSelected);
                        var newBinderFile = new BinderFile(binderPathFolderSelected.File) { ID = binderPathFolderSelected.File.ID + 1, Name = binderPathFolderSelected.File.Name + "Copy" };
                        var newBinderPathFolder = new BinderPathFolder(binderPathFolderSelected.Name + "Copy", binderPathFolderSelectedParent) { File = newBinderFile };
                        binderPathFolderSelectedParent.childFolders.Add(new KeyValuePair<string, BinderPathFolder>(binderPathFolderSelected.Name + "Copy", newBinderPathFolder));

                        CreateTreeViewChildFromBinderPathFolder(newBinderPathFolder, treeViewItemSelectedParent, indexOfSelectedTreeViewItem + 1);
                        System.Windows.MessageBox.Show($"Successfully Duplicated File {binderPathFolderSelected.Name}");
                    }
                }

            }
        }
        public void BeginRemoveSelectedItem()
        {
            if (FileBrowserHasSelectedItem)
            {
                TreeViewItem treeViewItem = (TreeViewItem)TreeViewItemSelected;
                var binderPathFolder = (BinderPathFolder)treeViewItem.DataContext;
                var filesListLocal = (binderPathFolder.isFile ? binderPathFolder.Parent : binderPathFolder).ToSFBinderFileListLocal();
                var filesList = BinderPathFolderRoot.ToSFBinderFileList();
                if (filesList.Count() > filesListLocal.Count())
                {
                    if (binderPathFolder.Parent != null)
                    {
                        IEnumerable<KeyValuePair<string, BinderPathFolder>> tempIEnumerable = binderPathFolder.Parent.childFolders.Where(a => a.Key == binderPathFolder.Name);
                        for (int i = 0; i < tempIEnumerable.Count(); i++)
                        {
                            var currentPair = tempIEnumerable.ElementAt(i);
                            binderPathFolder.Parent.childFolders.Remove(currentPair);
                        }
                    }
                    if (treeViewItem.Parent != null && treeViewItem.Parent is TreeViewItem)
                    {
                        TreeViewItem treeViewItemParent = (TreeViewItem)treeViewItem.Parent;
                        treeViewItemParent.Items.Remove(treeViewItem);
                        //ParentWindow.ReloadFileBrowser();
                    }
                    return;
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("At least one file must remain present in the binder at all times.");
                }
            }
        }
        public void BeginSelectedFileEdit()
        {
            if (FileBrowserHasSelectedItem)
            {
                var selectedTreeViewItem = TreeViewItemSelected as TreeViewItem;
                var selectedBinderPathFolder = selectedTreeViewItem.DataContext as BinderPathFolder;
                if (selectedTreeViewItem.Parent is TreeViewItem && selectedBinderPathFolder.isFile)
                {
                    var treeViewItemSelectedParent = (TreeViewItem)selectedTreeViewItem.Parent;
                    var binderPathFolderSelectedParent = (BinderPathFolder)treeViewItemSelectedParent.DataContext;
                    var indexOfSelectedTreeViewItem = treeViewItemSelectedParent.Items.IndexOf(selectedTreeViewItem);
                    string ID = selectedBinderPathFolder.ID.ToString();
                    string Name = selectedBinderPathFolder.Name;
                    string Flags = ((byte)selectedBinderPathFolder.File.Flags).ToString();
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
                            selectedBinderPathFolder.File.Name = Path.Join(Path.GetDirectoryName(selectedBinderPathFolder.File.Name), dialog.InputTextName);
                        }
                        if (dialog.InputTextFlags != Flags)
                        {
                            selectedBinderPathFolder.File.Flags = (Binder.FileFlags)Byte.Parse(dialog.InputTextFlags);
                        }
                        CreateTreeViewChildFromBinderPathFolder(selectedBinderPathFolder, treeViewItemSelectedParent, indexOfSelectedTreeViewItem);
                    }
                }
            }
        }
        public void BeginAddFile()
        {
            if (FileBrowserHasSelectedItem)
            {
                if (TreeViewItemSelected is TreeViewItem)
                {
                    var treeViewItemSelected = (TreeViewItem)TreeViewItemSelected;
                    var binderPathFolderSelected = (BinderPathFolder)treeViewItemSelected.DataContext;
                    if (!binderPathFolderSelected.isFile)
                    {
                        List<BinderFile> binderFileList = BinderPathFolderRoot.ToSFBinderFileList();

                        var openFileDialog1 = new OpenFileDialog() { Title = "Select the File/Files to add to the selected Folder", Multiselect = true };
                        if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            var dialog = new IDRequestDialog(openFileDialog1.FileNames.Count() > 1 ? "Select the starting ID for the files you've selected, each file's id will be in increments of 1." : "Choose an ID for the file you're adding.", binderFileList.Any() ? binderFileList.Last().ID.ToString() : "0");
                            dialog.ShowDialog();
                            if (!dialog.Canceled)
                            {
                                for (int i = 0; i < openFileDialog1.FileNames.Count(); i++)
                                {
                                    var currentFile = openFileDialog1.FileNames[i];
                                    byte[] selectedFileBytes = File.ReadAllBytes(currentFile);
                                    string selectedFileName = Path.GetFileName(currentFile);
                                    string addedFileInternalPath = binderPathFolderSelected.FullPath + "\\" + selectedFileName;

                                    if (Int32.TryParse(dialog.InputText, out int newID))
                                    {
                                        BND4 a = new BND4();
                                        var newBinderFile = new BinderFile(binderFileList.First()) { Bytes = selectedFileBytes, ID = newID + i, Name = addedFileInternalPath };
                                        var newBinderPathFolder = new BinderPathFolder(selectedFileName, binderPathFolderSelected) { File = newBinderFile };
                                        binderPathFolderSelected.childFolders.Add(new KeyValuePair<string, BinderPathFolder>(selectedFileName, newBinderPathFolder));
                                        //ReloadFileBrowser();
                                        CreateTreeViewChildFromBinderPathFolder(newBinderPathFolder, treeViewItemSelected);
                                    }
                                }
                                System.Windows.MessageBox.Show("File/Files Added Successfully to the binder");
                            }
                        }
                    }
                }
            }

        }
        public void BeginAddDirectory()
        {
            if (FileBrowserHasSelectedItem)
            {
                if (TreeViewItemSelected is TreeViewItem)
                {
                    var treeViewItemSelected = (TreeViewItem)TreeViewItemSelected;
                    var binderPathFolderSelected = (BinderPathFolder)treeViewItemSelected.DataContext;
                    if (!binderPathFolderSelected.isFile)
                    {
                        List<BinderFile> binderFileList = BinderPathFolderRoot.ToSFBinderFileList();
                        if (binderFileList.Any())
                        {
                            var dialog = new IDRequestDialog("Choose a name for the folder you're adding", "Folder");
                            dialog.ShowDialog();
                            if (!dialog.Canceled)
                            {
                                var newBinderPathFolder = new BinderPathFolder(dialog.InputText, binderPathFolderSelected);
                                binderPathFolderSelected.childFolders.Add(new KeyValuePair<string, BinderPathFolder>(dialog.InputText, newBinderPathFolder));
                                CreateTreeViewChildFromBinderPathFolder(newBinderPathFolder, treeViewItemSelected);
                                System.Windows.MessageBox.Show("Folder Added Successfully to the binder");
                            }
                        }
                    }
                }
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
        //-------------------------------------------------------------------------------
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// INotifyPropertyChanged Implementation
        /// </summary>
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
