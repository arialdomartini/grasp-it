using NUnit.Framework;
using FluentAssertions;
using System;
using NSubstitute;
using System.IO;
using System.Linq;

namespace Payment.Test
{
    public class WalletTest
    {
        Wallet _sut;
        ITaxOffice _taxOffice;
        ILoanShark _loanShark;

        [SetUp]
        public void SetUp()
        {
            _taxOffice = Substitute.For<ITaxOffice>();
            _loanShark = Substitute.For<ILoanShark>();
            _sut = new Wallet(_taxOffice, _loanShark);
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(_sut.LogFile);
        }

        [Test]
        public void ANewWalletIsEmpty()
        {
            _sut.Balance.Should().Be(0);
        }


        [Test]
        public void ByDefaultWalletIsNotOwnedByAnyone()
        {
            _sut.WalletOwner.Should().BeNull();
        }

        [TestCase(100)]
        [TestCase(0)]
        [TestCase(1000)]
        [TestCase(6)]
        public void ShouldBePossibleToDepositMoney(int balance)
        {
            _sut.Deposit(balance);

            _sut.Balance.Should().Be(balance);
        }

        [TestCase(0, 1, 1)]
        [TestCase(10, 10, 20)]
        [TestCase(5, 7, 12)]
        [TestCase(7, 5, 12)]
        public void DepositedMoneyAccumulate(int balance1, int balance2, int finalBalance)
        {
            _sut.Deposit(balance1);
            _sut.Deposit(balance2);

            _sut.Balance.Should().Be(finalBalance);
        }

        [Test]
        public void CannotWithdrawFromAnEmptyWallet()
        {
            _sut.Invoking(s => s.Withdraw(2))

            .ShouldThrow<EmptyWalletException>();
        }

        [TestCase(10, 10, 0)]
        [TestCase(10, 8, 2)]
        [TestCase(100, 0, 100)]
        [TestCase(10, 6, 4)]
        public void WithdrawingUpdatesTheBalance(int initialBalance, int withdrawnAmount, int finalBalance)
        {
            _sut.Deposit(initialBalance);

            _sut.Withdraw(withdrawnAmount);

            _sut.Balance.Should().Be(finalBalance);
        }

        [TestCase(10, 100)]
        [TestCase(10, 18)]
        [TestCase(100, 101)]
        [TestCase(1, 2)]
        public void WithdrawingMoreThanAvailableDoesNotChange(int initialBalance, int withdrawnAmount)
        {
            _sut.Deposit(initialBalance);

            try {
                _sut.Withdraw(withdrawnAmount);
            }
            catch
            {
                _sut.Balance.Should().Be(initialBalance);
            }
        }

        [TestCase(10, 100)]
        [TestCase(10, 18)]
        [TestCase(100, 101)]
        [TestCase(1, 2)]
        public void WithdrawingMoreThanAvailableShouldAlertTheLoanShark(int initialBalance, int withdrawnAmount)
        {
            _sut.Deposit(initialBalance);

            try {
                _sut.Withdraw(withdrawnAmount);
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
            _sut.Deposit(initialBalance);

            _sut.Invoking(s => s.Withdraw(withdrawnAmount))

           .ShouldThrow<InsufficientBalanceException>();
        }

        [TestCase(-1)]
        [TestCase(-2)]
        [TestCase(-100)]
        public void CantWithdrawNegativeAmounts(int withdrawnAmount)
        {
            _sut.Deposit(100);

            _sut.Invoking(s => s.Withdraw(withdrawnAmount))

           .ShouldThrow<ArgumentException>();
        }

        [TestCase(1001)]
        [TestCase(1002)]
        [TestCase(2000)]
        public void WithdrawingMoreThan1000EuroInvolveA2EuroTax(int requiredAmount)
        {
            var initialBalance = 10000;
            _sut.Deposit(initialBalance);
            _sut.WalletOwner = new Owner { FirstName = "foo", SecondName = "bar", SocialSecurityNumber = "1234" };

            _sut.Withdraw(requiredAmount);

            _sut.Balance.Should().Be(initialBalance - requiredAmount - 2);
        }


        [TestCase(1001)]
        [TestCase(1002)]
        [TestCase(2000)]
        public void IfTheWalletOwnerIsANonProfitOrganizationTaxationIsApplied(int requiredAmount)
        {
            var initialBalance = 10000;
            _sut.WalletOwner = null;
            _sut.Deposit(initialBalance);

            _sut.Withdraw(requiredAmount);

            _sut.Balance.Should().Be(initialBalance - requiredAmount);
        }



        [TestCase(1001)]
        [TestCase(1002)]
        [TestCase(2000)]
        public void TaxOfficeReceivesTaxes(int requiredAmount)
        {
            const int initialBalance = 10000;
            _sut.WalletOwner = new Owner { FirstName = "foo", SecondName = "bar", SocialSecurityNumber = "1234" };
            _sut.Deposit(initialBalance);

            _sut.Withdraw(requiredAmount);

            _taxOffice.Received().Pay("1234", 2);
        }

        [TestCase(1001, 1001)]
        [TestCase(1002, 1001)]
        [TestCase(2000, 1999)]
        [TestCase(2000, 2000)]
        public void ShouldThrowAnExceptionIfBalanceIsInsufficientToPayTaxes(int initialBalance, int requiredAmount)
        {
            _sut.Deposit(initialBalance);

            _sut.Invoking(s => s.Withdraw(requiredAmount))

           .ShouldThrow<InsufficientBalanceException>();
        }

        [Test]
        public void PaymentOfTaxesIsLoggedToFile()
        {
            const int initialBalance = 10000;
            _sut.WalletOwner = new Owner { FirstName = "foo", SecondName = "bar", SocialSecurityNumber = "1234" };
            _sut.Deposit(initialBalance);

            _sut.Withdraw(1005);

            var logContent = File.ReadAllLines(_sut.LogFile);
            logContent.First().Should().Be("1234 paid 2");
        }
    }
}