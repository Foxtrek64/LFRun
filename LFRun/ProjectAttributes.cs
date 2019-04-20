using System;
using System.Linq;
using System.Reflection;

namespace LFRun
{
    public class ProjectAttributes
    {
        private readonly AssemblyTitleAttribute _assemblyTitleAttribute;
        private readonly AssemblyDescriptionAttribute _assemblyDescriptionAttribute;
        private readonly AssemblyVersionAttribute _assemblyVersionAttribute;
        private readonly AssemblyCompanyAttribute _assemblyCompanyAttribute;
        private readonly AssemblyCopyrightAttribute _assemblyCopyrightAttribute;
        public string Title => _assemblyTitleAttribute?.Title ?? "Unknown";
        public string Description => _assemblyDescriptionAttribute?.Description ?? "Unknown";
        public string Version => _assemblyVersionAttribute?.Version ?? "Unknown";
        public string Company => _assemblyCompanyAttribute?.Company ?? "Unknown";
        public string Copyright => _assemblyCopyrightAttribute?.Copyright?.Replace("{Company}", Company) ?? "Unknown";

        public ProjectAttributes()
        {
            _assemblyTitleAttribute = GetAttribute<AssemblyTitleAttribute>();
            _assemblyDescriptionAttribute = GetAttribute<AssemblyDescriptionAttribute>();
            _assemblyVersionAttribute = GetAttribute<AssemblyVersionAttribute>();
            _assemblyCompanyAttribute = GetAttribute<AssemblyCompanyAttribute>();
            _assemblyCopyrightAttribute = GetAttribute<AssemblyCopyrightAttribute>();
        }

        public TAttribute GetAttribute<TAttribute>()
            where TAttribute : Attribute
        {
            return Assembly.GetEntryAssembly()
                .GetCustomAttributes(typeof(TAttribute))
                .OfType<TAttribute>()
                .SingleOrDefault();
        }
    }
}
