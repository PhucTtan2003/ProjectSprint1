using System;
using System.Collections.Generic;

namespace CreatHoSo.Data;

public partial class Equipment
{
    public int EquipmentId { get; set; }

    public string EquipmentName { get; set; } = null!;

    public int? DepartmentId { get; set; }

    public string? Status { get; set; }

    public DateOnly? LastMaintenanceDate { get; set; }

    public virtual Department? Department { get; set; }
}
