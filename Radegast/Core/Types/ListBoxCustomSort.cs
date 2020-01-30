/**
 * Radegast Metaverse Client
 * Copyright(c) 2009-2014, Radegast Development Team
 * Copyright(c) 2016-2020, Sjofn, LLC
 * All rights reserved.
 *  
 * Radegast is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.If not, see<https://www.gnu.org/licenses/>.
 */

using System;
using System.Windows.Forms;

namespace Radegast
{
    public class ListBoxCustomSort : ListBox
    {
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
