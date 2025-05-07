using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPToolsUI
{
    public static class StringExtensions
    {
        public static bool Contains(this string? source, string? value, StringComparison comparison)
        {
            if (source is null) return false;
            if (value is null) return false;
            return source.IndexOf(value, comparison) >= 0;
        }
    }
}
