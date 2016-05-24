using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Reflection;

namespace iostamagotchi
{
    /// <summary>
    /// This class contains references to all types in this assembly
    /// </summary>
    public class AssemblyTypes
    {
        public Type[] AssTypes = null;

        public void LoadTypes()
        {
            if (this.AssTypes == null)
            {
                Assembly a = Assembly.Load("iostamagotchi");
                this.AssTypes = a.GetTypes();
            }
        }

        /// <summary>
        /// Gets type by its name
        /// </summary>
        /// <param name="name">Name of type we're looking for</param>
        /// <returns></returns>
        public Type GetType(string name)
        {
            for (int i = 0; i < this.AssTypes.Length; i++)
                if (this.AssTypes[i].Name == name)
                    return this.AssTypes[i];

            return null; // return nothing when nothing was found
        }
    }
}
