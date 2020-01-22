using SimpleProxy.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class AuditAttribute : MethodInterceptionAttribute
    {
        public string ClassName;

        public AuditAttribute(string ClassName)
        {
            this.ClassName = ClassName;
        }
    }
}
