using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SethBlog.Services
{
    public class ReadTimeService
    {
        public string CalcReadTime(string articleText){
            var noHtml = StripTagsCharArray(articleText);
            var wordCount = WordCount(noHtml);
            int result = (wordCount / 200) + 1;
            return result.ToString();
        }
        private int WordCount(string inputString)
        {
            return inputString.Split(" ").Length;
        }
        //Taken from https://www.dotnetperls.com/remove-html-tags, used to remove all formatting from HTML document.
        //checks if we are inside an HTML tag and removes all content inside it.  
        //THIS DOES NOT REMOVE "&nbsp;" which means the end of a paragraph will be 
        private static string StripTagsCharArray(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }
    }
}
