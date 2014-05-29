using System.Configuration;

namespace AcceptanceTests.Configuration
{
    public class EnvironmentSettings : ConfigurationElementCollection
    {
        [ConfigurationProperty("selected")]
        public string Selected
        {
            get { return (string)this["selected"]; }
            set { this["selected"] = value; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new Environment();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Environment)element).Name;
        }
    }
}