using System;
using System.Collections.Generic;

namespace FinanceManagementSystem
{
    // b. Record type representing a financial transaction
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    // c. Interface for transaction processing behavior
    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    // d. Concrete processor implementations
    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Bank Transfer] Processed {transaction.Amount:C} for {transaction.Category}.");
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Mobile Money] Processed {transaction.Amount:C} for {transaction.Category}.");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Crypto Wallet] Processed {transaction.Amount:C} for {transaction.Category}.");
        }
    }

    // e. Base Account class
    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber;
            Balance = initialBalance;
        }

        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance -= transaction.Amount;
        }
    }

    // f. Sealed SavingsAccount class
    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance)
            : base(accountNumber, initialBalance)
        {
        }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine("Insufficient funds");
                return;
            }

            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction applied. Updated balance for account {AccountNumber}: {Balance:C}");
        }
    }

    // g. FinanceApp - integrates and simulates the system
    public class FinanceApp
    {
        private List<Transaction> _transactions = new List<Transaction>();

        public void Run()
        {
            // i. Instantiate a SavingsAccount
            var savingsAccount = new SavingsAccount("ACC-001", 1000m);

            // ii. Create three Transaction records
            var transaction1 = new Transaction(1, DateTime.Now, 150m, "Groceries");
            var transaction2 = new Transaction(2, DateTime.Now, 200m, "Utilities");
            var transaction3 = new Transaction(3, DateTime.Now, 5000m, "Entertainment"); // deliberately exceeds balance to demo insufficient funds

            // iii. Use processors to process each transaction
            ITransactionProcessor mobileMoneyProcessor = new MobileMoneyProcessor();
            ITransactionProcessor bankTransferProcessor = new BankTransferProcessor();
            ITransactionProcessor cryptoWalletProcessor = new CryptoWalletProcessor();

            mobileMoneyProcessor.Process(transaction1);
            bankTransferProcessor.Process(transaction2);
            cryptoWalletProcessor.Process(transaction3);

            // iv. Apply each transaction to the SavingsAccount
            savingsAccount.ApplyTransaction(transaction1);
            savingsAccount.ApplyTransaction(transaction2);
            savingsAccount.ApplyTransaction(transaction3);

            // v. Add all transactions to _transactions
            _transactions.Add(transaction1);
            _transactions.Add(transaction2);
            _transactions.Add(transaction3);

            Console.WriteLine($"\nTotal transactions recorded: {_transactions.Count}");
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var app = new FinanceApp();
            app.Run();
        }
    }
}
