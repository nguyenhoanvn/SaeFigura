using System;
using System.Collections.Generic;

namespace FigureManagementSystem.Models;

public partial class TblSeries
{
    public int SeriesId { get; set; }

    public string SeriesName { get; set; } = null!;

    public bool? IsActive { get; set; }

    public virtual ICollection<TblCharacter> TblCharacters { get; set; } = new List<TblCharacter>();
}
