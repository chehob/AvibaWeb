using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class VTicketSegmentPDFInfo
    {
        [Key]
        public Guid Id { get; set; }
        public int TicketID { get; set; }

        public string OriginName { get; set; }
        public string OriginAirportName { get; set; }
        public string DestinationName { get; set; }
        public string DestinationAirportName { get; set; }

        public string AirlineName { get; set; }
        public string FlightNumber { get; set; }
        public string Class { get; set; }
        public DateTime FlightDate { get; set; }
        public DateTime ArrivalDate { get; set; }

        public string BasicFare { get; set; }
        public string Bag { get; set; }
        public string OriginCode { get; set; }
        public string DestinationCode { get; set; }
        public string IsVoid { get; set; }
        public short TZD { get; set; }
        public string Term1 { get; set; }
        public string Term2 { get; set; }
        public string ClassWords { get; set; }
        public string QRSeg { get; set; }
        public string FactualCarrier { get; set; }
    }
}
