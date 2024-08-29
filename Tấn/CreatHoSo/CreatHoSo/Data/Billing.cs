using System;
using System.Collections.Generic;

namespace CreatHoSo.Data;

public partial class Billing
{
    public int BillingId { get; set; }

    public int? PatientId { get; set; }

    public DateOnly Date { get; set; }

    public decimal Amount { get; set; }

    public string? Status { get; set; }

    public virtual Patient? Patient { get; set; }
}
