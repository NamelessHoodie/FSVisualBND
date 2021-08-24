using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using SoulsFormats;
using FSBndAnimationRegister.MVVM.View;

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
        public MainViewModel()
        { 
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
