using System;
using System.Reflection.Emit;

namespace Payment
{   
    public class Wallet
    {
        public Owner WalletOwner { get; set; }
        const int Tax = 2;

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
            var actualValue = 0;
            try
            {
                try
                {
                    actualValue = TryWithdrawMoney(amount);
                }
                catch(NeedTaxesException)
                {
                    _taxOffice.Pay(GetSocialSecurityNumber(), Tax);
                    actualValue = (amount + Tax);
                }
                catch(InsufficientBalanceException)
                {
                    _loanShark.Alert();
                    throw;
                }
            }
            catch(NullReferenceException)
            {
                actualValue = amount;
            }
            Balance -= actualValue;
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
                if (Balance - wishedAmount - Tax < 0)
                    throw new InsufficientBalanceException();
                throw new NeedTaxesException();
            }
            return wishedAmount;
        }

        string GetSocialSecurityNumber()
        {
            return WalletOwner.SocialSecurityNumber;
        }
    }

    public interface ITaxOffice
    {
        void Pay(string nationalSecurityNumber, int tax);
    }

    public interface ILoanShark
    {
        void Alert();
    }

    public class Owner
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string SocialSecurityNumber { get; set; }
    }

    public class InsufficientBalanceException : Exception {}
    public class EmptyWalletException : Exception {}
    public class NeedTaxesException : Exception {};
    public class TaxFreeException : Exception {};
}