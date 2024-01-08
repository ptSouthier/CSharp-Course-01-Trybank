namespace Trybank.Lib;

public class TrybankLib
{
    public bool Logged;
    public int loggedUser;

    public int[,] Bank;
    public int registeredAccounts;
    private int maxAccounts = 50;
    private int accNumberIndex = 0;
    private int accAgencyIndex = 1;
    private int accPasswordIndex = 2;
    private int accBalanceIndex = 3;
    private int noUserLoggedValue = -99;

    public TrybankLib()
    {
        loggedUser = noUserLoggedValue;
        registeredAccounts = 0;
        Logged = false;
        Bank = new int[maxAccounts, 4];
    }

    // 1. Construa a funcionalidade de cadastrar novas contas
    public void RegisterAccount(int number, int agency, int pass)
    {
        for (int index = 0; index < registeredAccounts; index += 1)
        {
            int accNumber = Bank[index, accNumberIndex];
            int accAgency = Bank[index, accAgencyIndex];
            if (number == accNumber && agency == accAgency)
            {
                throw new ArgumentException("A conta já está sendo usada!");
            }
        }

        int newAccBalance = 0;

        Bank[registeredAccounts, accNumberIndex] = number;
        Bank[registeredAccounts, accAgencyIndex] = agency;
        Bank[registeredAccounts, accPasswordIndex] = pass;
        Bank[registeredAccounts, accBalanceIndex] = newAccBalance;
        registeredAccounts += 1;
    }

    // 2. Construa a funcionalidade de fazer Login
    public void Login(int number, int agency, int pass)
    {
        if (Logged)
        {
            throw new AccessViolationException("Usuário já está logado");
        }

        for (int index = 0; index < registeredAccounts; index += 1)
        {
            int accNumber = Bank[index, accNumberIndex];
            int accAgency = Bank[index, accAgencyIndex];
            int accPassword = Bank[index, accPasswordIndex];

            if (number == accNumber && agency == accAgency)
            {
                if (pass == accPassword)
                {
                    Logged = true;
                    loggedUser = index;
                }
                else if (pass != accPassword)
                {
                    throw new ArgumentException("Senha incorreta");
                }
            }
        }

        if (!Logged && loggedUser < 0)
        {
            throw new ArgumentException("Agência + Conta não encontrada");
        }
    }

    private void VerifyLogged()
    {
        if (!Logged)
        {
            throw new AccessViolationException("Usuário não está logado");
        }
    }

    // 3. Construa a funcionalidade de fazer Logout
    public void Logout()
    {
        VerifyLogged();
        
        Logged = false;
        loggedUser = noUserLoggedValue;
    }

    // 4. Construa a funcionalidade de checar o saldo
    public int CheckBalance()
    {
        VerifyLogged();

        int loggedUserAccBalance = Bank[loggedUser, accBalanceIndex];
        return loggedUserAccBalance;
    }

    // 5. Construa a funcionalidade de depositar dinheiro
    public void Deposit(int value)
    {
        VerifyLogged();

        int loggedUserAccBalance = Bank[loggedUser, accBalanceIndex];
        Bank[loggedUser, accBalanceIndex] = value + loggedUserAccBalance;
    }

    // 6. Construa a funcionalidade de sacar dinheiro
    public void Withdraw(int value)
    {
        VerifyLogged();

        int loggedUserAccBalance = Bank[loggedUser, accBalanceIndex];
        int accBalanceAfterWithdraw = loggedUserAccBalance - value;
        if (accBalanceAfterWithdraw < 0)
        {
            throw new InvalidOperationException("Saldo insuficiente");
        }
        else
        {
            Bank[loggedUser, accBalanceIndex] = accBalanceAfterWithdraw;
        }
    }

    // 7. Construa a funcionalidade de transferir dinheiro entre contas
    public void Transfer(int destinationNumber, int destinationAgency, int value)
    {
        VerifyLogged();

        int loggedUserAccBalance = Bank[loggedUser, accBalanceIndex];
        int accBalanceAfterTransfer = loggedUserAccBalance - value;
        if (accBalanceAfterTransfer < 0)
        {
            throw new InvalidOperationException("Saldo insuficiente");
        }
        else
        {
            bool successfulTransfer = false;
            
            for (int index = 0; index < registeredAccounts; index += 1)
            {
                int accNumber = Bank[index, accNumberIndex];
                int accAgency = Bank[index, accAgencyIndex];
                if (destinationNumber == accNumber && destinationAgency == accAgency)
                {
                    int destinationAccountBalance = Bank[index, accBalanceIndex];
                    Bank[loggedUser, accBalanceIndex] = accBalanceAfterTransfer;
                    Bank[index, accBalanceIndex] = destinationAccountBalance + value;
                    successfulTransfer = true;
                }
            }

            if (!successfulTransfer)
            {
                throw new InvalidOperationException("Conta destino inexistente");
            }
        }
    }

   
}
