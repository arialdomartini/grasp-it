using NUnit.Framework;
using FluentAssertions;
using System;
using NSubstitute;

namespace Payment.Test
{
    public class WalletTest
    {
        Wallet sut;
        ITaxOffice _taxOffice;
        ILoanShark _loanShark;

        [SetUp]
        public void SetUp()
        {
            _taxOffice = Substitute.For<ITaxOffice>();
            _loanShark = Substitute.For<ILoanShark>();
            sut = new Wallet(_taxOffice, _loanShark);
        }

        [Test]
        public void ANewWalletIsEmpty()
        {
            sut.Balance.Should().Be(0);
        }

        [TestCase(100)]
        [TestCase(0)]
        [TestCase(1000)]
        [TestCase(6)]
        public void ShouldBePossibleToDepositMoney(int balance)
        {
            sut.Deposit(balance);

            sut.Balance.Should().Be(balance);
        }

        [TestCase(0, 1, 1)]
        [TestCase(10, 10, 20)]
        [TestCase(5, 7, 12)]
        [TestCase(7, 5, 12)]
        public void DepositedMoneyAccumulate(int balance1, int balance2, int finalBalance)
        {
            sut.Deposit(balance1);
            sut.Deposit(balance2);

            sut.Balance.Should().Be(finalBalance);
        }

        [Test]
        public void CannotWithdrawFromAnEmptyWallet()
        {
            sut.Invoking(s => s.Withdraw(2))

            .ShouldThrow<EmptyWalletException>();
        }

        [TestCase(10, 10, 0)]
        [TestCase(10, 8, 2)]
        [TestCase(100, 0, 100)]
        [TestCase(10, 6, 4)]
        public void WithdrawingUpdatesTheBalance(int initialBalance, int withdrawnAmount, int finalBalance)
        {
            sut.Deposit(initialBalance);

            sut.Withdraw(withdrawnAmount);

            sut.Balance.Should().Be(finalBalance);
        }

        [TestCase(10, 100)]
        [TestCase(10, 18)]
        [TestCase(100, 101)]
        [TestCase(1, 2)]
        public void WithdrawingMoreThanAvailableDoesNotChange(int initialBalance, int withdrawnAmount)
        {
            sut.Deposit(initialBalance);

            try {
                sut.Withdraw(withdrawnAmount);
            }
            catch
            {
                sut.Balance.Should().Be(initialBalance);
            }
        }

        [TestCase(10, 100)]
        [TestCase(10, 18)]
        [TestCase(100, 101)]
        [TestCase(1, 2)]
        public void WithdrawingMoreThanAvailableShouldAlertTheLoanShark(int initialBalance, int withdrawnAmount)
        {
            sut.Deposit(initialBalance);

            try {
                sut.Withdraw(withdrawnAmount);
            }
            catch
            {
                _loanShark.Received().Alert();
            }
        }

        [TestCase(10, 100)]
        [TestCase(10, 18)]
        [TestCase(100, 101)]
        [TestCase(1, 2)]
        public void WithdrawingMoreThanAvailableThrowsAnException(int initialBalance, int withdrawnAmount)
        {
            sut.Deposit(initialBalance);

            sut.Invoking(s => s.Withdraw(withdrawnAmount))

           .ShouldThrow<InsufficientBalanceException>();
        }

        [TestCase(-1)]
        [TestCase(-2)]
        [TestCase(-100)]
        public void CantWithdrawNegativeAmounts(int withdrawnAmount)
        {
            sut.Deposit(100);

            sut.Invoking(s => s.Withdraw(withdrawnAmount))

           .ShouldThrow<ArgumentException>();
        }

        [TestCase(1001)]
        [TestCase(1002)]
        [TestCase(2000)]
        public void WithdrawingMoreThan1000EuroInvolveA2EuroTax(int requiredAmount)
        {
            var initialBalance = 10000;
            sut.Deposit(initialBalance);

            sut.Withdraw(requiredAmount);

            sut.Balance.Should().Be(initialBalance - requiredAmount - 2);
        }

        [TestCase(1001)]
        [TestCase(1002)]
        [TestCase(2000)]
        public void TaxOfficeReceivesTaxes(int requiredAmount)
        {
            const int initialBalance = 10000;
            sut.Deposit(initialBalance);

            sut.Withdraw(requiredAmount);

            _taxOffice.Received().Pay(2);
        }

        [TestCase(1001, 1001)]
        [TestCase(1002, 1001)]
        [TestCase(2000, 1999)]
        [TestCase(2000, 2000)]
        public void ShouldThrowAnExceptionIfBalanceIsInsufficientToPayTaxes(int initialBalance, int requiredAmount)
        {
            sut.Deposit(initialBalance);

            sut.Invoking(s => s.Withdraw(requiredAmount))

           .ShouldThrow<InsufficientBalanceException>();
        }
    }
}