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
    {Card.Id},
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
                    var tempName = enName.Replace(" ", string.Empty).ToLower();
                    ChuckCardId cardId = new ChuckCardId();
                    cardId.Card = item;
                    cardId.ChineseName = item.GetLocName(Locale.zhCN);
                    cardId.EnglishName = item.GetLocName(Locale.enUS);
                    cardId.TrimmedEnglishName = tempName;
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

        public void GenerateCardNameEnum()
        {
            Dictionary<string, Card> dic = Cards.All;
            List<ChuckCardName> list = new List<ChuckCardName>();
            foreach (Card item in dic.Values)
            {
                var enName = item.GetLocName(Locale.enUS);
                if (!string.IsNullOrEmpty(enName))
                {
                    var tempName = enName.Replace(" ", string.Empty).ToLower();
                    ChuckCardName cardName = new ChuckCardName();
                    cardName.Card = item;
                    cardName.ChineseName = item.GetLocName(Locale.zhCN);
                    cardName.EnglishName = item.GetLocName(Locale.enUS);
                    cardName.TrimmedEnglishName = tempName;
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
