using System;

namespace Payment
{   
    public class Wallet
    {
        public bool IsOwnedByNonProfitOrganization { get; set; }
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
                actualValue = TryWithdrawMoney(amount);
            }
            catch(TaxFreeException)
            {
                actualValue = amount;
            }
            catch(NeedTaxesException)
            {
                var tax = GetTaxes();
                _taxOffice.Pay(tax);
                actualValue = (amount + tax);
            }
            catch(InsufficientBalanceException)
            {
                _loanShark.Alert();
                throw;
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
                if (Balance - wishedAmount - GetTaxes() < 0)
                    throw new InsufficientBalanceException();
                throw new NeedTaxesException();
            }
            return wishedAmount;
        }

        int GetTaxes()
        {
            if (IsOwnedByNonProfitOrganization)
                throw new TaxFreeException();
            return Tax;
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
    public class TaxFreeException : Exception {};
}