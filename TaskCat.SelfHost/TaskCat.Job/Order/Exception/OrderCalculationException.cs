namespace TaskCat.Job.Order.Exceptions
{
    using System;

    /// <summary>
    /// Generic Exception to define a calculation exception for IOrderCalculationService
    /// </summary>
    public class OrderCalculationException : ArgumentException
    { 
        public OrderCalculationException(string paramName) : base("Calculation Verification Failed", paramName)
        {
        }
    }
}