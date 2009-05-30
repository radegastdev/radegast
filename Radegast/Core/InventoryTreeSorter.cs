using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using libsecondlife;
using libsecondlife.AssetSystem;
using libsecondlife.InventorySystem;

namespace SLeek
{
    public class InventoryTreeSorter : IComparer
    {
        private Dictionary<string, ITreeSortMethod> sortMethods = new Dictionary<string, ITreeSortMethod>();
        private string currentMethod;

        public InventoryTreeSorter()
        {
            RegisterSortMethods(Assembly.GetExecutingAssembly());
            currentMethod = ((ITreeSortMethod)sortMethods.Values[0]).Name;
        }

        private void RegisterSortMethods(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.GetInterface("ITreeSortMethod") != null)
                {
                    ConstructorInfo info = type.GetConstructor(Type.EmptyTypes);
                    ITreeSortMethod sort = (ITreeSortMethod)info.Invoke(null);

                    sortMethods.Add(sort.Name, sort);
                }
            }
        }

        public List<ITreeSortMethod> GetSortMethods()
        {
            if (sortMethods.Values.Count == 0) return null;

            List<ITreeSortMethod> methods = new List<ITreeSortMethod>();

            foreach (ITreeSortMethod method in sortMethods.Values)
                methods.Add(methods);

            return methods;
        }

        public string CurrentSortName
        {
            get { return currentMethod; }
            set
            {
                if (!sortMethods.ContainsKey(value))
                    throw new Exception("The specified sort method does not exist.");

                currentMethod = value;
            }
        }

        #region IComparer Members

        public int Compare(object x, object y)
        {
            TreeNode nodeX = (TreeNode)x;
            TreeNode nodeY = (TreeNode)y;

            InventoryBase ibX = (InventoryBase)nodeX.Tag;
            InventoryBase ibY = (InventoryBase)nodeY.Tag;

            return sortMethods[currentMethod].CompareNodes(ibX, ibY, nodeX, nodeY);
        }

        #endregion
    }
}
