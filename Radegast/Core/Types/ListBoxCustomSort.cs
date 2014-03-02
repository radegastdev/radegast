// 
// Radegast Metaverse Client
// Copyright (c) 2009-2014, Radegast Development Team
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
// $Id: ListBoxCustomSort.cs 300 2009-10-05 09:29:46Z latifer@gmail.com $
//
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Radegast
{
    public class ListBoxCustomSort : ListBox
    {
        public ListBoxCustomSort()
            : base()
        {
        }

        public void PerformSort()
        {
            QuickSort(0, Items.Count - 1);
        }

        private void QuickSort(int left, int right)
        {
            if (right > left)
            {
                int pivotIndex = left;
                int pivotNewIndex = QuickSortPartition(left, right, pivotIndex);

                QuickSort(left, pivotNewIndex - 1);
                QuickSort(pivotNewIndex + 1, right);
            }
        }

        private int QuickSortPartition(int left, int right, int pivot)
        {
            var pivotValue = (IComparable)Items[pivot];
            Swap(pivot, right);

            int storeIndex = left;
            for (int i = left; i < right; ++i)
            {
                if (pivotValue.CompareTo(Items[i]) >= 0)
                {
                    Swap(i, storeIndex);
                    ++storeIndex;
                }
            }

            Swap(storeIndex, right);
            return storeIndex;
        }

        private void Swap(int left, int right)
        {
            var temp = Items[left];
            Items[left] = Items[right];
            Items[right] = temp;
        }
    }
}
