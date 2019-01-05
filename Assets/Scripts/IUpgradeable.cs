public interface IUpgradeable {
  //Reflects the total amount invested
  int Invested { get; }

  //The maximum allowed value of Invested
  int MaxInvested { get; }

  //If possible, invests the specified amount
  //Returns false if the investment would cause Invested to be greater than MaxInvested, true otherwise
  bool Invest(int amount);

  //If possible, uninvests the specified amount
  bool Uninvest(int amount, out int refund);
}
