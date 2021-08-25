using FSBndAnimationRegister.MVVM.View;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace FSBndAnimationRegister
{
    public static class ExtensionMethods
    {
        public static bool AddBinderFileAndCreateFileStructure(this BinderPathFolder root, BinderFile bnd)
        {
            var currentBndLayer = root;
            var strArray = bnd.Name.Split('\\');
            for (int i = 1; i < strArray.Length; i++)
            {
                var tempIEnumerable = currentBndLayer.childFolders.Where(a => a.Key == strArray[i]);
                if (i == strArray.Length -1)
                {
                    var newBndPathObject = new BinderPathFolder(strArray[i], currentBndLayer);
                    currentBndLayer.childFolders.Add(new KeyValuePair<string, BinderPathFolder>(strArray[i], newBndPathObject));
                    currentBndLayer = newBndPathObject;
                    newBndPathObject.File = bnd;
                    return true;
                }
                else if (tempIEnumerable.Any())
                {
                    currentBndLayer = tempIEnumerable.First().Value;
                }
                else
                {
                    var newBndPathObject = new BinderPathFolder(strArray[i], currentBndLayer);
                    currentBndLayer.childFolders.Add(new KeyValuePair<string, BinderPathFolder>(strArray[i], newBndPathObject));
                    currentBndLayer = newBndPathObject;
                }
            }
            return false;
        }
        public static List<BinderFile> ToSFBinderFileListLocal(this BinderPathFolder binderPathRoot)
        {
            List<BinderFile> binderFileList = new List<BinderFile>();
            foreach (var (name, childFolder) in binderPathRoot.childFolders)
            {
                if (childFolder.isFile)
                {
                    binderFileList.Add(childFolder.File);
                }
            }
            binderFileList = binderFileList.OrderBy(binderFile => binderFile.ID).ToList();
            return binderFileList;
        }
        public static List<BinderFile> ToSFBinderFileList(this BinderPathFolder binderPathRoot)
        {
            List<BinderFile> binderFileList = new List<BinderFile>();
            ToSFBinderFileListRecursive(binderPathRoot, ref binderFileList);
            binderFileList = binderFileList.OrderBy(binderFile => binderFile.ID).ToList();
            return binderFileList;
        }
        private static void ToSFBinderFileListRecursive(this BinderPathFolder binderPathRoot, ref List<BinderFile> listToFill)
        {
            foreach (var (name, childFolder) in binderPathRoot.childFolders)
            {
                if (childFolder.isFile)
                {
                    listToFill.Add(childFolder.File);
                }
                ToSFBinderFileListRecursive(childFolder, ref listToFill);
            }

        }
    }
}
