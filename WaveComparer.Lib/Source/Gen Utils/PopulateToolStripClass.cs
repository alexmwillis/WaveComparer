using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace WaveComparerApplication
{
    struct ParsedEnum
    {
        public readonly Type EnumType;
        public readonly string[] Values;

        public ParsedEnum(Type enumType)
        {
            this.EnumType = enumType;
            this.Values = Enum.GetNames(enumType);
        }
    }       

    // *** This class needs tiding up
    class PopulateToolStripClass
    {
        static List<ParsedEnum> parsedEnumTypes = new List<ParsedEnum>();
            
        public static void PopulateToolStrip(ToolStripContainer toolStripContainer, object o, List<Type> toolableEnumTypes)
        {
            foreach (var enumType in toolableEnumTypes)
            {
                parsedEnumTypes.Add(new ParsedEnum(enumType));
            }
            string[] enumValues;

            List<ToolStrip> toolStripList = new List<ToolStrip>();
            toolStripList = GetCategoryToolStripList(o);
            foreach (var toolStrip in toolStripList)
            {
                toolStripContainer.TopToolStripPanel.Controls.Add(toolStrip);
            }

            var toolableMembers = GetToolableMembers(o);
            foreach (var member in toolableMembers)
            {
                var toolStrip = GetToolStripFromCategory(toolStripList, GetToolCategory(member));
                if (member is PropertyInfo)
                {
                    var property = member as PropertyInfo;
                    if (property.PropertyType == typeof(bool))
                    {
                        AddBooleanTool(o, property, toolStrip);
                    }
                    //else if (property.PropertyType == typeof(int))
                    //{
                    //    AddIntTool(o, property, toolStrip);
                    //}
                    else if (IsToolableEnumType(property.PropertyType, out enumValues))
                    {
                        AddEnumTool(o, property, toolStrip, enumValues);
                    }
                    else
                    {
                        throw new ArgumentException("Property is not toolable", member.Name);
                    }
                }
                else if (member is MethodInfo)
                {
                    var method = member as MethodInfo;
                    AddMethodTool(o, method, toolStrip);
                }
            }
        }
        
        static void AddEnumTool(object o, PropertyInfo property, ToolStrip toolStrip, string[] enumValues)
        {
            var comboBox = new ToolStripComboBox();
            toolStrip.Items.Add(comboBox);
            comboBox.Items.AddRange(enumValues);
            comboBox.SelectedIndex = (int)property.GetValue(o, null);
            var temp = property;
            comboBox.SelectedIndexChanged += (sender, e) =>
                {
                    temp.SetValue(o, comboBox.SelectedIndex, null);
                };
        }

        static void AddMethodTool(object o, MethodInfo method, ToolStrip toolStrip)
        {
            var toolButton = new ToolStripButton();
            toolButton.Click += (sender, e) => { method.Invoke(o, null); }; // method should return void ***
            toolButton.Text = GetToolName(method);
            toolStrip.Items.Add(toolButton);
        }

        //static void AddIntTool(object o, MethodInfo method, ToolStrip toolStrip)
        //{
        //    var toolTextBox = new ToolStrip TextBox();
        //    toolTextBox.Leave += (

        static ToolStrip GetToolStripFromCategory(List<ToolStrip> toolStripList, string category)
        {
            foreach (var toolStrip in toolStripList)
            {
                if (toolStrip.Text == category) { return toolStrip; }
            }
            return null;
        }
 
        static List<ToolStrip> GetCategoryToolStripList(object o)
        {
            var toolStripList = new List<ToolStrip>();
            var categoryNames = new List<string>();
 
            var t = o.GetType();
            var miArray = t.GetMembers();
            foreach (var mi in miArray)
            {
                var caArray = mi.GetCustomAttributes(typeof(ToolableAttribute), false);
                if (caArray.Length > 0)
                {
                    var categoryName = GetToolCategory(mi);
                    if (categoryNames.IndexOf(categoryName) == -1)
                    {
                        var toolStrip = new ToolStrip();
                        toolStripList.Add(toolStrip);
                        categoryNames.Add(categoryName);
                        toolStrip.Name = categoryName + "ToolStrip";
                        toolStrip.Text = categoryName;
                    }
                }
            }
            return toolStripList;
        }

        static bool IsToolableEnumType(Type enumType, out string[] enumValues)
        {
            foreach (var p in parsedEnumTypes)
            {
                if (p.EnumType == enumType)
                {
                    enumValues = p.Values;
                    return true;
                }
            }
            enumValues = null;
            return false;
        }

        static string GetToolName(MemberInfo toolableMember)
        {
            var attrList = toolableMember.GetCustomAttributes(typeof(ToolableAttribute), false);
            if (attrList.Length > 0)
            {
                return (attrList[0] as ToolableAttribute).ToolName;
            }
            else throw new ArgumentException("Not a toolable property", "MemberInfo");
        }
        
        static string GetToolCategory(MemberInfo toolableMember)
        {
            var attrList = toolableMember.GetCustomAttributes(typeof(ToolableAttribute), false);
            if (attrList.Length > 0)
            {
                return (attrList[0] as ToolableAttribute).ToolCategory;
            }
            else throw new ArgumentException("Not a toolable property", "MemberInfo");
        }

        public static List<MemberInfo> GetToolableMembers(object o)
        {
            var _toolableMembers = new List<MemberInfo>();
            var t = o.GetType();
            var miArray = t.GetMembers();
            foreach (var mi in miArray)
            {
                var caArray = mi.GetCustomAttributes(typeof(ToolableAttribute), false);
                if (caArray.Length > 0)
                {
                    _toolableMembers.Add(mi);
                }
            }
            return _toolableMembers;
        }

        public static List<MethodInfo> GetToolableMethods(object o)
        {
            var _toolableMethods = new List<MethodInfo>();
            var t = o.GetType();
            var miArray = t.GetMethods();
            foreach (var mi in miArray)
            {
                var caArray = mi.GetCustomAttributes(typeof(ToolableAttribute), false);
                if (caArray.Length > 0)
                {
                    _toolableMethods.Add(mi);
                }
            }
            return _toolableMethods;
        }
        static void AddBooleanTool(object o, PropertyInfo property, ToolStrip toolStrip)
        {
            var toolName = GetToolName(property);
            var toolButton = new ToolStripButton();
            if ((bool)property.GetValue(o, null))
            {
                toolButton.Text = toolName + ": On";
            }
            else
            {
                toolButton.Text = toolName + ": Off";
            }
            toolButton.ToolTipText = toolName;
            toolButton.Click += (sender, e) =>
                {
                    if ((bool)property.GetValue(o, null))
                    {
                        property.SetValue(o, false, null);
                        toolButton.Text = toolName + ": Off";
                    }
                    else
                    {
                        property.SetValue(o, true, null);
                        toolButton.Text = toolName + ": On";
                    }
                };
            toolStrip.Items.Add(toolButton);
        }
    }
}
