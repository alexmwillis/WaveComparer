using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WaveComparerLib.Gen_Utils
{
    [AttributeUsage(AttributeTargets.Property| AttributeTargets.Method)]
    public class ToolableAttribute : Attribute
    {
        /// <summary>
        /// Indicates that the member can to be included in a tool bar
        /// </summary>
        /// <param name="toolName">Tool Name</param>
        /// <param name="toolCategory">Tool Category</param>
        public ToolableAttribute(string toolName, string toolCategory)
        {
            this.ToolName = toolName;
            this.ToolCategory = toolCategory;
        }

        public string ToolName { get; private set; }
        public string ToolCategory { get; private set; }
    }
}
