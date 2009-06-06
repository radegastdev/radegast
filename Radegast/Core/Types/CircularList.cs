using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
