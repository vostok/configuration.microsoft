using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Configuration.Abstractions.SettingsTree;
using Vostok.Configuration.Microsoft.Tests.Helpers;

namespace Vostok.Configuration.Microsoft.Tests
{
    public class SettingsNodeFlattener_Tests
    {
        [TestCase("")]
        [TestCase("simpleName")]
        [TestCase("name_with_underscores")]
        [TestCase("name with spaces")]
        [TestCase("name:with:colons")]
        public void Should_FlattenValueSettingsNode(string name)
        {
            var settingsNode = new ValueNode(name, "testValue");
            var expectedFlatteringResult = new Dictionary<string, string>
            {
                {name, "testValue"}
            };

            var actualFlatteringResult = settingsNode.Flatten();

            actualFlatteringResult.Should().BeEquivalentTo(expectedFlatteringResult);
        }

        [TestCase("", 10)]
        [TestCase("simpleName", 10)]
        [TestCase("name with spaces", 100)]
        [TestCase("name:with:colons", 1000)]
        public void Should_FlattenSimpleObjectNode(string name, int childValuesAmount)
        {
            var expectedNames = RandomStringGenerator.CreateStrings(childValuesAmount, 16);
            var expectedValues = RandomStringGenerator.CreateStrings(childValuesAmount, 16);

            var childNodes = expectedNames.Zip(expectedValues, (s, s1) => new ValueNode(s, s1)).ToList();
            var settingsNode = new ObjectNode(name, childNodes);

            var expectedFlatteringResult = childNodes.ToDictionary(
                s => $"{name}:{s.Name}".TrimStart(':'),
                s => s.Value);

            var actualFlatteringResult = settingsNode.Flatten();

            actualFlatteringResult.Should().BeEquivalentTo(expectedFlatteringResult);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(100)]
        public void Should_FlattenArraySettingsNode(int arrayLength)
        {
            var expectedValues = RandomStringGenerator.CreateStrings(arrayLength, 16);

            var childNodes = expectedValues.Select(s => new ValueNode(s)).ToList();
            var settingsNode = new ArrayNode("testArray", childNodes);

            var expectedFlatteringResult = new Dictionary<string, string>();
            for (var i = 0; i < arrayLength; i++)
            {
                expectedFlatteringResult[$"testArray:{i}"] = expectedValues[i];
            }

            var actualFlatteringResult = settingsNode.Flatten();

            actualFlatteringResult.Should().BeEquivalentTo(expectedFlatteringResult);
        }

        [TestCase("root", "nested", "root:nested", 10)]
        [TestCase("", "nested", "nested", 10)]
        [TestCase("root", "", "root", 100)]
        [TestCase("", "", "", 100)]
        public void Should_FlattenNestedObjectSettingsNode(string rootNodeName, string nestedNodeName, string expectedPrefix, int childCount)
        {
            var childNames = RandomStringGenerator.CreateStrings(childCount, 16);
            var expectedValues = RandomStringGenerator.CreateStrings(childCount, 16);

            var childNodes = childNames.Zip(expectedValues, (s, s1) => new ValueNode(s, s1)).ToList();

            var nestedNode = new ObjectNode(nestedNodeName, childNodes);
            var settingsNode = new ObjectNode(rootNodeName, new[] {nestedNode});

            var expectedFlatteringResult = childNodes.ToDictionary(
                s => $"{expectedPrefix}:{s.Name}".TrimStart(':'),
                s => s.Value
            );

            var actualFlatteringResult = settingsNode.Flatten();

            actualFlatteringResult.Should().BeEquivalentTo(expectedFlatteringResult);
        }

        [TestCase("root", "nested", "root:nested", 10)]
        [TestCase("", "nested", "nested", 10)]
        [TestCase("root", "", "root", 100)]
        [TestCase("", "", "", 100)]
        public void Should_FlattenNestedArraySettingsNode(string rootNodeName, string nestedNodeName, string expectedPrefix, int arrayLength)
        {
            var expectedValues = RandomStringGenerator.CreateStrings(arrayLength, 16);

            var childNodes = expectedValues.Select(s => new ValueNode(s)).ToList();
            var arrayNode = new ArrayNode(nestedNodeName, childNodes);
            var settingsNode = new ObjectNode(rootNodeName, new[] {arrayNode});

            var expectedFlatteringResult = new Dictionary<string, string>();
            for (var i = 0; i < arrayLength; i++)
            {
                expectedFlatteringResult[$"{expectedPrefix}:{i}".TrimStart(':')] = expectedValues[i];
            }

            var actualFlatteringResult = settingsNode.Flatten();

            actualFlatteringResult.Should().BeEquivalentTo(expectedFlatteringResult);
        }
    }
}