using System;

namespace Lab15_StudentPortalWeb.Services
{
    public class SaifStampService : ISaifStampService
    {
        public string Stamp { get; }
        public string Owner { get; }

        public SaifStampService()
        {
            Owner = "Saif Elden Khaled Nazmy Lotfy";
            Stamp = Guid.NewGuid().ToString().Substring(0, 8);
        }
    }
}
