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
// $Id$
//
using System.Collections.Generic;

namespace Radegast
{
    /// <summary>
    /// A list which acts like the List class, but with more of a java
    /// influence. This allows you to set
    /// a looping variable to true, and creates
    /// a circular list. Also it utilizes the java iterator pattern of
    /// Next and HasNext.
    /// 
    /// Author: Wesley Tansey
    /// </summary>
    /// <typeparam name="T">The type of item that
    /// will be stored in the list</typeparam>

    public class CircularList<T> : List<T>
    {
        #region Member Variables
        private bool loop = true;
        private int index;
        #endregion

        #region Properties

        /// <summary>
        /// If true, the list will loop to the beginning when Next
        /// is called after the last element has been accessed.
        /// </summary>
        public bool Loop
        {
            get { return loop; }
            set { loop = value; }
        }

        /// <summary>
        /// The next element in the list. The user is responsible for
        /// making sure that HasNext is true
        /// before getting the next element.
        /// </summary>

        public T Next
        {
            get
            {
                if (index >= Count)
                {
                    if (!loop)
                    {
                        return default(T);
                    }
                    index = 0;
                }
                return this[index++];
            }
        }

        /// <summary>
        /// Tells whether there is another element in the list
        /// </summary>
        public bool HasNext
        {
            get
            {
                if (Count == 0 || (!loop && index >= Count))
                {
                    return false;
                }
                return true;
            }
        }
        #endregion
    }
}
