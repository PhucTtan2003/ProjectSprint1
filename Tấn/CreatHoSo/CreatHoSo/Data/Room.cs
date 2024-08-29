using System;
using System.Collections.Generic;

namespace CreatHoSo.Data;

public partial class Room
{
    public int RoomId { get; set; }

    public string RoomNumber { get; set; } = null!;

    public int? DepartmentId { get; set; }

    public string? RoomType { get; set; }

    public string? Status { get; set; }

    public virtual Department? Department { get; set; }
}
