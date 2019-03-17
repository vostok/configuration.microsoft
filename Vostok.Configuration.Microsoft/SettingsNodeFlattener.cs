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
            var context = new Stack<string>();
            IDictionary<string, string> result = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            VisitNode(settingsNode, context, result);
            return result;
        }

        private static void VisitNode(
            ISettingsNode settingsNode,
            Stack<string> context,
            IDictionary<string, string> result)
        {
            if (settingsNode == null)
                return;

            context.Push(settingsNode.Name);
            VisitNodeInternal(settingsNode, context, result);
            context.Pop();
        }

        private static void VisitNodeInternal(
            ISettingsNode settingsNode,
            Stack<string> context,
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
            Stack<string> context,
            IDictionary<string, string> result)
        {
            foreach (var node in nodes)
            {
                VisitNode(node, context, result);
            }
        }

        private static void VisitArray(
            IEnumerable<ISettingsNode> nodes,
            Stack<string> context,
            IDictionary<string, string> result)
        {
            var idx = 0;
            foreach (var node in nodes)
            {
                context.Push(idx.ToString());
                VisitNode(node, context, result);
                context.Pop();
                idx++;
            }
        }

        private static string AssemblePath(Stack<string> parts)
        {
            return string.Join(":", parts.Reverse().Where(s => !string.IsNullOrEmpty(s)));
        }
    }
}