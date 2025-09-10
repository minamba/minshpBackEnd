using MinshpConsolApp.Models;
using System;
using System.Collections.Generic;

namespace MinshpWebApp.Dal.Entities;

public partial class Order
{
    public int Id { get; set; }

    public DateTime? Date { get; set; }

    public string? Status { get; set; }

    public string? PaymentMethod { get; set; }

    public decimal? Amount { get; set; }

    public long OrderNumberInt { get; set; }

    public string? OrderNumber { get; set; }

    public int? CustomerId { get; set; }

    public decimal? DeliveryAmount { get; set; }


    //PARTIE SHIPPING

    public string? DeliveryMode { get; set; } = "home";   // "home" | "relay"
    public string? Carrier { get; set; } = "";            // ex: "Chronopost"
    public string? ServiceCode { get; set; } = "";        // code offre Boxtal
    public string? RelayId { get; set; }                 // id point relais
    public string? RelayLabel { get; set; }              // libellé point relais
    public string? BoxtalShipmentId { get; set; }        // id envoi Boxtal
    public string? TrackingNumber { get; set; }          // n° suivi

    public string? TrackingLink { get; set; }
    public string? LabelUrl { get; set; }                // URL étiquette

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<OrderCustomerProduct> OrderCustomerProducts { get; set; } = new List<OrderCustomerProduct>();
}
