//// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

//using System;

//namespace Bodoconsult.Finance.Payment.Banking.Extensions;

///// <summary>
///// String extension methods
///// </summary>
//public static class StringExtensions
//{
//    /// <summary>
//    /// Convert a string to a Guid. Invalid input strings end up in a Guid.Empty
//    /// </summary>
//    /// <param name="input">Input string</param>
//    /// <returns>Guid or Guid.Empty</returns>
//    public static Guid ToGuid(this string input)
//    {
//        return Guid.TryParse(input, out var guid) ? guid : Guid.Empty;
//    }
//}