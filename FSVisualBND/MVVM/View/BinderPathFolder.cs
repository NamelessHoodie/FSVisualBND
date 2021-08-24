using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSBndAnimationRegister.MVVM.View
{
    public class BinderPathFolder
    {
        public BinderPathFolder(string Name)
        {
            this.Name = Name;
        }
        public BinderPathFolder(string Name, BinderPathFolder Parent)
        {
            this.Name = Name;
            this.Parent = Parent;
        }
        public BinderPathFolder Parent;
        public string Name { get; set; }
        public int ID 
        { 
            get 
            {
                if (isFile)
                {
                    return File.ID;
                }
                else
                {
                    return -1;
                }
            } 
            set 
            {
                if (isFile)
                {
                    File.ID = value;
                }
            } 
        }
        public BinderFile File { get; set; } = null;
        public string FullPath
        {
            get
            {
                return GetBinderPath();
            }
        }
        public bool isFile
        {
            get
            {
                if (File != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public List<KeyValuePair<string, BinderPathFolder>> childFolders { get; set; } = new List<KeyValuePair<string, BinderPathFolder>>();
        public string GetBinderPath()
        {
            var tempList = new List<string>();
            BinderPathRecursive(this,ref tempList);
            tempList.Reverse();
            return string.Join('\\', tempList);
        }
        private List<string> BinderPathRecursive(BinderPathFolder root, ref List<string> path)
        {
            path.Add($"{root.Name}");
            if (root.Parent != null)
            {
                return BinderPathRecursive(root.Parent, ref path);
            }
            else
            {
                return path;
            }
        }
    }
}
