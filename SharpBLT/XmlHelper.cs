namespace SharpBLT;

using System.Text;
using System.Xml.Linq;

public static class XmlHelper
{
    private const string MergePrefix = "__merge_";

    public static string PreprocessXml(string xml)
    {
        StringBuilder sb = new();
        bool insideElement = false;
        bool insideAttribute = false;

        for (int i = 0; i < xml.Length; i++)
        {
            char c = xml[i];
            if (c == '<' && !insideAttribute)
            {
                insideElement = true;
                sb.Append(c);
            }
            else if (c == '>' && !insideAttribute)
            {
                insideElement = false;
                sb.Append(c);
            }
            else if (c == '"' && insideElement)
            {
                insideAttribute = !insideAttribute;
                sb.Append(c);
            }
            else if (c == ':' && insideElement && !insideAttribute)
            {
                // Replace `:` with a fake prefix, to work around dotnet's strictness regarding ':' and xml-namespaces
                sb.Append(MergePrefix);
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

    public static void ApplyMergedAttributes(XElement root)
    {
        // Apply merging logic
        TraverseAndMerge(root);

        // Clean up the prefix after merging
        RemovePrefix(root);
    }

    private static void TraverseAndMerge(XElement element)
    {
        // Apply merging logic for the current element
        Dictionary<string, string> mergedAttributes = [];

        // Process current element's attributes
        foreach (XAttribute attr in element.Attributes())
        {
            string prefixedName = attr.Name.LocalName;

            if (prefixedName.StartsWith(MergePrefix))
            {
                mergedAttributes[prefixedName] = MergeParentAttributes(element, attr);
            }
            else
            {
                mergedAttributes[prefixedName] = attr.Value;
            }
        }

        // Set merged attributes to the current element
        foreach (KeyValuePair<string, string> attr in mergedAttributes)
        {
            element.SetAttributeValue(attr.Key, attr.Value);
        }

        // Traverse children
        foreach (XElement child in element.Elements())
        {
            TraverseAndMerge(child);
        }
    }

    private static string MergeParentAttributes(XElement element, XAttribute currentAttr)
    {
        string currentPrefixedName = currentAttr.Name.LocalName;
        string value = currentAttr.Value;

        int prefixedChildren = element.Elements().Where(element => element.Attributes().Where(attr => attr.Name.LocalName.Equals(currentPrefixedName)).Any()).Count();

        if (prefixedChildren <= 0) // ignore midway prefixes, only handle last ones
        {
            XElement? ancestor = element.Parent;

            // Traverse up the tree to gather and merge attributes
            while (ancestor != null)
            {
                XAttribute? parentAttrWithPrefix = ancestor.Attribute(currentPrefixedName);
                XAttribute? parentAttrWithoutPrefix = ancestor.Attribute(currentPrefixedName[MergePrefix.Length..]);

                if (parentAttrWithPrefix != null)
                {
                    value = $"{parentAttrWithPrefix.Value}{value}";
                }
                else if (parentAttrWithoutPrefix != null)
                {
                    value = $"{parentAttrWithoutPrefix.Value}{value}";
                    break; // stop traversing at unprefixed parent
                }

                ancestor = ancestor.Parent;
            }
        }

        return value;
    }

    private static void RemovePrefix(XElement element)
    {
        // Collect prefixed attributes (i.e., merged ones) and unprefixed ones for cleanup
        List<XAttribute> attributesToClean = element.Attributes().Where(attr => attr.Name.LocalName.StartsWith(MergePrefix)).ToList();

        if (attributesToClean.Count > 0)
        {
            // For each prefixed attribute, clean up the entire chain of merged attributes
            foreach (XAttribute attr in attributesToClean)
            {
                string prefixedName = attr.Name.LocalName;
                string unprefixedName = attr.Name.LocalName[MergePrefix.Length..];

                // Clean up this element's attribute by removing the prefix
                element.SetAttributeValue(unprefixedName, attr.Value);

                // Remove all parent attributes (both prefixed and unprefixed) involved in the chain
                XElement? ancestor = element.Parent;
                while (ancestor != null)
                {
                    // Remove both prefixed and unprefixed versions of the attribute
                    XAttribute? prefixedAttrInParent = ancestor.Attribute(prefixedName);
                    XAttribute? unprefixedAttrInParent = ancestor.Attribute(unprefixedName);

                    if (prefixedAttrInParent != null)
                    {
                        prefixedAttrInParent.Remove();  // Remove prefixed parent attribute
                    }
                    else if (unprefixedAttrInParent != null)
                    {
                        unprefixedAttrInParent.Remove(); // Remove unprefixed parent attribute if it was part of the chain
                        break; // stop traversing at unprefixed parent
                    }
                    else
                    {
                        break; // no more attributes found, exit early
                    }

                    ancestor = ancestor.Parent;
                }

                // Remove the original prefixed attribute from the current element
                attr.Remove();
            }
        }

        // Recursively clean child elements
        foreach (XElement child in element.Elements())
        {
            RemovePrefix(child);
        }
    }
}
