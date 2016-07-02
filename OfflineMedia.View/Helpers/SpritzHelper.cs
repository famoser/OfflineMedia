using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Famoser.FrameworkEssentials.Logging;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels;
using Famoser.OfflineMedia.Business.Models.NewsModel.ContentModels.TextModels;

namespace Famoser.OfflineMedia.View.Helpers
{
    public class SpritzHelper
    {
        public static List<SpritzWord> GenerateList(ObservableCollection<BaseContentModel> contentModels)
        {
            var words = new List<SpritzWord>();
            try
            {
                foreach (var baseContentModel in contentModels)
                {
                    if (baseContentModel is TextContentModel)
                    {
                        var textModel = (TextContentModel)baseContentModel;
                        foreach (var paragraphModel in textModel.Content)
                        {
                            foreach (var model in paragraphModel.Children)
                            {
                                var list = ToSpritzWords(model);
                                words.AddRange(list);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.FatalError, "GenerateList failed", "SpritzHelper", ex);
                return null;
            }

            return words;
        }

        private static List<SpritzWord> ToSpritzWords(TextModel model)
        {
            var words = new List<SpritzWord>();
            string[] splitresult = model.Text.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
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
                        sw.Middle = wordlist[i][0];
                    else if (wordlist[i].Length >= 2 && wordlist[i].Length <= 5)
                    {
                        sw.Before = wordlist[i][0].ToString();
                        sw.Middle = wordlist[i][1];
                        sw.After = wordlist[i].Substring(2);
                    }
                    else if (wordlist[i].Length >= 6 && wordlist[i].Length <= 9)
                    {
                        sw.Before = wordlist[i].Substring(0, 2);
                        sw.Middle = wordlist[i][2];
                        sw.After = wordlist[i].Substring(3);
                    }
                    else //(wordlist[i].Length >= 10)
                    {
                        sw.Before = wordlist[i].Substring(0, 3);
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

            foreach (var textModel in model.Children)
            {
                var list = ToSpritzWords(textModel);
                words.AddRange(list);
            }
            return words;
        } 
    }
}
