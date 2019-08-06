using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using HearthDb.CardDefs;
using HearthDb.Enums;

namespace HearthDb.Tests
{
    [XmlRoot("CardDefs")]
    public class CardDefs
    {
        [XmlElement("Entity")]
        public List<Entity> Entites { get; set; }
    }

    public class Entity
    {
        public void SetValue(HearthDb.CardDefs.Entity entity)
        {
            CardId = entity.CardId;
            DbfId = entity.DbfId;
            Version = entity.Version;
            foreach (var tag in entity.Tags)
            {
                Tag temp = new Tag();
                temp.SetValue(tag);
                Tags.Add(temp);
            }

            foreach (var tag in entity.ReferencedTags)
            {
                Tag temp = new Tag();
                temp.SetValue(tag);
                ReferencedTags.Add(temp);
            }

            Power = entity.Power;

        }

        [XmlAttribute("CardID")]
        public string CardId { get; set; }

        [XmlAttribute("ID")]
        public int DbfId { get; set; }

        [XmlAttribute("version")]
        public int Version { get; set; }

        [XmlElement("Tag")]
        public List<Tag> Tags { get; set; } = new List<Tag>();

        [XmlElement("ReferencedTag")]
        public List<Tag> ReferencedTags { get; set; } = new List<Tag>();

        [XmlElement("Power")]
        public Power Power { get; set; }

        public int GetTag(GameTag gameTag) => Tags.FirstOrDefault(x => x.EnumId == (int)gameTag)?.Value ?? 0;

        public int GetReferencedTag(GameTag gameTag) => ReferencedTags.FirstOrDefault(x => x.EnumId == (int)gameTag)?.Value ?? 0;

        public string GetInnerValue(GameTag gameTag) => Tags.FirstOrDefault(x => x.EnumId == (int)gameTag)?.InnerValue;

    }
}
