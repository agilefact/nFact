using System.Configuration;

namespace AcceptanceTests.Configuration
{
    public class Setting : ConfigurationSection
    {
        [ConfigurationProperty("name")]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("value")]
        public string Value
        {
            get { return (string)this["value"]; }
            set { this["value"] = value; }
        }

        [ConfigurationProperty("visible", DefaultValue = "True")]
        public bool Visible
        {
            get { return (bool)this["visible"]; }
            set { this["visible"] = value; }
        }

        [ConfigurationProperty("editable", DefaultValue = "True")]
        public bool Editable
        {
            get { return (bool)this["editable"]; }
            set { this["editable"] = value; }
        }
        
    }
}