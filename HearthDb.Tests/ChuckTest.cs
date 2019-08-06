using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using HearthDb.Enums;
using NUnit.Framework;

namespace HearthDb.Tests
{
    public class TrimHelper
    {
        public static string TrimEnglishName(string temp)
        {
            if (temp == null)
            {
                return null;
            }
            temp = temp.Replace("&lt;", "");
            temp = temp.Replace("b&gt;", "");
            temp = temp.Replace("/b&gt;", "");
            temp = temp.ToLower(new System.Globalization.CultureInfo("en-US", false));
            temp = temp.Replace("'", "");
            temp = temp.Replace(" ", "");
            temp = temp.Replace(":", "");
            temp = temp.Replace(".", "");
            temp = temp.Replace("!", "");
            temp = temp.Replace("?", "");
            temp = temp.Replace("-", "");
            temp = temp.Replace("_", "");
            temp = temp.Replace(",", "");
            temp = temp.Replace("(", "");
            temp = temp.Replace(")", "");
            temp = temp.Replace("/", "");
            temp = temp.Replace("\"", "");
            temp = temp.Replace("+", "");
            temp = temp.Replace("’", "");
            temp = temp.Replace("=", "");
            return temp;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ChuckCardId
    {
        public Card Card { get; set; }
        public string EnglishName { get; set; }
        public string ChineseName { get; set; }
        public string TrimmedEnglishName { get; set; }

        public override string ToString()
        {
            return $@"
    /// <summary>
    /// {Card.Id}
    /// {EnglishName}
    /// {ChineseName}
    /// {Card.Set}
    /// {Card.Class}
    /// {Card.Type}
    /// Collectible = {Card.Collectible}
    /// {Card.Text?.Replace("\n","\n/// ")}
    /// </summary>
    {Card.Id},
";
        }
    }

    public class ChuckCardName : ChuckCardId
    {
        public override string ToString()
        {
            return $@"
    /// <summary>
    /// {Card.Id}
    /// {EnglishName}
    /// {ChineseName}
    /// {Card.Set}
    /// {Card.Class}
    /// {Card.Type}
    /// Collectible = {Card.Collectible}
    /// {Card.Text?.Replace("\n", "\n/// ")}
    /// </summary>
    {TrimmedEnglishName},
";
        }

    }

    [TestFixture]
    public class ChuckTest
    {
        [Test]
        public void GenerateCardIdEnum()
        {
            Dictionary<string, Card> dic = Cards.All;
            List<ChuckCardId> list = new List<ChuckCardId>();
            foreach (Card item in dic.Values)
            {
                var enName = item.GetLocName(Locale.enUS);
                if (!string.IsNullOrEmpty(enName))
                {
                    ChuckCardId cardId = new ChuckCardId();
                    cardId.Card = item;
                    cardId.ChineseName = item.GetLocName(Locale.zhCN);
                    cardId.EnglishName = enName;
                    cardId.TrimmedEnglishName = TrimHelper.TrimEnglishName(enName);
                    list.Add(cardId);
                }
            }

            Dictionary<string, string> dic2 = new Dictionary<string, string>();
            var cardSets = list.GroupBy(x => x.Card.Set);
            foreach (var cardSet in cardSets)
            {
                var tempCardSet = cardSet.OrderBy(x => x.TrimmedEnglishName);
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var card in tempCardSet)
                {
                    stringBuilder.AppendLine($"{card}");
                }

                dic2.Add(cardSet.Key.ToString(), stringBuilder.ToString());
            }

            foreach (var cardSet in dic2)
            {
                Console.WriteLine($"///{cardSet.Key}");
                Console.WriteLine(cardSet.Value);
            }
        }

        [Test]
        public void GenerateCardDb()
        {
            var cards = Cards.All;
            List<Entity> list = new List<Entity>();
            foreach (var card in cards.Values)
            {
                var originEntity = card.Entity;
                Entity entity = new Entity();
                entity.SetValue(originEntity);
                list.Add(entity);
            }

            CardDefs cardDefs=new CardDefs();
            cardDefs.Entites = list;

            XmlSerializer ser = new XmlSerializer(typeof(CardDefs));

            using (FileStream fs = new FileStream(@"c:\_carddb.txt", FileMode.Create))
            {
                ser.Serialize(fs, cardDefs);
            }
            Console.WriteLine(list.Count);
        }

        [Test]
        public void GenerateCardNameEnum()
        {
            Dictionary<string,int> trimmedEnglishNameDictionary = new Dictionary<string, int>();

            Dictionary<string, Card> dic = Cards.All;
            List<ChuckCardName> list = new List<ChuckCardName>();
            foreach (Card item in dic.Values)
            {
                var enName = item.GetLocName(Locale.enUS);
                if (!string.IsNullOrEmpty(enName))
                {
                    ChuckCardName cardName = new ChuckCardName();
                    cardName.Card = item;
                    cardName.ChineseName = item.GetLocName(Locale.zhCN);
                    cardName.EnglishName = enName;
                    var trimmedEnglishName = TrimHelper.TrimEnglishName(enName);
                    if (trimmedEnglishNameDictionary.ContainsKey(trimmedEnglishName))
                    {
                        int count = trimmedEnglishNameDictionary[trimmedEnglishName];
                        count++;
                        trimmedEnglishNameDictionary[trimmedEnglishName] = count;
                        trimmedEnglishName = $"{trimmedEnglishName}_chuck_{count}";
                    }
                    else
                    {
                        trimmedEnglishNameDictionary.Add(trimmedEnglishName, 1);
                    }

                    if (item.Id.Equals("GILA_BOSS_66p"))
                    {
                        trimmedEnglishName = "silence_hero_power";
                    }
                    else if (item.Id.Equals("TB_LethalSetup001a"))
                    {
                        trimmedEnglishName = "continue1";
                    }else if (item.Id.Equals("LOOT_333e"))
                    {
                        trimmedEnglishName = "AddLevel1";
                    }

                    cardName.TrimmedEnglishName = trimmedEnglishName;
                    list.Add(cardName);
                }
            }

            Dictionary<string, string> dic2 = new Dictionary<string, string>();
            var cardSets = list.GroupBy(x => x.Card.Set);
            foreach (var cardSet in cardSets)
            {
                var tempCardSet = cardSet.OrderBy(x => x.TrimmedEnglishName);
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var card in tempCardSet)
                {
                    stringBuilder.AppendLine($"{card}");
                }

                dic2.Add(cardSet.Key.ToString(), stringBuilder.ToString());
            }

            foreach (var cardSet in dic2)
            {
                Console.WriteLine($"///{cardSet.Key}");
                Console.WriteLine(cardSet.Value);
            }
        }
    }
}
