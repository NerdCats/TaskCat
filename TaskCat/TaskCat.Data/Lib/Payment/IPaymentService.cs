﻿namespace TaskCat.Data.Lib.Payment
{
    using Model;
    using Model.Inventory;
    using Request;
    using Response;
    using System.Collections.Generic;
    using Entity;

    /// <summary>
    /// Payment service interface
    /// </summary>
    public partial interface IPaymentService
    {
        /// <summary>
        /// Load active payment methods
        /// </summary>      
        /// <returns>Payment methods</returns>
        IList<IPaymentMethod> GetActivePaymentMethods();

        /// <summary>
        /// Load all payment providers
        /// </summary>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <param name="filterByCountryId">Load records allowed only in a specified country; pass 0 to load all records</param>
        /// <returns>Payment providers</returns>
        IList<IPaymentMethod> GetAllPaymentMethods(int storeId = 0, int filterByCountryId = 0);

        /// <summary>
        /// Get a payment method by its name
        /// </summary>
        /// <param name="name">payment method name</param>
        /// <returns></returns>
        IPaymentMethod GetPaymentMethodByName(string name);

        /// <summary>
        /// Gets a list of coutnry identifiers in which a certain payment method is now allowed
        /// </summary>
        /// <param name="paymentMethod">Payment method</param>
        /// <returns>A list of country identifiers</returns>
        IList<int> GetRestrictedCountryIds(IPaymentMethod paymentMethod);

        /// <summary>
        /// Saves a list of coutnry identifiers in which a certain payment method is now allowed
        /// </summary>
        /// <param name="paymentMethod">Payment method</param>
        /// <param name="countryIds">A list of country identifiers</param>
        void SaveRestrictedCountryIds(IPaymentMethod paymentMethod, List<int> countryIds);


        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        ProcessPaymentResponse ProcessPayment(ProcessPaymentRequest processPaymentRequest);

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="job">Job that is associated with the payment method</param>
        /// <returns>Result</returns>
        bool CanRePostProcessPayment(Job job);


        /// <summary>
        /// Gets an additional handling fee of a payment method
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <param name="paymentMethodName">Payment method name</param>
        /// <returns>Additional handling fee</returns>
        decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart, string paymentMethodName);



        /// <summary>
        /// Gets a value indicating whether capture is supported by payment method
        /// </summary>
        /// <param name="paymentMethodSystemName">Payment method system name</param>
        /// <returns>A value indicating whether capture is supported</returns>
        bool SupportCapture(string paymentMethodSystemName);

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        CapturePaymentResponse Capture(CapturePaymentRequest capturePaymentRequest);



        /// <summary>
        /// Gets a value indicating whether partial refund is supported by payment method
        /// </summary>
        /// <param name="paymentMethodSystemName">Payment method system name</param>
        /// <returns>A value indicating whether partial refund is supported</returns>
        bool SupportPartiallyRefund(string paymentMethodSystemName);

        /// <summary>
        /// Gets a value indicating whether refund is supported by payment method
        /// </summary>
        /// <param name="paymentMethodSystemName">Payment method system name</param>
        /// <returns>A value indicating whether refund is supported</returns>
        bool SupportRefund(string paymentMethodSystemName);

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        RefundPaymentResponse Refund(RefundPaymentRequest refundPaymentRequest);



        /// <summary>
        /// Gets a value indicating whether void is supported by payment method
        /// </summary>
        /// <param name="paymentMethodSystemName">Payment method system name</param>
        /// <returns>A value indicating whether void is supported</returns>
        bool SupportVoid(string paymentMethodSystemName);

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        VoidPaymentResponse Void(VoidPaymentRequest voidPaymentRequest);

        /// <summary>
        /// Gets masked credit card number
        /// </summary>
        /// <param name="creditCardNumber">Credit card number</param>
        /// <returns>Masked credit card number</returns>
        string GetMaskedCreditCardNumber(string creditCardNumber);

    }
}
