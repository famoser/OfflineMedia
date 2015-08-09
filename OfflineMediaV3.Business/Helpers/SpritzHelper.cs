using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using OfflineMediaV3.Business.Models;
using OfflineMediaV3.Business.Models.NewsModel;
using OfflineMediaV3.Common.Framework.Logs;

namespace OfflineMediaV3.Business.Helpers
{
    public static class SpritzHelper
    {
        public static List<SpritzWord> GenerateList(List<ContentModel> contentModel)
        {
            var words = new List<SpritzWord>();
            try
            {
                foreach (var content in contentModel)
                {
                    if (content.Html != null)
                    {
                        string cleanstring = Regex.Replace(content.Html, "<.*?>", String.Empty);
                        string[] splitresult = cleanstring.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        if (splitresult != null)
                        {
                            var wordlist = new List<string>(splitresult);

                            for (int i = 0; i < wordlist.Count; i++)
                            {
                                if (wordlist[i].Contains("-") && wordlist[i].IndexOf("-") != wordlist[i].Length - 1 && !Regex.IsMatch(wordlist[i], "{0-9}-{0-9}"))
                                {
                                    int index = wordlist[i].IndexOf("-") + 1;
                                    wordlist.Insert(i + 1, wordlist[i].Substring(index));
                                    wordlist[i] = wordlist[i].Substring(0, index);
                                }

                                //ab 14 muss getrennt werden, versuche dann, das word zu splitten
                                if (wordlist[i].Length > 13)
                                {
                                    //wenn wort länger als 13 + 13 werden einfach die ersten 13 buchstaben genommen
                                    if (wordlist[i].Length > 26)
                                    {
                                        wordlist.Insert(i + 1, wordlist[i].Substring(13));
                                        wordlist[i] = wordlist[i].Substring(0, 13);
                                    }
                                    else
                                    {
                                        //wordlist wird zweigeteilt
                                        int count = wordlist[i].Length / 2;
                                        wordlist.Insert(i + 1, wordlist[i].Substring(count));
                                        wordlist[i] = wordlist[i].Substring(0, count);
                                    }
                                }

                                var sw = new SpritzWord();
                                if (wordlist[i].Length == 1)
                                    sw.Middle = wordlist[1][0];
                                else if (wordlist[i].Length >= 2 && wordlist[i].Length <= 5)
                                {
                                    sw.Before = wordlist[i][0].ToString();
                                    sw.Middle = wordlist[i][1];
                                    sw.After = wordlist[i].Substring(2);
                                }
                                else if (wordlist[i].Length >= 6 && wordlist[i].Length <= 9)
                                {
                                    sw.Before = wordlist[i].Substring(0, 2).ToString();
                                    sw.Middle = wordlist[i][2];
                                    sw.After = wordlist[i].Substring(3);
                                }
                                else //(wordlist[i].Length >= 10)
                                {
                                    sw.Before = wordlist[i].Substring(0, 3).ToString();
                                    sw.Middle = wordlist[i][3];
                                    sw.After = wordlist[i].Substring(4);
                                }
                                if (sw.After != null)
                                {
                                    if (sw.After.Contains("."))
                                    {
                                        sw.Lenght = 4;
                                        words.Add(sw);
                                        words.Add(new SpritzWord() { Lenght = 5 });
                                    }
                                    else if (sw.After.Contains(";") || sw.After.Contains(",") || sw.After.Contains(":") || sw.Middle == '-')
                                    {
                                        sw.Lenght = 4;
                                        words.Add(sw);
                                    }
                                    else
                                    {
                                        sw.Lenght = 1;
                                        words.Add(sw);
                                    }
                                }
                                else
                                {
                                    sw.Lenght = 1;
                                    words.Add(sw);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.FatalError, "SpritzHelper", "GenerateList failed", ex);
                return null;
            }
           
            return words;
        }
    }
}
