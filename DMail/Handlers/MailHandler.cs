using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DMail.Handlers
{
    internal class FileMailHandler
    {
        private string domain = Startup.DomainName;
        public MailHandler()
        {

        }

        public async Task<string> HandleMail(string reversePath, string forwardPath, string mail)
        {
            // First ensure all newlines are CRLF newlines.
            reversePath = NormalizeNewlines(reversePath);
            forwardPath = NormalizeNewlines(forwardPath);
            mail = NormalizeNewlines(mail);
        }

        public string NormalizeNewlines(string dirtyString)
        {
            return Regex.Replace(dirtyString, @"(?<![\n\r])[\n\r](?![\n\r])", "\r\n");
        }
    }
}
