using System;
using System.Collections.Generic;

namespace CreatHoSo.Data;

public partial class Prescription
{
    public int PrescriptionId { get; set; }

    public int? PatientId { get; set; }

    public int? DoctorId { get; set; }

    public int? MedicationId { get; set; }

    public DateOnly Date { get; set; }

    public string? Dosage { get; set; }

    public string? Instructions { get; set; }

    public virtual Doctor? Doctor { get; set; }

    public virtual Medication? Medication { get; set; }

    public virtual Patient? Patient { get; set; }
}
