using System;

namespace Payment
{   
    public class Wallet
    {
        const int tax = 2;

        ITaxOffice _taxOffice;
        ILoanShark _loanShark;

        public Wallet(ITaxOffice taxOffice, ILoanShark loanShark)
        {
            _taxOffice = taxOffice;
            _loanShark = loanShark;
        }

        public int Balance { get; private set; }

        public void Deposit(int depositedAmount)
        {
            Balance += depositedAmount;
        }

        public void Withdraw(int amount)
        {
            if (amount < 0)
                throw new ArgumentException();
            try
            {
                Balance -= TryWithdrawMoney(amount);
            }
            catch(NeedTaxesException)
            {
                _taxOffice.Pay(tax);
                Balance -= (amount + tax);
            }
            catch(InsufficientBalanceException)
            {
                _loanShark.Alert();
                throw;
            }
        }

        public int TryWithdrawMoney(int wishedAmount)
        {
            if (Balance == 0)
                throw new EmptyWalletException();
            if (wishedAmount > Balance)
            {
                throw new InsufficientBalanceException();
            }
            if(wishedAmount > 1000)
            {
                if (Balance - wishedAmount - tax < 0)
                    throw new InsufficientBalanceException();
                throw new NeedTaxesException();
            }
            return wishedAmount;
        }
    }

    public interface ITaxOffice
    {
        void Pay(int tax);
    }

    public interface ILoanShark
    {
        void Alert();
    }

    public class InsufficientBalanceException : Exception {}
    public class EmptyWalletException : Exception {}
    public class NeedTaxesException : Exception {};
}