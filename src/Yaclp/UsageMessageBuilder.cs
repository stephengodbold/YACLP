﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Yaclp.Attributes;
using Yaclp.Extensions;

namespace Yaclp
{
    public class UsageMessageBuilder
    {
        private readonly Type _parametersType;

        public UsageMessageBuilder(Type parametersType)
        {
            _parametersType = parametersType;
        }

        public string Build()
        {
            var sb = new StringBuilder();

            var executableName = Environment.GetCommandLineArgs()[0];

            var mandatoryArgsMessage = BuildMandatoryArgsCommandLineMessage();
            var optionalArgsMessage = BuildOptionalArgsCommandLineMessage();

            sb.AppendLine("Usage:");
            sb.AppendFormat("\t{0} {1} {2}", executableName, mandatoryArgsMessage, optionalArgsMessage);
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("Details:");
            foreach (var descriptionMessage in BuildParameterDescriptionMessages())
            {
                sb.AppendFormat("\t{0}", descriptionMessage);
                sb.AppendLine();
                sb.AppendLine();
            }
            sb.AppendLine();

            return sb.ToString();
        }

        private string BuildMandatoryArgsCommandLineMessage()
        {
            var args = _parametersType.GetMandatoryProperties()
                .Select(prop => "<{0}>".FormatWith(prop.Name))
                .ToArray();

            return string.Join(" ", args);
        }

        private string BuildOptionalArgsCommandLineMessage()
        {
            var args = _parametersType.GetOptionalProperties()
                .Select(prop => "[/{0}:<{0}>]".FormatWith(prop.Name))
                .ToArray();

            return string.Join(" ", args);
        }

        private IEnumerable<string> BuildParameterDescriptionMessages()
        {
            var properties = _parametersType
                .GetMandatoryProperties()
                .Union(_parametersType.GetOptionalProperties())
                .ToArray();

            var maxPropertyNameLength = properties.Max(p => p.Name.Length);

            return properties
                .Select(propertyInfo => BuildParameterDescriptionMessage(propertyInfo, maxPropertyNameLength))
                .ToArray();
        }

        private static string BuildParameterDescriptionMessage(PropertyInfo propertyInfo, int maxPropertyLength)
        {
            var descriptionAttribute = propertyInfo.GetCustomAttributes<ParameterDescriptionAttribute>().FirstOrDefault();
            var description = descriptionAttribute != null ? descriptionAttribute.Description : string.Empty;

            var exampleAttribute = propertyInfo.GetCustomAttributes<ParameterExampleAttribute>().FirstOrDefault();
            var example = exampleAttribute != null ? exampleAttribute.Example : null;

            var defaultAttribute = propertyInfo.GetCustomAttributes<ParameterDefaultAttribute>().FirstOrDefault();
            var defaultValue = defaultAttribute != null ? defaultAttribute.Value : null;

            var explanation = new StringBuilder();

            if (string.IsNullOrWhiteSpace(description) &&
                string.IsNullOrWhiteSpace(example) &&
                string.IsNullOrWhiteSpace(defaultValue))
            {
                explanation.Append("(No idea. Sorry.)");
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(description))
                    explanation.AppendFormat("{0} ", description);
                
                if (!string.IsNullOrWhiteSpace(example) || !string.IsNullOrWhiteSpace(defaultValue))
                    explanation.AppendFormat("e.g. '{0}'", example ?? defaultValue);
            }

            var sb = new StringBuilder();
            sb.AppendFormat("{0}", propertyInfo.Name.PadRight(maxPropertyLength));
            sb.Append("\t");
            sb.Append(explanation);
            return sb.ToString();
        }
    }
}