using System;
using System.Collections.Generic;

namespace CreatHoSo.Data;

public partial class Medication
{
    public int MedicationId { get; set; }

    public string MedicationName { get; set; } = null!;

    public string? Description { get; set; }

    public string? Dosage { get; set; }

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}
