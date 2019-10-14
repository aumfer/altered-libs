using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altered.Pipeline.Serilog
{
    /// <summary>
    /// Enriches log events with a value.
    /// </summary>
    public sealed class ValueEnricher : ILogEventEnricher
    {
        readonly string variableName;
        readonly string variableValue;

        public ValueEnricher(string variableName, string variableValue)
        {
            this.variableName = variableName;
            this.variableValue = variableValue;
        }

        /// <summary>
        /// Enrich the log event.
        /// </summary>
        /// <param name="logEvent">The log event to enrich.</param>
        /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var property = propertyFactory.CreateProperty(variableName, variableValue);
            logEvent.AddOrUpdateProperty(property);
        }
    }

    public static class ValueEnricherExtensions
    {
        public static LoggerConfiguration WithValue(this LoggerEnrichmentConfiguration loggerEnrichmentConfiguration, string variableName, string variableValue) =>
            loggerEnrichmentConfiguration.With(new ValueEnricher(variableName, variableValue));
        public static LoggerConfiguration WithEnvironmentVariable(this LoggerEnrichmentConfiguration loggerEnrichmentConfiguration, string variableName, string environmentVariableName = null) =>
            loggerEnrichmentConfiguration.WithValue(variableName, Environment.GetEnvironmentVariable(environmentVariableName ?? variableName));
    }
}
