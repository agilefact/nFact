using System;
using System.Configuration;

namespace AcceptanceTests.Configuration
{
    public class TestSettingsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new Setting();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Setting)element).Name;
        }

        public new string this[string name]
        {
            get
            {
                var element = BaseGet(name);
                if (element == null)
                    throw new ApplicationException(string.Format("Could not find a Test Setting for name '{0}.'", name));
                return ((Setting) element).Value;
            }
            set
            {
                var element = BaseGet(name);
                ((Setting) element).Value = value;
            }
        }

        public void Add(Setting element)
        {
            //LockItem = false;  // the workaround
            BaseAdd(element);
        }
    }
}