using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace DevSpot.Antigravity.AcceptanceTests.Support.Forms
{
    public class FormParser
    {
        public FormSession ParseForm(string html, string? formActionOrId = null)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            HtmlNode? formNode = null;

            if (!string.IsNullOrEmpty(formActionOrId))
            {
                // Try finding by ID first
                formNode = doc.GetElementbyId(formActionOrId);

                // If not found, try finding by Action containing the string
                if (formNode == null)
                {
                    formNode = doc.DocumentNode.SelectSingleNode($"//form[contains(@action, '{formActionOrId}')]");
                }
            }

            // Fallback to first form on the page
            if (formNode == null)
            {
                formNode = doc.DocumentNode.SelectSingleNode("//form");
            }

            if (formNode == null)
            {
                throw new InvalidOperationException("No form found on the page.");
            }

            var session = new FormSession
            {
                Action = formNode.GetAttributeValue("action", ""),
                Method = formNode.GetAttributeValue("method", "POST")
            };

            Console.WriteLine($"[FormParser] Found form action: '{session.Action}', method: '{session.Method}'");

            // Extract all inputs
            var inputs = formNode.SelectNodes(".//input");
            if (inputs != null)
            {
                foreach (var input in inputs)
                {
                    var name = input.GetAttributeValue("name", "");
                    var type = input.GetAttributeValue("type", "");
                    var val = input.GetAttributeValue("value", "");

                    if (!string.IsNullOrEmpty(name))
                    {
                        if (type == "radio")
                        {
                            var isChecked = input.Attributes.Contains("checked");
                            if (isChecked)
                            {
                                session.SetField(name, val);
                                Console.WriteLine($"[FormParser] Parsed radio: '{name}' = '{val}' (checked)");
                            }
                            else
                            {
                                Console.WriteLine($"[FormParser] Ignored unchecked radio: '{name}' = '{val}'");
                            }
                        }
                        else if (type == "checkbox")
                        {
                            var isChecked = input.Attributes.Contains("checked");
                            session.SetField(name, isChecked ? "true" : "false");
                            Console.WriteLine($"[FormParser] Parsed checkbox: '{name}' = '{isChecked}'");
                        }
                        else
                        {
                            session.SetField(name, val);
                            Console.WriteLine($"[FormParser] Parsed input: '{name}' = '{val}'");
                        }
                    }
                }
            }

            // Extract textareas
            var textareas = formNode.SelectNodes(".//textarea");
            if (textareas != null)
            {
                foreach (var textarea in textareas)
                {
                    var name = textarea.GetAttributeValue("name", "");
                    var val = textarea.InnerText;
                    if (!string.IsNullOrEmpty(name))
                    {
                        session.SetField(name, val);
                        Console.WriteLine($"[FormParser] Parsed textarea: '{name}' = '{val}'");
                    }
                }
            }

            // Extract select options
            var selects = formNode.SelectNodes(".//select");
            if (selects != null)
            {
                foreach (var select in selects)
                {
                    var name = select.GetAttributeValue("name", "");
                    if (!string.IsNullOrEmpty(name))
                    {
                        var selectedOption = select.SelectSingleNode(".//option[@selected]");
                        var val = selectedOption != null ? selectedOption.GetAttributeValue("value", "") : "";
                        session.SetField(name, val);
                        Console.WriteLine($"[FormParser] Parsed select: '{name}' = '{val}'");
                    }
                }
            }

            return session;
        }
    }
}
