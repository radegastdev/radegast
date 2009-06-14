// 
// Radegast Metaverse Client
// Copyright (c) 2009, Radegast Development Team
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//       this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of the application "Radegast", nor the names of its
//       contributors may be used to endorse or promote products derived from
//       this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// $Id$
//
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
