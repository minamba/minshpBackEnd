using MinshpWebApp.Domain.Services.Shipping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Models
{
    public class ShipmentResultV3
    {
        public string? ShippingOrderId { get; set; }
        public string? ShipmentId { get; set; }
        public string? Status { get; set; }
        public string? CarrierCode { get; set; }
        public string? ProductCode { get; set; }
        public string? ServiceCode { get; set; }
        public string? TrackingNumber { get; set; }
        public string? LabelUrl { get; set; }
        public Money? DeliveryPriceExclTax { get; set; }
        public Money? InsurancePriceExclTax { get; set; }
        public DateTimeOffset? EstimatedDeliveryDate { get; set; }
        public DateTimeOffset? ExpectedTakingOverDate { get; set; }
    }

    public class Money {
     public decimal? Value { get; set; }
        public string? Currency { get; set; }
    }




    public static class ShipmentResultMapper
    {
        public static ShipmentResult ToLegacy(ShipmentResultV3 r) =>
            new(
                providerShipmentId: r.ShippingOrderId ?? "",
                carrier: r.CarrierCode ?? "Boxtal",
                serviceCode: r.ServiceCode ?? r.ProductCode ?? "",
                trackingNumber: r.TrackingNumber ?? "",
                labelUrl: r.LabelUrl ?? ""
            );
    }

}
