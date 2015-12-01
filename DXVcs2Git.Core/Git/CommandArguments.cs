using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXVcs2Git.Core.Git {
    public class CommandArguments {
        private readonly StringBuilder command;

        public CommandArguments() {
            this.command = new StringBuilder();
        }

        public void AddArg(string argument, params object[] args) {
            this.command.AppendFormat(CultureInfo.InvariantCulture, argument, args);
            this.command.Append(" ");
        }

        public override string ToString() {
            return this.command.ToString();
        }
    }
}
