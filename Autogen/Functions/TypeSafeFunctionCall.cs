﻿using AutoGen.Core;
namespace AutogenDotNet.Functions
{
    public partial class TypeSafeFunctionCall
    {
        /// <summary>
        /// Get weather report
        /// </summary>
        /// <param name="city">city</param>
        /// <param name="date">date</param>
        [Function]
        public async Task<string> WeatherReport(string city, string date)
        {
            return await Task.FromResult($"Weather report for {city} on {date} is sunny");
        }
    }
}
