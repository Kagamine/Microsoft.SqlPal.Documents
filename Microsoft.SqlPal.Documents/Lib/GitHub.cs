using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Text;

namespace Microsoft.SqlPal.Documents.Lib
{
    public static class GitHub
    {
        public static string GetRawFile(string Endpoint)
        {
            var path = Path.Combine(Startup.Config["Docs:Path"].ToString(), Endpoint.TrimStart('/'));
            if (!File.Exists(path))
                return "";
            return File.ReadAllText(path);
        }
        
        public static string RenderTocMd()
        {
            var toc = GetTocMd();
            return TocToUl(toc.Split('\n').Select(x => x.TrimEnd('\r')).ToArray());
        }

        private static string GetTocMd(string Endpoint = "toc.md", int Level = 0)
        {
            var toc = GetRawFile(Endpoint).Split('\n').Select(x => x.TrimEnd('\r')).ToList();
            var tasks = new List<Task>();
            for (var i = 0; i < toc.Count; i++)
            {
                var href = AHrefRegex.Match(toc[i]).Value;
                var title = AInnerTextRegex.Match(toc[i]).Value;

                if (toc[i].StartsWith("#"))
                {
                    toc[i] = BuildSharps(Level) + toc[i];
                    if (!string.IsNullOrWhiteSpace(href))
                        toc[i] = toc[i].Replace(href, Endpoint.Substring(0, Endpoint.Length - "toc.md".Length) + href);
                }

                if (toc[i].EndsWith("toc.md)"))
                {
                    var cnt = CountLeft(toc[i], '#');
                    if (!string.IsNullOrWhiteSpace(href))
                    {
                        toc[i] = BuildSharps(cnt) + " " + title + "\n" + GetTocMd(Endpoint.Substring(0, Endpoint.Length - "toc.md".Length) + href, cnt);
                    }
                }
            }
            return string.Join("\n", toc);
        }

        private static Regex AHrefRegex = new Regex(@"(?<=\().*(?=\))");
        private static Regex AInnerTextRegex = new Regex(@"(?<=\[).*(?=\])");

        private static string TocToUl(string[] toc, int level = 1, int begin = 0)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<ul>");
            var cnt = 0;
            for (var i = begin; i < toc.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(toc[i]) || toc[i].StartsWith("<!--") || CountLeft(toc[i], '#') > level)
                    continue;
                if (CountLeft(toc[i], '#') < level)
                    break;
                cnt++;
                sb.AppendLine("<li>");
                var text = AInnerTextRegex.Match(toc[i]);
                var href = AHrefRegex.Match(toc[i].Replace($"[{ text }]", ""));
                if (href.Success && text.Success)
                {
                    sb.AppendLine($"<a href=\"{ href.Value }\">");
                    sb.AppendLine(text.Value);
                    sb.AppendLine("</a>");
                }
                else
                {
                    sb.AppendLine($"<a href=\"javascript:;\" onclick=\"Expand(this)\">");
                    sb.AppendLine(toc[i].Trim().TrimStart('#').Trim());
                    sb.AppendLine("</a>");
                }
                sb.Append(TocToUl(toc, level + 1, i + 1));
                sb.AppendLine("</li>");
            }
            sb.AppendLine("</ul>");
            if (cnt == 0)
                return "";
            else
                return sb.ToString();
        }

        private static string BuildSharps(int count)
        {
            var ret = new StringBuilder();
            for (var i = 0; i < count; i++)
                ret.Append("#");
            return ret.ToString();
        }

        private static int CountLeft(string src, char ch)
        {
            var ret = 0;
            for (var i = 0; i < src.Length; i++)
                if (src[i] == ch)
                    ret++;
                else
                    break;
            return ret;
        }

        public static string FilterMarkdown(string md)
        {
            var tmp = md.Replace("\r", "").Split('\n').ToList();
            filter:
            var cnt = -1;
            var begin = -1;
            for (var i = 0; i < tmp.Count; i++)
            {
                if (IsDash(tmp[i].TrimEnd()))
                {
                    if (begin == -1)
                    {
                        begin = i;
                        cnt = CountDash(tmp[i]);
                    }
                    else if (CountDash(tmp[i]) == cnt && begin >= 0)
                    {
                        var gotoFlag = true;
                        for (var j = begin + 1; j < i; j++)
                        {
                            if (!string.IsNullOrWhiteSpace(tmp[j]) && tmp[j].Split(':').Length != 2)
                            {
                                gotoFlag = false;
                                break;
                            }
                        }
                        if (gotoFlag)
                        {
                            tmp.RemoveRange(begin, i - begin + 1);
                            goto filter;
                        }
                    }
                }
            }
            return string.Join("\r\n", tmp);
        }

        private static bool IsDash(string src)
        {
            for (var i = 0; i < src.Length; i++)
                if (src[i] != '-')
                    return false;
            return true;
        }

        private static int CountDash(string src)
        {
            var ret = 0;
            for (var i = 0; i < src.Length; i++)
                if (src[i] == '-')
                    ret++;
            return ret;
        }
    }
}
