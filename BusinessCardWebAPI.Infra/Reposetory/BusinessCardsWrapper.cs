using BusinessCardWebAPI.Core.DTO;
using System.Xml.Serialization;

namespace BusinessCardWebAPI.Infra.Reposetory
{
    [XmlRoot("BusinessCards")]
    public class BusinessCardsWrapper
    {
        [XmlElement("BusinessCard")]
        public List<CreateBusinessCardsDto> BusinessCards { get; set; }
    }

}

