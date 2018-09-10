﻿/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using Python.Runtime;
using QuantConnect.Orders;
using QuantConnect.Securities;
using System;

namespace QuantConnect.Python
{
    /// <summary>
    /// Wraps a <see cref="PyObject"/> object that represents a security's model of buying power
    /// </summary>
    public class BuyingPowerModelPythonWrapper : IBuyingPowerModel
    {
        private readonly dynamic _model;

        /// <summary>
        /// Constructor for initialising the <see cref="BuyingPowerModelPythonWrapper"/> class with wrapped <see cref="PyObject"/> object
        /// </summary>
        /// <param name="model">Represents a security's model of buying power</param>
        public BuyingPowerModelPythonWrapper(PyObject model)
        {
            using (Py.GIL())
            {
                foreach (var attributeName in new[] { "GetBuyingPower", "GetLeverage", "GetMaximumOrderQuantityForTargetValue", "GetReservedBuyingPowerForPosition", "HasSufficientBuyingPowerForOrder", "SetLeverage" })
                {
                    if (!model.HasAttr(attributeName))
                    {
                        throw new NotImplementedException($"IBuyingPowerModel.{attributeName} must be implemented. Please implement this missing method on {model.GetPythonType()}");
                    }
                }
            }
            _model = model;
        }

        /// <summary>
        /// Gets the buying power available for a trade
        /// </summary>
        /// <param name="portfolio">The algorithm's portfolio</param>
        /// <param name="security">The security to be traded</param>
        /// <param name="direction">The direction of the trade</param>
        /// <returns>The buying power available for the trade</returns>
        public decimal GetBuyingPower(SecurityPortfolioManager portfolio, Security security, OrderDirection direction)
        {
            using (Py.GIL())
            {
                return _model.GetBuyingPower(portfolio, security, direction);
            }
        }

        /// <summary>
        /// Gets the current leverage of the security
        /// </summary>
        /// <param name="security">The security to get leverage for</param>
        /// <returns>The current leverage in the security</returns>
        public decimal GetLeverage(Security security)
        {
            using (Py.GIL())
            {
                return _model.GetLeverage(security);
            }
        }

        /// <summary>
        /// Get the maximum market order quantity to obtain a position with a given value in account currency
        /// </summary>
        /// <param name="portfolio">The algorithm's portfolio</param>
        /// <param name="security">The security to be traded</param>
        /// <param name="target">Target percentage holdings</param>
        /// <returns>Returns the maximum allowed market order quantity and if zero, also the reason</returns>
        public GetMaximumOrderQuantityForTargetValueResult GetMaximumOrderQuantityForTargetValue(SecurityPortfolioManager portfolio, Security security, decimal target)
        {
            using (Py.GIL())
            {
                return _model.GetMaximumOrderQuantityForTargetValue(portfolio, security, target);
            }
        }

        /// <summary>
        /// Gets the amount of buying power reserved to maintain the specified position
        /// </summary>
        /// <param name="security">The security for the position</param>
        /// <returns>The reserved buying power in account currency</returns>
        public decimal GetReservedBuyingPowerForPosition(Security security)
        {
            using (Py.GIL())
            {
                return _model.GetReservedBuyingPowerForPosition(security);
            }
        }

        /// <summary>
        /// Check if there is sufficient buying power to execute this order.
        /// </summary>
        /// <param name="portfolio">The algorithm's portfolio</param>
        /// <param name="security">The security to be traded</param>
        /// <param name="order">The order to be checked</param>
        /// <returns>Returns buying power information for an order</returns>
        public HasSufficientBuyingPowerForOrderResult HasSufficientBuyingPowerForOrder(SecurityPortfolioManager portfolio, Security security, Order order)
        {
            using (Py.GIL())
            {
                return _model.HasSufficientBuyingPowerForOrder(portfolio, security, order);
            }
        }

        /// <summary>
        /// Sets the leverage for the applicable securities, i.e, equities
        /// </summary>
        /// <remarks>
        /// This is added to maintain backwards compatibility with the old margin/leverage system
        /// </remarks>
        /// <param name="security">The security to set leverage for</param>
        /// <param name="leverage">The new leverage</param>
        public void SetLeverage(Security security, decimal leverage)
        {
            using (Py.GIL())
            {
                _model.SetLeverage(security, leverage);
            }
        }
    }
}