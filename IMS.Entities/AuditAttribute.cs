using SimpleProxy.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class AuditAttribute : MethodInterceptionAttribute
    {
        public string ProcessName;
        public string ClassName;

        public AuditAttribute(string ProcessName,string ClassName)
        {
            this.ProcessName = ProcessName;
            this.ClassName = ClassName;
        }
    }
}
