﻿using OXG.LinkCutter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OXG.LinkCutter.Data
{
    public class LinkManager
    {
        string alphabet = "qwertyuiopasdfghjklzxcvbnm1234567890";//Отсюда берутся символы для генерации сокращённой ссылки
        string pattern = @"/(?:([^\:]*)\:\/\/)?(?:([^\:\@]*)(?:\:([^\@]*))?\@)?(?:([^\/\:]*)\.(?=[^\.\/\:]*\.[^\.\/\:]*))?([^\.\/\:]*)(?:\.([^\/\.\:]*))?(?:\:([0-9]*))?(\/[^\?#]*(?=.*?\/)\/)?([^\?#]*)?(?:\?([^#]*))?(?:#(.*))?/";//Паттерн проверки регулярного выражения URL

        StringBuilder tempStr;//генерированная строка
        Random rnd;//Генератор случайных чисел
        Link _link;//Возвращаемый объект

        public LinkManager()
        {
            _link = new Link();
            rnd = new Random();
            tempStr = new StringBuilder();
        }

        public Link Cut(string link) 
        {
            if (Regex.IsMatch(link, pattern, RegexOptions.IgnoreCase))
            {
                _link.OriginalLink = link;
                for (int i = 0; i < 6; i++)
                {
                   tempStr.Append(alphabet[rnd.Next(alphabet.Length-1)]);
                }
                _link.CuttedLink = tempStr.ToString();
                return _link;
            }
            else
            {
                return null;
            }
        }
    }
}
