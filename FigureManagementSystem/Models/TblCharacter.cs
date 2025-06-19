using System;
using System.Collections.Generic;

namespace FigureManagementSystem.Models;

public partial class TblCharacter
{
    public int CharacterId { get; set; }

    public string CharacterName { get; set; } = null!;

    public int SeriesId { get; set; }

    public string MainColor { get; set; } = null!;

    public int? PopularityRank { get; set; }

    public bool? IsActive { get; set; }

    public virtual TblSeries Series { get; set; } = null!;

    public virtual ICollection<TblProduct> TblProducts { get; set; } = new List<TblProduct>();
}
