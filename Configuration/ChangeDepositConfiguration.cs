namespace UFF.VendingMachine.Configuration
{

    /// POCO container maps back to configuation - PurseConfiguration
    public class ChangeDepositConfiguration
    {
        /// <summary>
        /// The coin nominal in fractions.
        /// </summary>
        public int Nominal { get; set; }

        /// <summary>
        /// The number coins in the set.
        /// </summary>
        public int Number { get; set; }
    }
}
