using System;
using System.Collections.Generic;
using System.Text;

namespace RadegastNc
{
    public class MoneyBalanceEventArgs : EventArgs
    {
        private int newAmount;

        public MoneyBalanceEventArgs(int newAmount)
        {
            this.newAmount = newAmount;
        }

        public int NewAmount
        {
            get { return newAmount; }
        }
    }
}
