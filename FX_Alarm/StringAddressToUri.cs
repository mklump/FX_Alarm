// -----------------------------------------------------------------------
// <copyright file="StringAddressToUri.cs" company="PIPs for Heaven, LLC">
//     Copyright (c) PIPs for Heaven, LLC.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace FX_Alarm
{
    using System;
    using System.Windows.Data;
    using System.Windows.Controls;

    /// <summary>
    /// StaticResource string address to Uri instance converter
    /// </summary>
    public class StringAddressToUriConverter : IValueConverter
    {
        /// <summary>
        /// Default constructor as needed
        /// </summary>
        public StringAddressToUriConverter()
        {
        }

        /// <summary>
        /// Mandatory Convert() funtion implementation from the IValueConverter interface contract
        /// </summary>
        /// <param name="value">String address to convert to a Uri instance</param>
        /// <param name="targetType">not used</param>
        /// <param name="parameter">not used</param>
        /// <param name="culture">not used</param>
        /// <returns>A Uri instance from the specified string</returns>
        public object Convert(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            string input = value as string;
            return String.IsNullOrEmpty(input) ?
                null : new Uri(input, UriKind.Absolute);
        }

        /// <summary>
        /// Mandatory ConvertBack() funtion implementation from the IValueConverter interface contract
        /// </summary>
        /// <param name="value">Uri instance to convert back to a string address</param>
        /// <param name="targetType">not used</param>
        /// <param name="parameter">not used</param>
        /// <param name="culture">not used</param>
        /// <returns>String representation of this Uri instance Path and Query</returns>
        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            Uri input = value as Uri;
            return input == null ?
                String.Empty : input.ToString();
        }
    } // End of public class StringAddressToUriConverter : IValueConverter

    ///// <summary>
    ///// Uri format validation class for use with the StringAddressToUriConverter
    ///// </summary>
    //public class UriValidationRule : ValidationRule
    //{
    //    /// <summary>
    //    /// Default constructor as needed
    //    /// </summary>
    //    public UriValidationRule()
    //    {
    //    }

    //    /// <summary>
    //    /// Fewest argument number overriden Validate function from the ValidationRule base class
    //    /// </summary>
    //    /// <param name="value">Input value to validate</param>
    //    /// <param name="cultureInfo">Localization language and culture parameter</param>
    //    /// <returns>The validation rule result</returns>
    //    public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
    //    {
    //        string input = value as string;
    //        if (String.IsNullOrEmpty(input)) // Valid input, converts to null.
    //        {
    //            return new ValidationResult(true, null);
    //        }
    //        Uri outUri;
    //        if (Uri.TryCreate(input, UriKind.Absolute, out outUri))
    //        {
    //            return new ValidationResult(true, null);
    //        }
    //        else
    //        {
    //            return new ValidationResult(false, "String is not a valid URI");
    //        }
    //    }
    //}
} // End of namespace FX_Alarm