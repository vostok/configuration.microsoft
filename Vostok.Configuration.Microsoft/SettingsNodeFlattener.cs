using System;
using System.Collections.Generic;
using System.Linq;
using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Configuration.Microsoft
{
    internal static class SettingsNodeFlattener
    {
        public static IDictionary<string, string> Flatten(this ISettingsNode settingsNode)
        {
            var context = new List<string>();
            IDictionary<string, string> result = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (settingsNode != null)
                VisitNodeInternal(settingsNode, context, result);

            return result;
        }

        private static void VisitNode(
            ISettingsNode settingsNode,
            List<string> context,
            IDictionary<string, string> result)
        {
            if (settingsNode == null)
                return;

            context.Add(settingsNode.Name);
            VisitNodeInternal(settingsNode, context, result);
            context.RemoveAt(context.Count - 1);
        }

        private static void VisitNodeInternal(
            ISettingsNode settingsNode,
            List<string> context,
            IDictionary<string, string> result)
        {
            switch (settingsNode)
            {
                case ValueNode valueNode:
                    result[AssemblePath(context)] = valueNode.Value;
                    break;

                case ArrayNode arrayNode:
                    VisitArray(arrayNode.Children, context, result);
                    break;

                case ObjectNode objectNode:
                    VisitObject(objectNode.Children, context, result);
                    break;
            }
        }

        private static void VisitObject(
            IEnumerable<ISettingsNode> nodes,
            List<string> context,
            IDictionary<string, string> result)
        {
            foreach (var node in nodes)
            {
                VisitNode(node, context, result);
            }
        }

        private static void VisitArray(
            IEnumerable<ISettingsNode> nodes,
            List<string> context,
            IDictionary<string, string> result)
        {
            var idx = 0;
            foreach (var node in nodes)
            {
                var appendIndex = node.Name == null;
                if (appendIndex)
                    context.Add(idx.ToString());

                VisitNode(node, context, result);

                if (appendIndex)
                    context.RemoveAt(context.Count - 1);

                idx++;
            }
        }

        private static string AssemblePath(List<string> parts)
        {
            return string.Join(":", parts.Where(s => !string.IsNullOrEmpty(s)));
        }
    }
}